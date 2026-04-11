using DonutMessager.Models;
using DonutMessager.ViewModels;
using System.Windows;

namespace DonutMessager.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(User user)
        {
            InitializeComponent();

            var vm = new SettingsViewModel(user);
            DataContext = vm;

            vm.CloseAction = Close;
        }
    }
}
