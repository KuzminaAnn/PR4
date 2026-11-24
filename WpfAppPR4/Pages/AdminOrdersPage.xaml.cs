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
        public AdminOrdersPage()
        {
            InitializeComponent();
            LoadOrders();
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

        private void OrdersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(OrdersListBox.SelectedItem is OrderResponseDto order)) return;

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
    }
}
