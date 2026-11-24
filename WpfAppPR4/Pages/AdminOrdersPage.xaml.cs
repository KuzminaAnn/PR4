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
    /// Логика взаимодействия для AdminOrdersPage.xaml
    /// </summary>
    public partial class AdminOrdersPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private List<OrderResponseDto> _orders = new List<OrderResponseDto>();
        private OrderResponseDto _selectedOrder;
        public class StatusDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class ChangeStatusDto
        {
            public int IdStatus { get; set; }
        }
        public AdminOrdersPage()
        {
            InitializeComponent();
            LoadOrders();
            LoadStatuses();
        }
        private async void LoadOrders()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/order");
                var content = await response.Content.ReadAsStringAsync();
                _orders = JsonConvert.DeserializeObject<List<OrderResponseDto>>(content) ?? new List<OrderResponseDto>();
                OrdersListBox.ItemsSource = _orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }

        }
        private async void LoadStatuses()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/order/statuses");
                var content = await response.Content.ReadAsStringAsync();
                // Десериализуем как анонимный тип через динамический список
                var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(content);
                StatusComboBox.ItemsSource = statuses;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статусов: " + ex.Message);
            }
        }

        private void OrdersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(OrdersListBox.SelectedItem is OrderResponseDto order)) return;

            _selectedOrder = order;

            if (order.Id.HasValue)
            {
                StatusComboBox.SelectedValue = order.IdStatus.Value;
            }
            else
            {
                StatusComboBox.SelectedIndex = -1;
            }

            DetailsPanel.Children.Clear();

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Заказ №{order.Id}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            DetailsPanel.Children.Add(new TextBlock { Text = $"Клиент: {order.UserName} (ID {order.UserId})" });
            DetailsPanel.Children.Add(new TextBlock { Text = $"Дата: {order.Date}" });
            DetailsPanel.Children.Add(new TextBlock { Text = $"Статус: {order.StatusName}" });
            DetailsPanel.Children.Add(new TextBlock { Text = $"Доставка: {order.DeliveryTypeName}" });

            if (!string.IsNullOrEmpty(order.PickupPointName))
                DetailsPanel.Children.Add(new TextBlock { Text = $"Пункт выдачи: {order.PickupPointName}" });

            if (!string.IsNullOrEmpty(order.Address))
                DetailsPanel.Children.Add(new TextBlock { Text = $"Адрес: {order.Address}" });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Итого: {order.TotalPrice}",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 10)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = "Товары:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            });

            foreach (var item in order.Items)
            {
                DetailsPanel.Children.Add(new TextBlock
                {
                    Text = $"• {item.ProductName} — {item.ProductCount} шт × {item.Price} = {item.SumPrice}",
                    Margin = new Thickness(0, 2, 0, 2)
                });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Сначала выберите заказ!");
                return;
            }

            if (!(StatusComboBox.SelectedValue is int newStatusId))
            {
                MessageBox.Show("Выберите новый статус!");
                return;
            }

            if (newStatusId == _selectedOrder.IdStatus)
            {
                MessageBox.Show("Этот статус уже установлен.");
                return;
            }

            try
            {
                var requestDto = new ChangeStatusDto { IdStatus = newStatusId };
                var json = JsonConvert.SerializeObject(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"/api/order/{_selectedOrder.Id}/status", content);
                var result = JsonConvert.DeserializeObject<OrderResponseDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    MessageBox.Show("Статус успешно изменён!");
                    LoadOrders(); // перезагружаем список
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Ошибка смены статуса");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }
    }

}
