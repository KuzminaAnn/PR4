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
    /// Логика взаимодействия для UserrPage.xaml
    /// </summary>
    public partial class UserrPage : Page
    {
        public UserrPage()
        {
            InitializeComponent();

            if (AffUser.CurrentUser != null)
            {
                Name.Text = "Привет, " + AffUser.CurrentUser.Login + "!";
            }
        }

    }
}
