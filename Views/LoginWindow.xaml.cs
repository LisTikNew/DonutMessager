using DonutMessager.Models;
using DonutMessager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DonutMessager.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var vm = new LoginViewModel();
            vm.LoginSucceeded += OnLoginSucceeded;

            DataContext = vm;
        }

        private void OnLoginSucceeded(User user)
        {
            var main = new MainWindow(user);
            main.Show();
            Close();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as LoginViewModel;
            if (vm?.SelectedUser != null)
                vm.LoginCommand.Execute(null);
        }
    }
}
