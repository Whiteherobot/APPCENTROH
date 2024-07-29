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
    /// Interaction logic for EmployeView.xaml
    /// </summary>
    public partial class EmployeView : Page
    {
        private UserRepository userRepository = new UserRepository();
        public EmployeView()
        {
            InitializeComponent();
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

            if (mainView != null)
            {
                // Navegar a la nueva página dentro del frame de la ventana principal
                mainView.mainFrame.Navigate(new AddEmploye());
            }

        }
        private void LoadData()
        {
            List<UserModel> users = userRepository.GetAllUsers();
            dataGrid.ItemsSource = users;
        }

        private void BtnAdd_Click1(object sender, RoutedEventArgs e)
        {
            LoadData();

        }
        private void BtnAdd_Click2(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is UserModel selectedUser)
            {
                UserRepository userRepository = new UserRepository();

                bool isDeleted = userRepository.DeleteUserAndRelatedRecords(selectedUser.Id);

                if (isDeleted)
                {
                    var users = (List<UserModel>)dataGrid.ItemsSource;
                    users.Remove(selectedUser);

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = users;

                    MessageBox.Show("Usuario eliminado correctamente.");
                }
                else
                {
                    MessageBox.Show("Error al eliminar el usuario.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un usuario para eliminar.");
            }
        }
        private void btnSearch_Click1(object sender, RoutedEventArgs e)
        {
            string name = txtSearchName.Text;

            if (!string.IsNullOrEmpty(name))
            {
                UserRepository userRepository = new UserRepository();
                List<UserModel> users = userRepository.SearchUsersByName(name);

                if (users != null && users.Count > 0)
                {
                    dataGrid.ItemsSource = users;
                }
                else
                {
                    MessageBox.Show("No se encontraron usuarios con ese nombre.", "Resultado de la búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                    dataGrid.ItemsSource = null; // Limpia el DataGrid si no se encuentran resultados
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un nombre para buscar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnActivating_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is UserModel selectedUser)
            {
                UserRepository userRepository = new UserRepository();

                // Cambia entre activo ('1') e inactivo ('2')
                string newStatus = selectedUser.IsActive == "1" ? "2" : "1";
                userRepository.UpdateUserStatus(selectedUser.Id, newStatus);

                MessageBox.Show(newStatus == "1" ? "El usuario ha sido activado." : "El usuario ha sido desactivado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Actualiza la lista de usuarios en la UI
                LoadData();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string name = txtSearchName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Por favor, ingrese el nombre del usuario a buscar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserRepository userRepository = new UserRepository();
            var users = userRepository.SearchUsersByName1(name);

            if (users.Count > 0)
            {
                dataGrid.ItemsSource = users;
            }
            else
            {
                MessageBox.Show("No se encontraron usuarios con ese nombre.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
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
