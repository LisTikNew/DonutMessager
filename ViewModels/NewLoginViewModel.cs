using DonutMessager.Helpers;
using DonutMessager.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class NewLoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private string _avatarPath;
        public string AvatarPath
        {
            get => _avatarPath;
            set { _avatarPath = value; OnPropertyChanged(); }
        }

        private bool _isUserFound;
        public bool IsUserFound
        {
            get => _isUserFound;
            set { _isUserFound = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public Action CloseAction { get; set; }

        public NewLoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Username == Username);

            if (user == null)
            {
                ErrorMessage = "Wrong Username";
                IsUserFound = false;
                return;
            }

            // Username found → show avatar
            AvatarPath = user.AvatarPath;
            IsUserFound = true;

            // Check password
            if (string.IsNullOrWhiteSpace(Password))
                return;

            if (Password != user.Password)
            {
                ErrorMessage = "Wrong Password";
                return;
            }

            if (user != null && user.Password == Password)
            {
                // 4. Добавляем пользователя в локальный список
                LocalUsers.Add(user.Id);

                Properties.Settings.Default.LastUserId = user.Id;
                Properties.Settings.Default.Save();

                var main = new MainWindow(user);
                Application.Current.MainWindow = main;
                main.Show();

                MessageBox.Show(
                    "New account added successfully",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CloseAction?.Invoke();
            }
            else
            {
                ErrorMessage = "Wrong Username or Password";
            }
        }
    }
}
