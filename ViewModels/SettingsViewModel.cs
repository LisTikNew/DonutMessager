using DonutMessager.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    class SettingsViewModel
    {
        public ICommand LogoutCommand => new RelayCommand(_ =>
        {
            Properties.Settings.Default.LastUserId = 0;
            Properties.Settings.Default.Save();

            new LoginWindow().Show();
            CloseAction?.Invoke();
        });

        public ICommand ChangeAccountCommand => new RelayCommand(() =>
        {
            Properties.Settings.Default.LastUserId = 0;
            Properties.Settings.Default.Save();

            new LoginWindow().Show();
            CloseAction?.Invoke();
        });
    }
}
