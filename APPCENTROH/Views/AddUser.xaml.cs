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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace APPCENTROM.Views
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Page
    {
        private RegisterViewModel _registerViewModel;
        private RegisterViewModel registerViewModel;
        public AddUser(RegisterViewModel data)
        {
            _registerViewModel = data;
            InitializeComponent();
            _registerViewModel = registerViewModel;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Password) ||
                string.IsNullOrWhiteSpace(txtPassConfirm.Password) ||
                string.IsNullOrWhiteSpace(txtPermiso.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPass.Password != txtPassConfirm.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_registerViewModel == null)
            {
                MessageBox.Show("No se ha proporcionado información de la persona.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Llama al método que realiza la inserción en la base de datos
            UserRepository userRepository = new UserRepository();
            userRepository.InsertData(_registerViewModel, txtUser.Text, txtPass.Password, txtPermiso.Text);

            // Mostrar mensaje de éxito y cerrar las ventanas
            MessageBox.Show("Registro exitoso", "Éxito");

            MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

            if (mainView != null)
            {
                mainView.mainFrame.Navigate(new EmployeView());
            }
        }
    }
    }
    
