using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
        private int _currentUserId;
        public UserrPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += async (s, e) => await LoadUserReviews();
            _currentUserId = AffUser.CurrentUser.Id;
            if (_apiService._httpClient != null)
            {
                _apiService._httpClient.BaseAddress = new Uri("http://localhost:5225");
            }

            LoadFavorites();

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
        private async void LoadFavorites()
        {
            try
            {
                // Передаем userId в метод API
                var favorites = await _apiService.GetFavoriteGamesAsync(_currentUserId);
                LOVE.ItemsSource = favorites;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async void delete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var game = button.CommandParameter as FavoriteGame;

            // ПРОВЕРКА: Убедитесь, что у нас есть и ID товара, и ID пользователя
            if (game == null || game.Id <= 0)
            {
                MessageBox.Show("Ошибка: Неверные данные товара.");
                return;
            }

            if (_currentUserId <= 0)
            {
                MessageBox.Show("Ошибка: Пользователь не авторизован (ID = 0).");
                return;
            }

            try
            {
                bool isDeleted = await _apiService.DeleteFavoriteAsync(_currentUserId, game.Id);

                if (isDeleted)
                {
                    var list =  LOVE.ItemsSource as List<FavoriteGame>;
                    if (list != null)
                    {
                        list.Remove(game);
                        LOVE.ItemsSource = null;
                        LOVE.ItemsSource = list;
                    }
                    MessageBox.Show("Товар убран из избранного.");
                }
                else
                {
                    MessageBox.Show("Не удалось удалить товар. Проверьте консоль вывода (Output) для деталей.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}");
            }
        }
    }
}
