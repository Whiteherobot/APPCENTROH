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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APPCENTROM.Views
{
    /// <summary>
    /// Interaction logic for AddEmploye.xaml
    /// </summary>
    public partial class AddEmploye : Page
    {

        public RegisterViewModel RegisterViewModel { get; private set; }
        public AddEmploye()
        {
            InitializeComponent();
        }
        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Password) ||
                string.IsNullOrWhiteSpace(txtPassConfirm.Password) ||
                string.IsNullOrWhiteSpace(cmbEmployeeType.Text) ||
                string.IsNullOrWhiteSpace(cmbPermiso.Text))
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
            if (userRepository.IsCedulaExists(txtIDnumber.Text))
            {
                MessageBox.Show("La cédula ya está registrada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Crea el modelo de registro con los datos del formulario
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                nombre = txtName.Text,
                apellido = txtSecondName.Text,
                cedula = txtIDnumber.Text,
                direccion = txtAddres.Text,
                telefono = txtPhone.Text,
                Email = txtMail.Text,
                tipo = cmbEmployeeType.Text // El tipo de empleado seleccionado
            };

            string username = txtUser.Text;
            string password = txtPass.Password;
            string permiso = cmbPermiso.Text; // El permiso seleccionado

            // Llama al método que realiza la inserción en la base de datos
            
            userRepository.InsertData(registerViewModel, username, password, permiso);

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
