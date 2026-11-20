using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using WpfAppPR4.Models;
using WpfAppPR4.Services;

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private List<ProductResponseDto> _products = new List<ProductResponseDto>();
        public AdminPage()
        {
            InitializeComponent();
            LoadProducts();
        }
        private async void LoadProducts()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/product");
                var content = await response.Content.ReadAsStringAsync();
                _products = JsonConvert.DeserializeObject<List<ProductResponseDto>>(content) ?? new List<ProductResponseDto>();
                ProductsDataGrid.ItemsSource = _products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // null означает режим создания нового товара
            NavigationService.Navigate(new EditProductPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductResponseDto selected)
            {
                NavigationService.Navigate(new EditProductPage(selected.Id));
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductResponseDto selected)
            {
                var result = MessageBox.Show($"Удалить товар '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await httpClient.DeleteAsync($"/api/product/{selected.Id}");
                        var content = await response.Content.ReadAsStringAsync();
                        var dto = JsonConvert.DeserializeObject<ProductResponseDto>(content);

                        if (dto != null && dto.Success)
                        {
                            MessageBox.Show("Товар удалён");
                            LoadProducts(); // Перезагружаем список
                        }
                        else
                        {
                            MessageBox.Show(dto?.Message ?? "Ошибка удаления");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сети: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCategoriesPage());
        }
    }
}
