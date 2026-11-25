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
    /// Логика взаимодействия для CreateReview.xaml
    /// </summary>
    public partial class CreateReview : Page
    {
        private int _userId;
        private int _productId;
        private readonly ApiService _apiService;
        public CreateReview()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }
        public void SetReviewData(int userId, int productId)
        {
            _userId = userId;
            _productId = productId;

            // Запускаем загрузку названия товара
            LoadProductName();
        }

        private async void LoadProductName()
        {
            // Можно сделать отдельный запрос за названием товара или взять его из кэша.
            // Для простоты предположим, что у нас есть метод GetProductById в ApiService.
            // Если нет, можно просто написать "Товар #" + _productId
            Product product = await _apiService.GetProductByIdAsync(_productId);

            if (product != null)
            {
                ProductNameText.Text = $"Отзыв на игру: {product.Name}";
            }
            else
            {
                ProductNameText.Text = $"Отзыв на товар ID: {_productId}";
            }
        }

        private async void SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReviewText.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст отзыва.");
                return;
            }

            // Получаем оценку из ComboBox (индекс 0 = 5 звезд, индекс 4 = 1 звезда)
            // Или наоборот, зависит от того, как ты хочешь. Сейчас: SelectedIndex 4 -> 1/5
            int rating = 5 - RatingCombo.SelectedIndex;

            bool success = await _apiService.AddReviewAsync(_userId, _productId, ReviewText.Text, rating);

            if (success)
            {
                MessageBox.Show("Спасибо! Ваш отзыв успешно отправлен.");
                // Возвращаемся назад в историю
                if (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
            }
            else
            {
                MessageBox.Show("Произошла ошибка при отправке отзыва.");
            }
        }
    }
}
