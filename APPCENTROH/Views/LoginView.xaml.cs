using APPCENTROH.Models;
using APPCENTROH.Repositories;
using APPCENTROH.View;
using APPCENTROM.Views;
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

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

            // Autenticar al usuario y obtener el rol y estado activo
            string role;
            bool isActive;
            bool isAuthenticated = userRepository.AuthenticateUser(credential, out role, out isActive);

            if (isAuthenticated)
            {
                if (!isActive)
                {
                    // Si el usuario está inactivo, mostrar un mensaje específico
                    MessageBox.Show("El usuario está inactivo. Por favor, contacte al administrador.", "Usuario Inactivo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                UserModel user = userRepository.GetByUsername(txtUser.Text);

                if (!string.IsNullOrEmpty(role))
                {
                    // Abrir la vista correspondiente según el rol
                    if (string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
                    {
                        var mainView = new MainView(user); // Pasa el objeto user si es necesario
                        mainView.Show();
                    }
                    else if (string.Equals(role, "Empleado", StringComparison.OrdinalIgnoreCase))
                    {


                    }

                    // Cerrar la ventana de inicio de sesión
                    this.Close();
                }
                else
                {
                    // Si no se encuentra el rol del usuario, mostrar un mensaje de error
                    MessageBox.Show("Error al obtener el rol del usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Cerrar la ventana de inicio de sesión
                this.Close();
            }
            else
            {
                // Si la autenticación falla y el usuario no está inactivo, mostrar un mensaje de error de contraseña
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
