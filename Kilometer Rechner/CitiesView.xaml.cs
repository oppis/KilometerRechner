using System.Windows;
using System.Windows.Data;

using Kilometer_Rechner.Helper;

using Microsoft.EntityFrameworkCore;

namespace Kilometer_Rechner
{
    /// <summary>
    /// Interaktionslogik für Cities.xaml
    /// </summary>
    public partial class CitiesView : Window
    {
        private readonly CityDbContext _citiesDbContext = new();
        private CollectionViewSource citiesViewSource;

        public CitiesView()
        {
            InitializeComponent();

            citiesViewSource = (CollectionViewSource)FindResource(nameof(citiesViewSource));
        }

        /// <summary>
        /// Ragieren auf laden des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CityLoadView();
        }

        /// <summary>
        /// Starten des Herunterladen nach Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CityDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            pbLoadCitites.IsIndeterminate = true;
            buttonLoadCities.IsEnabled = false;
            citiesViewSource.Source = null;

            await Task.Run(() => CityDownload.ConvertContent());
            CityLoadView();
            
            pbLoadCitites.IsIndeterminate = false;
            buttonLoadCities.IsEnabled = true;
        }

        /// <summary>
        /// Laden der Städte aus der DB in die DataGrid View
        /// </summary>
        private void CityLoadView()
        {
            _citiesDbContext.Database.EnsureCreated();
            _citiesDbContext.Cities.Load();
            citiesViewSource.Source = _citiesDbContext.Cities.Local.ToObservableCollection();
        }
    }
}