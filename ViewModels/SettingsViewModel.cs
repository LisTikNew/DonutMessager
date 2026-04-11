using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessager.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public User CurrentUser { get; }

        public Action CloseAction { get; set; }

        public ICommand LogoutCommand { get; }
        public ICommand ChangeAccountCommand { get; }

        public SettingsViewModel(User user)
        {
            CurrentUser = user;

            LogoutCommand = new RelayCommand(_ =>
            {
                var confirm = new ConfirmLogoutWindow();
                confirm.Owner = Application.Current.MainWindow;
                confirm.ShowDialog();

                if (!confirm.Confirmed)
                    return;

                LocalUsers.Remove(CurrentUser.Id);

                Properties.Settings.Default.LastUserId = 0;
                Properties.Settings.Default.Save();

                CloseAction?.Invoke();

                Application.Current.Shutdown();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            });

            ChangeAccountCommand = new RelayCommand(_ =>
            {
                Properties.Settings.Default.LastUserId = 0;
                Properties.Settings.Default.Save();

                var login = new LoginWindow();
                Application.Current.MainWindow = login;
                login.Show();
                foreach (Window w in Application.Current.Windows)
                    if (w != login)
                        w.Close();
            });
        }
    }
}