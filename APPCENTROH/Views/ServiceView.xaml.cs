using APPCENTROH.Models;
using APPCENTROH.Repositories;
using APPCENTROH.Views;
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
    /// Interaction logic for ServiceView.xaml
    /// </summary>
    public partial class ServiceView : Page
    {
        public ServiceView()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

            if (mainView != null)
            {
                // Navegar a la nueva página dentro del frame de la ventana principal
                mainView.mainFrame.Navigate(new AddService());
            }

        }

        private void BtnAdd_Click1(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }

        private void LoadServices()
        {
            UserRepository userRepository = new UserRepository();
            List<UserModel> services = userRepository.GetAllServices();

            // Supongamos que tienes un DataGrid llamado 'dataGridServices'
            dataGrid.ItemsSource = services;
        }

        private void BtnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is UserModel selectedService)
            {
                UserRepository userRepository = new UserRepository();

                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea eliminar este servicio?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool isDeleted = userRepository.DeleteService(selectedService.Id);

                    if (isDeleted)
                    {
                        MessageBox.Show("Servicio eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Actualiza la lista de servicios en la UI
                        LoadServices();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el servicio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un servicio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = txtSearchName.Text.Trim();

            if (string.IsNullOrEmpty(serviceName))
            {
                MessageBox.Show("Por favor, ingrese el nombre del servicio a buscar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserRepository userRepository = new UserRepository();
            var services = userRepository.SearchServicesByName(serviceName);

            if (services.Count > 0)
            {
                dataGrid.ItemsSource = services;
            }
            else
            {
                MessageBox.Show("No se encontraron servicios con ese nombre.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid.ItemsSource = null; // O mantén la lista previa según sea necesario
            }
        }

        private void txtSearchName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchName.Text == "Search...")
            {
                txtSearchName.Text = string.Empty;
                txtSearchName.Foreground = new SolidColorBrush(Colors.White); // Cambia el color del texto al hacer clic
            }
        }
    }
}
