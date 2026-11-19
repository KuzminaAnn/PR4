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
    /// Логика взаимодействия для EditProductPage.xaml
    /// </summary>
    public partial class EditProductPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private readonly int? _productId;
        private ProductResponseDto _originalProduct;

        public EditProductPage(int? productId)
        {
            InitializeComponent();
            _productId = productId;
            TitleText.Text = productId.HasValue ? "Редактирование товара" : "Добавление нового товара";
            InitializeData();
        }
        private async void InitializeData()
        {
            await LoadCategories();
            if (_productId.HasValue)
                await LoadProduct(_productId.Value);
        }

        private async System.Threading.Tasks.Task LoadCategories()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/category");
                var content = await response.Content.ReadAsStringAsync();
                CategoryBox.ItemsSource = JsonConvert.DeserializeObject<List<CategoryDto>>(content);
            }
            catch { }
        }

        private async System.Threading.Tasks.Task LoadProduct(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"/api/product/{id}");
                var content = await response.Content.ReadAsStringAsync();
                _originalProduct = JsonConvert.DeserializeObject<ProductResponseDto>(content);

                if(_originalProduct != null && _originalProduct.Success)
{
                    NameBox.Text = _originalProduct.Name;
                    DescriptionBox.Text = _originalProduct.Description;
                    CategoryBox.SelectedValue = _originalProduct.IdCategory;
                    ImageBox.Text = _originalProduct.Image;
                    PriceBox.Text = _originalProduct.Price?.ToString() ?? "0";
                    RatingBox.Text = _originalProduct.Rating?.ToString() ?? "0";
                    QuantityBox.Text = _originalProduct.Quantity?.ToString() ?? "0";
                    CountBox.Text = _originalProduct.ProductCount?.ToString() ?? "0";
                    IsOverBox.IsChecked = _originalProduct.IsOver ?? false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Название обязательно!");
                return;
            }

            var newDto = new ProductRequestDto
            {
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                IdCategory = CategoryBox.SelectedValue != null ? Convert.ToInt32(CategoryBox.SelectedValue) : 1,
                Image = ImageBox.Text.Trim(),
                Price = int.TryParse(PriceBox.Text, out int p) ? p : 0,
                Rating = float.TryParse(RatingBox.Text, out float r) ? r : 0,
                Quantity = int.TryParse(QuantityBox.Text, out int q) ? q : 0,
                ProductCount = int.TryParse(CountBox.Text, out int c) ? c : 0,
                IsOver = IsOverBox.IsChecked ?? false
            };

            // ✅ Сравнение со старыми данными (только в режиме редактирования)
            if (_productId.HasValue && _originalProduct != null)
            {
                bool isChanged =
                    _originalProduct.Name != newDto.Name ||
                    _originalProduct.Description != newDto.Description ||
                    _originalProduct.IdCategory != newDto.IdCategory ||
                    _originalProduct.Image != newDto.Image ||
                    _originalProduct.Price != newDto.Price ||
                    _originalProduct.Rating != newDto.Rating ||
                    _originalProduct.Quantity != newDto.Quantity ||
                    _originalProduct.ProductCount != newDto.ProductCount ||
                    _originalProduct.IsOver != newDto.IsOver;

                if (!isChanged)
                {
                    MessageBox.Show("Изменений не обнаружено.", "Информация");
                    NavigationService?.GoBack();
                    return;
                }
            }

            try
            {
                var json = JsonConvert.SerializeObject(newDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _productId.HasValue
                    ? await httpClient.PutAsync($"/api/product/{_productId}", content)
                    : await httpClient.PostAsync("/api/product", content);

                var result = JsonConvert.DeserializeObject<ProductResponseDto>(
                    await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    MessageBox.Show("Успешно сохранено!");
                    NavigationService?.GoBack();
                }
                else
                {
                    MessageBox.Show("Ошибка: " + (result?.Message ?? "неизвестная"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
