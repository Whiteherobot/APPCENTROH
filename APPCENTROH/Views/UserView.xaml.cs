using APPCENTROH.Repositories;
using APPCENTROH.Views;
using APPCENTROM.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace APPCENTROM.Views
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        private RegisterViewModel registerViewModel;

        public UserView(RegisterViewModel data)
        {
            InitializeComponent();
            registerViewModel = data;
        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBlock_MouseDown(object sender, RoutedEventArgs e)
        {
            // LoginView loginView = new LoginView();

            // loginView.Show();

            // this.Close();
        }
    }
}
