using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppPR4.Services;

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private readonly ApiService _apiService;
        public StartPage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            // Загружаем данные при старте
            Loaded += async (s, e) => await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var products = await _apiService.GetProductsAsync();
            Box.ItemsSource = products;
        }
        private void Aff_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AftorisPage());
        }
        private void Sort_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
        }
        private void Poisc_TextChanged(object sender, RoutedEventArgs e)
        {

        }
        private void Filtr_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
        private void Podrob_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
