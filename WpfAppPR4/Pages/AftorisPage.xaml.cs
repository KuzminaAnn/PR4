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
    /// Логика взаимодействия для AftorisPage.xaml
    /// </summary>
    public partial class AftorisPage : Page
    {
        //private static readonly HttpClient httpClient = new HttpClient();

        public AftorisPage()
        {
            InitializeComponent();
            //httpClient.BaseAddress = new Uri("http://localhost:5225");
        }


        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5225"),
            Timeout = TimeSpan.FromSeconds(10)
        };
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrPage());
        }

        private async void Aff_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text.Trim();
            string password = Pasword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                StatusTextBlock.Text = "Заполните все поля!";
                return;
            }

            Login.IsEnabled = false;
            StatusTextBlock.Text = "Проверка...";

            try
            {
                // Формируем URL с параметрами
                var requestUri = $"/api/auth/login?login={Uri.EscapeDataString(login)}&password={Uri.EscapeDataString(password)}";

                // Отправляем GET-запрос
                var response = await httpClient.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                // Парсим JSON через Newtonsoft.Json
                var result = JsonConvert.DeserializeObject<AuthResponseDto>(content);

                Login.IsEnabled = true;
                StatusTextBlock.Text = "";

                if (result != null && result.Success)
                {
                    MessageBox.Show($"Добро пожаловать, {result.UserName}!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new UserrPage());
                    App.Current.Properties["UserId"] = result.UserId;
                    App.Current.Properties["UserName"] = result.UserName;
                }
                else
                {
                    StatusTextBlock.Text = result?.Message ?? "Неизвестная ошибка";
                }
            }
            catch (Exception ex)
            {
                Login.IsEnabled = true;
                StatusTextBlock.Text = "Ошибка сети: " + ex.Message;
            }
        }
    }
}
