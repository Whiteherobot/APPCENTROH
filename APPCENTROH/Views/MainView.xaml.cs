using APPCENTROH.Models;
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

namespace APPCENTROH.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private UserModel user;
        public MainView(UserModel user)
        {
            InitializeComponent();
            this.user = user;
            txtUserName.Text = $"{user.Name} {user.LastName}"; // Asigna el nombre del usuario al TextBlock
        }

        //public MainView(UserModel user) : this()
        //{
            // Aquí puedes inicializar los controles de la ventana principal utilizando los datos del usuario
        //}

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
          private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
