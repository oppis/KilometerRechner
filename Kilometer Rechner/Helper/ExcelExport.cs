using System.Data;
using System.Drawing;
using System.Reflection;

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
    } 
}

public class ExcelUtlity
{
    public bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string ReporType)
    {
        Application excel;
        Workbook excelworkBook;
        Worksheet excelSheet;
        Range excelCellrange;
        try
        {
            excel = new Application();


            excel.Visible = false;
            excel.DisplayAlerts = false;
            excelworkBook = excel.Workbooks.Add(Type.Missing);
            excelSheet = (Worksheet)excelworkBook.ActiveSheet;
            excelSheet.Name = worksheetName;




            //excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();  
            int rowcount = 2;
            for (int Idx = 0; Idx < dataTable.Columns.Count; Idx++)
            {
                excelSheet.Range["A1"].Offset[0, Idx].Value = dataTable.Columns[Idx].ColumnName;
                //row header styles  
                excelSheet.Range["A1"].Offset[0, Idx].Font.Size = 14;
                excelSheet.Range["A1"].Offset[0, Idx].Font.Name = "Arial";
                excelSheet.Range["A1"].Offset[0, Idx].Font.FontStyle = "Bold";
                excelSheet.Range["A1"].Offset[0, Idx].Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
                excelSheet.Range["A1"].Offset[0, Idx].Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.Gray);




            }
            for (int Idx = 0; Idx < dataTable.Rows.Count; Idx++)
            {
                excelSheet.Range["A2"].Offset[Idx].Resize[1, dataTable.Columns.Count].Value =
                dataTable.Rows[Idx].ItemArray;
            }


            excelSheet.Activate();
            excelSheet.Application.ActiveWindow.FreezePanes = true;
            Microsoft.Office.Interop.Excel.Range firstRow = (Microsoft.Office.Interop.Excel.Range)excelSheet.Rows[1];


            firstRow.AutoFilter(1,
            Type.Missing,
            Microsoft.Office.Interop.Excel.XlAutoFilterOperator.xlAnd,
            Type.Missing,
            true);


            excelSheet.Columns.AutoFit();


            excelworkBook.SaveAs(saveAsLocation); ;
            excelworkBook.Close();
            excel.Quit();
            return true;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message);
            return false;
        }
        finally
        {
            excelSheet = null;
            excelCellrange = null;
            excelworkBook = null;
        }


    }
}