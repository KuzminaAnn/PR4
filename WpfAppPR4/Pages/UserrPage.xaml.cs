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
        private List<HistoryOrder> _allOrders = new List<HistoryOrder>();
        public UserrPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += async (s, e) => await LoadUserReviews();
            Loaded += async (s, e) => await LoadOrderHistory();
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

        private async void AddToBasket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка авторизации
                if (AffUser.CurrentUser == null)
                {
                    MessageBox.Show("Пожалуйста, войдите в аккаунт!");
                    return;
                }

                // Получаем объект FavoriteGame из DataContext кнопки
                if (sender is Button btn && btn.DataContext is FavoriteGame favGame)
                {
                    System.Diagnostics.Debug.WriteLine($"Добавляем игру из избранного: ID={favGame.Id}, Name={favGame.Name}");

                    // Вызываем API. 
                    // ВАЖНО: Убедись, что у favGame есть свойства IdProduct и Price.
                    // Если свойство цены называется иначе (например, GamePrice), поправь ниже.
                    bool success = await _apiService.AddToBasketAsync(
                        AffUser.CurrentUser.Id,
                        favGame.Id,   // Берем ID продукта из объекта избранного
                        favGame.Price        // Берем цену (проверь название свойства!)
                    );

                    if (success)
                    {
                        MessageBox.Show($"Игра \"{favGame.Name}\" добавлена в корзину!");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить товар. Проверьте Output");
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка: не удалось получить данные об игре.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
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

        private void Basket_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BasketPage());
        }

        private void Podrob_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selectedGame = btn?.DataContext as Product;

            if (selectedGame != null)
            {
                var page = new WpfAppPR4.Pages.GamePage();
                page.SetProduct(selectedGame);
                this.NavigationService.Navigate(page);
            }
        }
        private async void UserProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrderHistory();
        }

        private async Task LoadOrderHistory()
        {
            if (AffUser.CurrentUser == null) return;

            try
            {
                var api = new ApiService();
                var data = await api.GetUserOrdersAsync(AffUser.CurrentUser.Id);

                // Если API вернул null, создаем пустой список
                _allOrders = data ?? new List<HistoryOrder>();

                ApplySorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void ApplySorting()
        {
            // 1. Проверка: существует ли ComboBox Sort?
            if (Sort == null) return;

            // 2. Проверка: есть ли данные?
            if (_allOrders == null || !_allOrders.Any())
            {
                his.ItemsSource = null;
                return;
            }

            IEnumerable<HistoryOrder> sortedOrders;

            // 3. Безопасная сортировка
            // Проверяем SelectedIndex, чтобы избежать ошибок если ничего не выбрано
            if (Sort.SelectedIndex == 1)
            {
                // Сортируем от старых к новым. 
                // Добавляем проверку на null внутри даты, если вдруг она придет пустой
                sortedOrders = _allOrders.OrderBy(o => o.Data).ToList();
            }
            else
            {
                // По умолчанию (или индекс 0): от новых к старым
                sortedOrders = _allOrders.OrderByDescending(o => o.Data).ToList();
            }

            his.ItemsSource = sortedOrders;
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Вызываем сортировку только если страница уже загружена и данные есть
            if (_allOrders != null && _allOrders.Any())
            {
                ApplySorting();
            }
        }
        private void LeaveReview_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что кнопка существует и в Tag лежит ID продукта
            if (sender is Button btn && btn.Tag is int productId)
            {
                if (AffUser.CurrentUser == null)
                {
                    MessageBox.Show("Для оставления отзыва необходимо войти в систему.");
                    return;
                }

                // Создаем страницу отзыва
                var reviewPage = new WpfAppPR4.Pages.CreateReview();

                // Передаем данные: ID пользователя и ID товара
                reviewPage.SetReviewData(AffUser.CurrentUser.Id, productId);

                // Переходим на страницу (предполагаем, что ты используешь NavigationService)
                this.NavigationService.Navigate(reviewPage);
            }
        }
    }
}
