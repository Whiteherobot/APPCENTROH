using APPCENTROH.Repositories;
using APPCENTROH.Views;
using APPCENTROM.ViewModels;
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
using System.Xml.Linq;
using static APPCENTROH.View.RegisterView;

namespace APPCENTROM.Views
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        private RegisterViewModel registerViewModel;

        public UserView(RegisterViewModel data)
        {
            InitializeComponent();
            registerViewModel = data;
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
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
            string.IsNullOrWhiteSpace(txtPass.Password) ||
            string.IsNullOrWhiteSpace(txtPassConfirm.Password))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtPass.Password != txtPassConfirm.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserRepository userRepository = new UserRepository();
            userRepository.InsertData(registerViewModel, txtUser.Text, txtPass.Password);

            // Mostrar mensaje de éxito y cerrar las ventanas
            MessageBox.Show("Registro exitoso", "Éxito");

            LoginView loginView = new LoginView();


            loginView.Show();

            this.Close();
        }

        private void TextBlock_MouseDown(object sender, RoutedEventArgs e)
        {
            // LoginView loginView = new LoginView();

            // loginView.Show();

            // this.Close();
        }
    }
}
