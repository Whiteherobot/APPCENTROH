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
using System.Windows.Shapes;
using APPVETERINARIA.Views;

namespace APPVETERINARIA.View
{
    /// <summary>
    /// Lógica de interacción para RegisterEmployeView.xaml
    /// </summary>
    public partial class RegisterEmployeView : Window
    {
        public RegisterEmployeView()
        {
            InitializeComponent();
        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_MouseDown(object sender, RoutedEventArgs e)
        {
            LoginView loginView = new LoginView();

            loginView.Show();

            this.Close();
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
