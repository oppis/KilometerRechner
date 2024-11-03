using System.Net.Http.Headers;
using System.Net.Http;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

using Kilometer_Rechner.Models;

namespace Kilometer_Rechner.Helper
{
    internal class CalcKilometer
    {
        private readonly DbContext _DbContext = new();
        private static int currentState = 0;
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Berechnung der Kilometer zwischen zwei Punkten per Luftlinie
        /// </summary>
        /// <returns></returns>
        private static double CalcAirLine(CityModel city1, CityModel city2)
        {
            double circumference = 40000.0; // Erdumfang in km am Äquator

            //Radianten berechnen
            double latitude1Rad = DegreesToRadians(city1.Latitude);
            double longitude1Rad = DegreesToRadians(city1.Longitude);
            double latititude2Rad = DegreesToRadians(city2.Latitude);
            double longitude2Rad = DegreesToRadians(city2.Longitude);

            double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }

            double angleCalculation =
                Math.Acos(
                  Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                  Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

            return circumference * angleCalculation / (2.0 * Math.PI);
        }

        /// <summary>
        /// Berechnung der Kilometer zwischen zwei Punkte als Route
        /// </summary>
        /// <returns></returns>
        private static async Task<Dictionary<int, double>> CalcRoute(CityModel city1, List<CityModel> cities2, System.Windows.Controls.ProgressBar pbLoadCalc)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            httpClient.DefaultRequestHeaders.ConnectionClose = false;

            Dictionary<int, double> routes = [];

            foreach (CityModel city2 in cities2)
            {
                //URL für API
                string urlApiCall = $"http://localhost:5000/route/v1/driving/{city1.Longitude.ToString(CultureInfo.InvariantCulture)},{city1.Latitude.ToString(CultureInfo.InvariantCulture)};{city2.Longitude.ToString(CultureInfo.InvariantCulture)},{city2.Latitude.ToString(CultureInfo.InvariantCulture)}?overview=false&alternatives=false&steps=false";

                HttpResponseMessage response = await httpClient.GetAsync(urlApiCall);

                if (response.IsSuccessStatusCode)
                {
                    JsonNode jsonValues = JsonSerializer.Deserialize<JsonNode>(await response.Content.ReadAsStringAsync());

                    if (jsonValues["code"].ToString() == "Ok")
                    {
                        routes.Add(city2.Id, double.Parse(jsonValues["routes"][0]["distance"].ToString()) / 1000);
                        pbLoadCalc.Dispatcher.Invoke(() => pbLoadCalc.Value = currentState++);
                    }
                    else
                    {
                        throw new Exception("Dienst konnte keine Werte berechnen!");
                    }
                }
                else
                {
                    throw new Exception("Fehler beim Berechnen Status Code: " + response.StatusCode);
                }
            }

            httpClient.Dispose();

            return routes;
        }

        /// <summary>
        /// Starten der Berechnung für die Luftkilometer und speichern in Datenbank
        /// </summary>
        /// <param name="cityFrom"></param>
        /// <param name="cities"></param>
        /// <returns></returns>
        public async Task SaveToDB(CityModel cityFrom, List<CityModel> cities, System.Windows.Controls.ProgressBar pbLoadCalc)
        {
            pbLoadCalc.Dispatcher.Invoke(() => pbLoadCalc.Maximum = _DbContext.Cities.Count());
            
            List<List<CityModel>> citiesBlocks = cities
                                    .Select((x, i) => new { Index = i, Value = x })
                                    .GroupBy(x => x.Index / 500)
                                    .Select(x => x.Select(v => v.Value).ToList())
                                    .ToList();


            foreach (List<CityModel> citiesBlock in citiesBlocks)
            {
                Dictionary<int, double> routesKm = await CalcKilometer.CalcRoute(cityFrom, citiesBlock, pbLoadCalc);

                foreach (KeyValuePair<int, double> routeKm in routesKm)
                {
                    CityModel currentCity = _DbContext.Cities.Single(city => city.Id == routeKm.Key);

                    Calculation calculation = new()
                    {
                        BasePlz = cityFrom.Id,
                        CalcDate = DateTime.Now,
                        IdPlz = routeKm.Key,
                        AirLineKm = CalcKilometer.CalcAirLine(cityFrom, currentCity),
                        RouteLineKm = routeKm.Value,
                    };

                    _DbContext.Caculations.Add(calculation);
                }
            }
            
            _DbContext.SaveChanges();
        }
    }
}