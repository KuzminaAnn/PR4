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
        private List<Product> _originalProducts = new List<Product>();
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
            LoadData();
        }
        private async Task LoadProducts()
        {        
            try
            {
                var products = await _apiService.GetProductsAsync();
                Box.ItemsSource = products;
                _originalProducts = products;
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
        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_originalProducts == null || !_originalProducts.Any()) return;

            // Получаем выбранный текст из ComboBox
            var selectedOption = (Sort.SelectedItem as ComboBoxItem)?.Content?.ToString();

            List<Product> sortedList = new List<Product>(_originalProducts); // Создаем копию для сортировки

            switch (selectedOption)
            {
                case "Цена: по возрастанию":
                    sortedList = sortedList.OrderBy(p => p.Price).ToList();
                    Box.ItemsSource = sortedList;
                    break;

                case "Цена: по убыванию":
                    sortedList = sortedList.OrderByDescending(p => p.Price).ToList();
                    Box.ItemsSource = sortedList;
                    break;

                case "По алфавиту (А-Я)":
                    sortedList = sortedList.OrderBy(p => p.Name).ToList();
                    Box.ItemsSource = sortedList;
                    break;

                case "По алфавиту (Я-А)":
                    sortedList = sortedList.OrderByDescending(p => p.Name).ToList();
                    Box.ItemsSource = sortedList;
                    break;

                default: // "По умолчанию" или null
                    sortedList = new List<Product>(_originalProducts);
                    break;
            }

            // Обновляем ListBox новым отсортированным списком
            Box.ItemsSource = sortedList;
        }
        private void Poisc_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Если список еще не загружен, ничего не делаем
            if (_originalProducts == null) return;

            string searchText = Poisc.Text.ToLower(); // Приводим к нижнему регистру для удобства

            List<Product> filteredList;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Если поле пустое, показываем весь список (или текущую отсортированную версию)
                filteredList = new List<Product>(_originalProducts);
            }
            else
            {
                // Ищем совпадения по Названию
                filteredList = _originalProducts.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(searchText))
                ).ToList();
            }

            // Обновляем ListBox
            Box.ItemsSource = filteredList;
        }
        private void Filtr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_originalProducts == null) return;

            var selectedCategory = Filtr.SelectedItem as Category;

            List<Product> filteredList;

            // Если выбрано "Все категории" (Id == 0) или ничего не выбрано
            if (selectedCategory == null || selectedCategory.Id == 0)
            {
                filteredList = new List<Product>(_originalProducts);
            }
            else
            {
                // Фильтруем по ID категории
                filteredList = _originalProducts
                    .Where(p => p.Idcategory == selectedCategory.Id)
                    .ToList();
            }

            Box.ItemsSource = filteredList;
        }
        private void Podrob_Click(object sender, RoutedEventArgs e)
        {
            if (AffUser.CurrentUser == null)
            {
                MessageBox.Show("Ага, захотели они товар посмотреть. Авторизуйся или регистрируйся сначала!");
                NavigationService.Navigate(new AftorisPage());
            }
            else
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
            
        }

        private async Task LoadData()
        {
            // 1. Загружаем данные
            _originalProducts = await _apiService.GetProductsAsync();
            var categoriesFromApi = await _apiService.GetCategoriesAsync();

            System.Diagnostics.Debug.WriteLine($"Категорий пришло из API: {categoriesFromApi.Count}");

            // 2. Очищаем комбобокс перед заполнением
            Filtr.Items.Clear();

            // 3. Добавляем пункт "Все категории" первым
            Filtr.Items.Add(new Category { Id = 0, Name = "Все категории" });

            // 4. Добавляем остальные категории вручную
            foreach (var cat in categoriesFromApi)
            {
                Filtr.Items.Add(cat);
            }

            // 5. Выбираем первый элемент по умолчанию
            if (Filtr.Items.Count > 0)
            {
                Filtr.SelectedIndex = 0;
            }

            // 6. Показываем товары
            Box.ItemsSource = _originalProducts;

            System.Diagnostics.Debug.WriteLine($"В ComboBox теперь {Filtr.Items.Count} элементов.");
        }
        private void Acc_Click(object sender, RoutedEventArgs e)
        {
            if (AffUser.CurrentUser.Idrole == 1)
            {
                NavigationService.Navigate(new AdminPage());
            }
            else if (AffUser.CurrentUser.Idrole == 2)
            {
                NavigationService.Navigate(new UserrPage());
            }            
        }
    }
}
