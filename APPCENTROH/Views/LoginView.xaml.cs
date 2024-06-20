using APPVETERINARIA.View;
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
using WPF_LoginForm.Models;

namespace APPVETERINARIA.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e) { }

        private void BtnRegisterClient_Click(object sender, RoutedEventArgs e)
        {
            RegisterClientView registerclientView = new RegisterClientView();

            registerclientView.Show();

            this.Close();
        }

        private void TextBlock_MouseDown(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRegisterEmploye_Click(object sender, RoutedEventArgs e)
        {
            RegisterEmployeView registeremployeView = new RegisterEmployeView();

            registeremployeView.Show();

            this.Close();
        }
    }
}
