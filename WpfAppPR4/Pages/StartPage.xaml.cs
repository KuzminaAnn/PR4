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
using WpfAppPR4.Models;

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
            Loaded += async (s, e) => await LoadProducts();
            if (AffUser.CurrentUser == null)
            {
                Aff.Visibility = Visibility.Visible;
                Acc.Visibility = Visibility.Collapsed;
            }
            else {
                Aff.Visibility = Visibility.Collapsed;
                Acc.Visibility = Visibility.Visible;
            }
        }
        private async Task LoadProducts()
        {
            try
            {
                var products = await _apiService.GetProductsAsync();
                Box.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
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
            Button btn = sender as Button;
            var selectedGame = btn?.DataContext as Product;

            if (selectedGame != null)
            {
                var detailsWindow = new GamePage();

                detailsWindow.SetProduct(selectedGame);

                detailsWindow.NavigationService.Navigate(new GamePage());
            }
        }

        private void Acc_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserrPage());
        }
    }
}
