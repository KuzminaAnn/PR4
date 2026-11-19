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

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();           
        }
        public void SetProduct(Product product)
        {
            if (product == null) return;
            
            TitleText.Text = product.Name;
            DescText.Text = product.Description;
            PriceText.Text = $"{product.Price} руб.";
            RatingText.Text = product.Rating.ToString("F1");

            // Обработка картинки (пока просто путь, если он корректный)
            if (!string.IsNullOrEmpty(product.Image) && product.Image != "\\\\")
            {
                // Если картинка лежит локально в папке Images проекта:
                // GameImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{System.IO.Path.GetFileName(product.Image)}"));

                // Или если это полный путь/URL:
                try
                {
                    GameImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(product.Image, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }
        }

  
    }
}
