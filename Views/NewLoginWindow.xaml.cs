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
    public partial class NewLoginWindow : Window
    {
        public NewLoginWindow()
        {
            InitializeComponent();

            var vm = new NewLoginViewModel();
            vm.CloseAction = Close;
            DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is NewLoginViewModel vm)
                vm.Password = PasswordBox.Password;
        }
    }
}
