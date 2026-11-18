using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для Hash.xaml
    /// </summary>
    public partial class Hash : Page
    {
        public Hash()
        {
            InitializeComponent();
            
        }
        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x =>
                x.ToString("X2")));
            }
        }

        private void Ddd_Click(object sender, RoutedEventArgs e)
        {
            Has.Clear();
            string hashedPassword = GetHash(Pas.Text);
            Has.Text = hashedPassword;
        }
    }
}
