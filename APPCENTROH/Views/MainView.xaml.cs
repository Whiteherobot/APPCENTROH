using APPCENTROH.Models;
using APPCENTROM.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

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
            txtNameUser.Text = $"{user.Name} {user.LastName}"; // Asigna el nombre del usuario al TextBlock
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);

        }
        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

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
        private void RbShowHomeView_Click(object sender, RoutedEventArgs e)
        {
            HomeView homeView = new HomeView();

            mainFrame.Navigate(new HomeView());

        }
        
        private void RbShowHomeView_Click1(object sender, RoutedEventArgs e)
        {
            CustomerView customerView = new CustomerView();

            mainFrame.Navigate(new CustomerView());

        }

        private void RbShowHomeView_Click2(object sender, RoutedEventArgs e)
        {

            mainFrame.Navigate(new PACA());

        }

        private void RbShowHomeView_Click3(object sender, RoutedEventArgs e)
        {
            

            mainFrame.Navigate(new AppointmentsView());

        }

        private void RbShowHomeView_Click4(object sender, RoutedEventArgs e)
        {

            mainFrame.Navigate(new InvoiceView());

        }
    }
}
