using System.Data;
using System.Reflection;
using Microsoft.Win32;

using ClosedXML.Excel;

namespace Kilometer_Rechner.Helper
{
    class ExcelExport
    {
        /// <summary>
        /// Liste zu einer DataTable umwandeln
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    //inserting property values to data table rows
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check data table
            return dataTable;
        }

        /// <summary>
        /// DataTable als Excel File exportieren
        /// </summary>
        /// <param name="dataTable"></param>
        public static async void SaveFile(DataTable dataTable, System.Windows.Controls.ProgressBar pbLoadCalc)
        {
            //Abfrage für Dateinamen und Speicherort
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Datei Speicher Excel Export Kilometer",
                Filter = "Excel (*.xlsx) | *.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                pbLoadCalc.Dispatcher.Invoke(() => pbLoadCalc.IsIndeterminate = true);

                XLWorkbook xLWorkbook = new();
                xLWorkbook.Worksheets.Add(dataTable, "Kilometer");

                await Task.Run(() => xLWorkbook.SaveAs(saveFileDialog.FileName));

                pbLoadCalc.Dispatcher.Invoke(() => pbLoadCalc.IsIndeterminate = false);

                UserMessage.ShowMessageBoxInfo("Excel Export", "Die Excel Datei wurde hier gespeichert: \n" + saveFileDialog.FileName);
            }
        }
    }
} 