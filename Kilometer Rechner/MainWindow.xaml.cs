using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Kilometer_Rechner.Helper;

using Microsoft.EntityFrameworkCore;

namespace Kilometer_Rechner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CityDbContext _context = new CityDbContext();
        private CollectionViewSource citiesViewSource;
        
        public MainWindow()
        {
            InitializeComponent();

            citiesViewSource = (CollectionViewSource)FindResource(nameof(citiesViewSource));
            //CityDownload.ConvertContent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _context.Database.EnsureCreated();

            // load the entities into EF Core
            _context.Cities.Load();

            // bind to the source
            citiesViewSource.Source = _context.Cities.Local.ToObservableCollection();
        }

    }
}