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
using WpfAppPR4.Models;

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для AftorisPage.xaml
    /// </summary>
    public partial class AftorisPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public AftorisPage()
        {
            InitializeComponent();
            httpClient.BaseAddress = new Uri("http://localhost:5225");
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrPage());
        }

        private async void Aff_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text.Trim();
            string password = Pasword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            // Формируем JSON
            var json = "{\"Login\":\"" + login + "\",\"Password\":\"" + password + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // УСПЕХ! Просто сохраняем логин в глобальную переменную
                    AffUser.CurrentUser = new Userr
                    {
                        Login = login
                    };
                    MessageBox.Show("Вы вошли в аккаунт");
                    // Переходим дальше
                    NavigationService.Navigate(new UserrPage());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к API");
            }

        }
    }
}
