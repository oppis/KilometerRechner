using System.Net.Http.Headers;
using System.Net.Http;

using Kilometer_Rechner.Models;

namespace Kilometer_Rechner.Helper
{
    internal class CityDownload
    {
        /// <summary>
        /// Inhalt von der Datei herunterladen
        /// </summary>
        /// <returns>String Wert von Webseite</returns>
        private static async Task<string> GetContent()
        {
            //URL für API
            string urlTextFile = "https://basti1012.bplaced.net/town/streetsuche/OK_PLZ_entfernungsrechnung/PLZ.txt";

            try
            {
                //Abrufen der API
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage response = await httpClient.GetAsync(urlTextFile);
                httpClient.Dispose();
                if (response.IsSuccessStatusCode) //Prüfen ob erfolgreiche Meldungen -> Dann Daten Parsen
                {


                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Fehler beim Donwload Status Code: " +  response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                //TODO -> Fehlermeldung Download
                throw;
            }
        }

        /// <summary>
        /// Inhalt Konvertieren
        /// </summary>
        public static async void ConvertContent()
        {
            string currentContent = await GetContent();

            string[] contentLines = currentContent.Split(new string[] {"\n"}, StringSplitOptions.None);

            foreach (string line in contentLines) 
            { 
                string[] lineArray = line.Split(new string[] { "\t" }, StringSplitOptions.None);

                if (lineArray[0] == "#loc_id" | string.IsNullOrEmpty(lineArray[0]))
                {

                }
                else
                {
                    using (var cititesContext = new CityDbContext())
                    {
                        CityModel cityModel = new()
                        {
                            PLZ = lineArray[1],
                            Lon = float.Parse(lineArray[2]),
                            Lat = float.Parse(lineArray[3]),
                            Ort = lineArray[4],
                        };

                        cititesContext.Add(cityModel);
                        cititesContext.SaveChanges();
                    }
                }
            }

        }
    }
}
