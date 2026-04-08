using DonutMessager.Helpers;
using DonutMessager.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Action CloseAction { get; set; }

        public ICommand LogoutCommand { get; }
        public ICommand ChangeAccountCommand { get; }

        public SettingsViewModel()
        {
            LogoutCommand = new RelayCommand(_ =>
            {
                Properties.Settings.Default.LastUserId = 0;
                Properties.Settings.Default.Save();

                new LoginWindow().Show();
                CloseAction?.Invoke();
            });

            ChangeAccountCommand = new RelayCommand(_ =>
            {
                Properties.Settings.Default.LastUserId = 0;
                Properties.Settings.Default.Save();

                new LoginWindow().Show();
                CloseAction?.Invoke();
            });
        }
    }
}
