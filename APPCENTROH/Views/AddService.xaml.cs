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
    /// Interaction logic for AddService.xaml
    /// </summary>
    public partial class AddService : Page
    {
        public AddService()
        {
            InitializeComponent();
        }
        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            // Validación de campos vacíos
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    cmbIva.SelectedItem == null)
                {
                    MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación del precio
                if (!decimal.TryParse(txtPrice.Text, out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un valor numérico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Obtener la selección del ComboBox de IVA
                string iva = (cmbIva.SelectedItem as ComboBoxItem).Content.ToString() == "Sí" ? "1" : "2";

                try
                {
                    UserRepository userRepository = new UserRepository();
                    userRepository.InsertService(txtName.Text, precio, iva);

                    MessageBox.Show("Servicio agregado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Limpiar los campos
                    txtName.Clear();
                    txtPrice.Clear();
                    cmbIva.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el servicio: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

                if (mainView != null)
                {
                    // Navegar a la nueva página dentro del frame de la ventana principal
                    mainView.mainFrame.Navigate(new ServiceView());
                }
            


        }

    }
}
