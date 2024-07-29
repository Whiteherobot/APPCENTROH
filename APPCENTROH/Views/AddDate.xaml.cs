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
    /// Interaction logic for AppointmentsView.xaml
    /// </summary>
    public partial class AddDate : Page
    {
       
        public AddDate()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;

        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();

            if (mainView != null)
            {
                // Navegar a la nueva página dentro del frame de la ventana principal
                mainView.mainFrame.Navigate(new ServiceView());
            }
        }

        private void TxtAppointmentTime_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "HH:mm")
            {
                textBox.Text = string.Empty;
            }
        }

        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            UserRepository userRepository = new UserRepository();

            // Validar campos obligatorios
            if (cmbDoctors.SelectedItem == null || cmbPatients.SelectedItem == null || dpAppointmentDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtAppointmentTime.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar formato de fecha y hora
            DateTime appointmentDateTime;
            if (!DateTime.TryParse($"{dpAppointmentDate.SelectedDate.Value.ToShortDateString()} {txtAppointmentTime.Text}", out appointmentDateTime))
            {
                MessageBox.Show("Fecha y hora de la cita no válidas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var doctorId = (int)cmbDoctors.SelectedValue;
            var patientId = (int)cmbPatients.SelectedValue;
            var estado = ((ComboBoxItem)cmbEstado.SelectedItem).Content.ToString();

            // Llamar al método para agregar la cita
            if (userRepository.AddAppointment(appointmentDateTime, estado, doctorId, patientId))
            {
                MessageBox.Show("Cita agregada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("El médico ya tiene una cita a esta hora.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var userRepository = new UserRepository();
            cmbDoctors.ItemsSource = userRepository.GetDoctors();
            cmbPatients.ItemsSource = userRepository.GetPatients();
        }
    }
}
