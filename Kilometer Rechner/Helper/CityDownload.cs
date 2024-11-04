using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;

using Microsoft.EntityFrameworkCore;

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
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage response = await httpClient.GetAsync(urlTextFile);
                httpClient.Dispose();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Fehler beim Download Status Code: " +  response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                UserMessage.ShowMessageBox("Download Städte", "Fehler beim Abruf: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Inhalt Konvertieren und in Datenbank speichern
        /// </summary>
        public static async Task ConvertContent()
        {
            try
            {
                using DbContext cititesContext = new();
                await cititesContext.Cities.ExecuteDeleteAsync();

                string currentContent = await GetContent();

                string[] contentLines = currentContent.Split(new string[] { "\n" }, StringSplitOptions.None);

                foreach (string line in contentLines)
                {
                    string[] lineArray = line.Split(new string[] { "\t" }, StringSplitOptions.None);

                    if (lineArray[0] != "#loc_id" & !string.IsNullOrEmpty(lineArray[0]))
                    {
                        CityModel cityModel = new()
                        {
                            PLZ = lineArray[1],
                            Longitude = double.Parse(lineArray[2],NumberStyles.Any, CultureInfo.InvariantCulture),
                            Latitude = double.Parse(lineArray[3], NumberStyles.Any, CultureInfo.InvariantCulture),
                            Ort = lineArray[4],
                        };

                        cititesContext.Add(cityModel);    
                    }
                }

                cititesContext.SaveChanges();
            }
            catch (Exception ex)
            {
                UserMessage.ShowMessageBox("Verarbeitung Städte", $"Fehler beim Parsen: {ex.Message}");
            }
        }
    }
}