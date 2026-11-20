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
using WpfAppPR4.Models;
using WpfAppPR4.Services;

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserrPage.xaml
    /// </summary>
    public partial class UserrPage : Page
    {
        private readonly ApiService _apiService;
        public UserrPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += async (s, e) => await LoadUserReviews();

            if (AffUser.CurrentUser != null)
            {
                Name.Text = $"Здравствуй, {AffUser.CurrentUser.Name}!";
                name.Text = AffUser.CurrentUser.Name.ToString();
                login.Text = AffUser.CurrentUser.Login.ToString();
            }
        }
        private async Task LoadUserReviews()
        {
            if (AffUser.CurrentUser == null) return;
            int userId = AffUser.CurrentUser.Id;
            var reviews = await _apiService.GetUserReviewsAsync(userId);
            Review.ItemsSource = reviews;
        }

        private void basket_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }

        private void Out_Click(object sender, RoutedEventArgs e)
        {
            AffUser.CurrentUser = null;
            NavigationService.Navigate(new StartPage());

        }
    }
}
