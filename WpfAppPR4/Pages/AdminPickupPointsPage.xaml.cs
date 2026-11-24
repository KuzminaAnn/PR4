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

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPickupPointsPage.xaml
    /// </summary>
    public partial class AdminPickupPointsPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private List<PickupPointDto> _points = new List<PickupPointDto>();
        public AdminPickupPointsPage()
        {
            InitializeComponent();
            LoadPickupPoints();
        }
        private async void LoadPickupPoints()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/pickuppoint");
                var content = await response.Content.ReadAsStringAsync();
                _points = JsonConvert.DeserializeObject<List<PickupPointDto>>(content) ?? new List<PickupPointDto>();
                PickupPointsListBox.ItemsSource = _points;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void PickupPointsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PickupPointsListBox.SelectedItem is PickupPointDto selected)
            {
                NameBox.Text = selected.Name;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Заполните название и адрес!");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(new PickupPointDto { Name = name});
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/pickuppoint", content);
                var result = JsonConvert.DeserializeObject<PickupPointDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    NameBox.Clear();
                    LoadPickupPoints();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Ошибка добавления");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(PickupPointsListBox.SelectedItem is PickupPointDto selected))
            {
                MessageBox.Show("Выберите пункт выдачи для редактирования!");
                return;
            }

            string name = NameBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Заполните название и адрес!");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(new PickupPointDto { Name = name });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"/api/pickuppoint/{selected.Id}", content);
                var result = JsonConvert.DeserializeObject<PickupPointDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    NameBox.Clear();
                    LoadPickupPoints();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Ошибка редактирования");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(PickupPointsListBox.SelectedItem is PickupPointDto selected))
            {
                MessageBox.Show("Выберите пункт выдачи для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить пункт выдачи '{selected.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                var response = await httpClient.DeleteAsync($"/api/pickuppoint/{selected.Id}");
                var result = JsonConvert.DeserializeObject<PickupPointDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    NameBox.Clear();
                    LoadPickupPoints();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
