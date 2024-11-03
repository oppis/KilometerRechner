using System.Windows;
using System.Windows.Data;

using Kilometer_Rechner.Helper;
using Kilometer_Rechner.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

            CalculationLoadView();
        }

        /// <summary>
        /// Ragieren auf laden des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Öffnen der Städe Verwaltung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CityButton_Click(object sender, RoutedEventArgs e)
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
            _DbContext.Caculations.Load();
            calculationViewSource.Source = _DbContext.Caculations.Local.ToObservableCollection();
        }

        private async void ButtonCalcKm_Click(object sender, RoutedEventArgs e)
        {
            pbLoadCalc.IsIndeterminate = true;
            buttonCalcKm.IsEnabled = false;

            if (!string.IsNullOrEmpty(txtPostcodeFrom.Text))
            {
                try
                {
                    CityModel cityFrom = _DbContext.Cities.Single(city => city.PLZ == txtPostcodeFrom.Text);

                    List<CityModel> cities = _DbContext.Cities.ToList();

                    await Task.Run(() => CalcKilometer.CalcAirLineToDB(cityFrom, cities));

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

            pbLoadCalc.IsIndeterminate = false;
            buttonCalcKm.IsEnabled = true;
        }
    }
}