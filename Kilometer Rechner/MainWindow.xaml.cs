using System.Data;
using System.Windows;
using System.Windows.Data;

using Kilometer_Rechner.Helper;
using Kilometer_Rechner.Models;

namespace Kilometer_Rechner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DbContext _DbContext = new();
        private CollectionViewSource calculationViewSource;

        public MainWindow()
        {
            InitializeComponent();

            calculationViewSource = (CollectionViewSource)FindResource(nameof(calculationViewSource));
        }

        /// <summary>
        /// Öffnen der Städe Verwaltung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickCities(object sender, RoutedEventArgs e)
        {
            CitiesView citiesView = new();
            citiesView.ShowDialog();
        }

        /// <summary>
        /// Laden der Berechnungen aus der DB in die DataGrid View
        /// </summary>
        private void CalculationLoadView()
        {
            _DbContext.Database.EnsureCreated();

            var dataObj = _DbContext.Caculations
                .Join(_DbContext.Cities, calc => calc.BasePlz, cities => cities.Id, (calc, cities) => new
                {
                    calc.CalcDate,
                    StartPlz = cities.PLZ,
                    StartOrt = cities.Ort,
                    calc.IdPlz,
                    calc.AirLineKm,
                    calc.RouteLineKm,
                }).Where(calcBase => calcBase.StartPlz == txtPostcodeFrom.Text)
                .Join(_DbContext.Cities, calc => calc.IdPlz, cities => cities.Id, (calc,cities) => new
                {
                    calc.CalcDate,
                    calc.StartPlz,
                    calc.StartOrt,
                    EndPlz = cities.PLZ,
                    EndOrt = cities.Ort,
                    calc.AirLineKm,
                    calc.RouteLineKm,                   
                })
                .ToList();

            calculationViewSource.Source = dataObj;
        }

        /// <summary>
        /// Reagieren auf Button Click Berechnung der Kilometer und speichern in Tabelle mit laden in die View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonClickCalcKm(object sender, RoutedEventArgs e)
        {
            buttonCalcKm.IsEnabled = false;
            buttonCitesShow.IsEnabled = false;

            if (!string.IsNullOrEmpty(txtPostcodeFrom.Text))
            {
                try
                {
                    CityModel cityFrom = _DbContext.Cities.Single(city => city.PLZ == txtPostcodeFrom.Text);

                    List<CityModel> cities = _DbContext.Cities.ToList();

                    CalcKilometer calcKilometer = new();
                    await Task.Run(() => calcKilometer.SaveToDB(cityFrom, cities, pbLoadCalc));

                    CalculationLoadView();
                }
                catch (Exception ex)
                {
                    UserMessage.ShowMessageBox("Berechnung", "Fehler beim berechnen der Kilometer:\n" + ex.Message);
                }
            }
            else
            {
                UserMessage.ShowMessageBox("Berechnung", "Es wurde keine Postleitzahl angegeben!");
            }

            buttonCalcKm.IsEnabled = true;
            buttonCitesShow.IsEnabled = true;
        }

        /// <summary>
        /// Reagieren auf verlassen des Textfeldes für die Postleitzahl -> Prüfen ob schon Ergebnisse vorhanden sind
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPostcodeFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculationLoadView();
        }

        /// <summary>
        /// Export des DataGrid zu Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickExportExcel(object sender, RoutedEventArgs e)
        {
            List<dynamic> listTable = dataGridCalc.Items.OfType<dynamic>().ToList();

            List<CalculationResult> list = listTable.Select(x => new CalculationResult
            {
                CalcDate = x.CalcDate,
                StartPlz = x.StartPlz,
                StartOrt = x.StartOrt,
                EndPlz = x.EndPlz,
                EndOrt = x.EndOrt,
                AirLineKm = x.AirLineKm,
                RouteLineKm = x.RouteLineKm
            }).ToList();

            DataTable dataTable = ExcelExport.ToDataTable(list);


        }
    }
}