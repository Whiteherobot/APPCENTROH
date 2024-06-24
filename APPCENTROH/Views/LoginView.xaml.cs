using APPCENTROH.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using APPCENTROH.Repositories;
using APPCENTROH.Models;

namespace APPCENTROH.Views
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e) 
        {
            NetworkCredential credential = new NetworkCredential(txtUser.Text, txtPassx.Password);

            UserRepository userRepository = new UserRepository();


            // Autenticar al usuario
            bool isAuthenticated = userRepository.AuthenticateUser(credential);

            if (isAuthenticated)
            {
                // Obtener detalles del usuario
                UserModel user = userRepository.GetByUsername(txtUser.Text);

                if (user != null)
                {
                    // Si se encuentra el usuario, abrir la ventana principal
                    var mainView = new MainView(user);
                    mainView.Show();

                    // Cerrar la ventana de inicio de sesión
                    this.Close();
                }
                else
                {
                    // Si no se encuentra el usuario, mostrar un mensaje de error
                    MessageBox.Show("Error al obtener los detalles del usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Si la autenticación falla, mostrar un mensaje de error al usuario
                MessageBox.Show("Usuario o contraseña incorrectos. Por favor, inténtelo de nuevo.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnRegisterClient_Click(object sender, RoutedEventArgs e)
        {
            RegisterView registerclientView = new RegisterView();

            registerclientView.Show();

            this.Close();
        }

        private void TextBlock_MouseDown(object sender, RoutedEventArgs e)
        {

        }

        private void BindablePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
