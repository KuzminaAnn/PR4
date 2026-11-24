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
    /// Логика взаимодействия для RegistrPage.xaml
    /// </summary>
    public partial class RegistrPage : Page
    {
        public RegistrPage()
        {
            InitializeComponent();
        }
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5225"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private void Exet_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void Regist_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string passwordConfirm = PasswordConfirmBox.Password;

            if (password != passwordConfirm)
            {
                StatusTextBlock.Text = "Пароли не совпадают!";
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
            {
                StatusTextBlock.Text = "Заполните все поля!";
                return;
            }

            Regist.IsEnabled = false;
            StatusTextBlock.Text = "Регистрация...";

            try
            {
                var requestData = new
                {
                    Login = login,
                    Password = password,
                    Name = name,
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<AuthResponseDto>(responseContent);

                Regist.IsEnabled = true;
                StatusTextBlock.Text = "";

                if (result != null && result.Success)
                {
                    AffUser.CurrentUser = new Userr
                    {
                        Id = result.UserId,
                        Name = result.UserName,
                        Login = login,  // переменная login из поля ввода
                        Idrole = result.Idrole
                    };
                    MessageBox.Show($"Регистрация успешна! Добро пожаловать, {result.UserName}!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    App.Current.Properties["UserId"] = result.UserId;
                    App.Current.Properties["UserName"] = result.UserName;

                    // ✅ Вместо Close() — навигация на страницу пользователя
                    NavigationService.Navigate(new UserrPage());
                }
                else
                {
                    StatusTextBlock.Text = result?.Message ?? "Ошибка регистрации";
                }
            }
            catch (Exception ex)
            {
                Regist.IsEnabled = true;
                StatusTextBlock.Text = "Ошибка сети: " + ex.Message;
            }
        }
    }
}
