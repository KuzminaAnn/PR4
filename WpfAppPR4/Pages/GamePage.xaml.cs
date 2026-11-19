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
            GameImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(product.Image, UriKind.RelativeOrAbsolute));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }

        private void Love_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
