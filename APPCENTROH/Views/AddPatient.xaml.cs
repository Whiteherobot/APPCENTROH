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
    /// Interaction logic for AddPatient.xaml
    /// </summary>
    public partial class AddPatient : Page
    {
        public AddPatient()
        {
            InitializeComponent();
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {

            // Supongamos que tienes un ViewModel ya poblado con los datos de la vista
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                cedula = txtID.Text,
                nombre = txtName.Text,
                apellido = txtSecondName.Text,
                direccion = txtAddres.Text,
                telefono = txtPhone.Text,
                Email = txtEmail.Text,
                fecha = txtFecha.Text
            };

            UserRepository userRepository = new UserRepository();
            if (userRepository.IsCedulaExists(txtID.Text))
            {
                MessageBox.Show("La cédula ya está registrada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Llama al método de inserción
           
            userRepository.InsertData1(registerViewModel);

            // Encuentra la ventana principal abierta
            MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

            if (mainView != null)
            {
                // Navegar a la nueva página dentro del frame de la ventana principal
                mainView.mainFrame.Navigate(new CustomerView());
            }


        }



    }
}
