﻿using APPCENTROH.Views;
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
    }
}
