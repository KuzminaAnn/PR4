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
    /// Логика взаимодействия для AdminCategoriesPage.xaml
    /// </summary>
    public partial class AdminCategoriesPage : Page
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5225") };
        private List<CategoryDto> _categories = new List<CategoryDto>();
        public AdminCategoriesPage()
        {
            InitializeComponent();
            LoadCategories();
        }
        private async void LoadCategories()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/category");
                var content = await response.Content.ReadAsStringAsync();
                _categories = JsonConvert.DeserializeObject<List<CategoryDto>>(content) ?? new List<CategoryDto>();
                CategoriesListBox.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesListBox.SelectedItem is CategoryDto selected)
                CategoryNameBox.Text = selected.Name;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = CategoryNameBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название категории!");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(new CategoryDto { Name = name });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/category", content);
                var result = JsonConvert.DeserializeObject<CategoryDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    CategoryNameBox.Clear();
                    LoadCategories();
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

        private async void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(CategoriesListBox.SelectedItem is CategoryDto selected))
            {
                MessageBox.Show("Выберите категорию для переименования!");
                return;
            }

            string newName = CategoryNameBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Введите новое название!");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(new CategoryDto { Name = newName });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"/api/category/{selected.Id}", content);
                var result = JsonConvert.DeserializeObject<CategoryDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    CategoryNameBox.Clear();
                    LoadCategories();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Ошибка переименования");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(CategoriesListBox.SelectedItem is CategoryDto selected))
            {
                MessageBox.Show("Выберите категорию для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить категорию '{selected.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                var response = await httpClient.DeleteAsync($"/api/category/{selected.Id}");
                var result = JsonConvert.DeserializeObject<CategoryDto>(await response.Content.ReadAsStringAsync());

                if (result != null && result.Success)
                {
                    LoadCategories();
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
    }
}
