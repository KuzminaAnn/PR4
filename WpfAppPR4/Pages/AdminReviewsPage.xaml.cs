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
    /// Логика взаимодействия для AdminReviewsPage.xaml
    /// </summary>
    public partial class AdminReviewsPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private List<ReviewResponseDto> _reviews = new List<ReviewResponseDto>();
        public AdminReviewsPage()
        {
            InitializeComponent();
            LoadReviews();
        }
        private async void LoadReviews()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/review");
                var content = await response.Content.ReadAsStringAsync();
                _reviews = JsonConvert.DeserializeObject<List<ReviewResponseDto>>(content) ?? new List<ReviewResponseDto>();
                ReviewsListBox.ItemsSource = _reviews;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ReviewsListBox.SelectedItem is ReviewResponseDto selected))
            {
                MessageBox.Show("Выберите отзыв для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить отзыв пользователя '{selected.UserName}'?",
                "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                var response = await httpClient.DeleteAsync($"/api/review/{selected.Id}");
                var result = JsonConvert.DeserializeObject<ReviewResponseDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    LoadReviews();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Ошибка удаления");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadReviews();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
