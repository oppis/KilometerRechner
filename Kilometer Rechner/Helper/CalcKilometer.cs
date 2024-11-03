using System.Runtime.CompilerServices;

using Azure.Core;

using Kilometer_Rechner.Models;

namespace Kilometer_Rechner.Helper
{
    internal class CalcKilometer
    {
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
        /// Starten der Berechnung für die Luftkilometer und speichern in Datenbank
        /// </summary>
        /// <param name="cityFrom"></param>
        /// <param name="cities"></param>
        /// <returns></returns>
        public static async Task CalcAirLineToDB(CityModel cityFrom, List<CityModel> cities)
        {
            using DbContext dbContext = new();
            
            foreach (CityModel city in cities)
            {
                Calculation calculation = new()
                {
                    BasePlz = cityFrom.Id,
                    CalcDate = DateTime.Now,
                    IdPlz = city.Id,
                    AirLineKm = CalcKilometer.CalcAirLine(cityFrom, city),
                };

                dbContext.Add(calculation);                
            }

            dbContext.SaveChanges();
        }

        /// <summary>
        /// Berechnung der Kilometer zwisxhen zwei Punkte als Route
        /// </summary>
        /// <returns></returns>
        public static int Route()
        {
            return 0;
        }
    }
}