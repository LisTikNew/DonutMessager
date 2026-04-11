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
    /// Interaction logic for NewLoginWindow.xaml
    /// </summary>
    public partial class NewLoginWindow : Window
    {
        public NewLoginWindow()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                if (DataContext is NewLoginViewModel vm)
                    vm.CloseAction = Close;
            };
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is NewLoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
