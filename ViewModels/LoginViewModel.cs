using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessager.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public Action CloseAction { get; set; }
        public string Password { get; set; }
        public ObservableCollection<User> Users { get; set; } = new();

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand CreateUserCommand { get; }

        public event Action<User> LoginSucceeded;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login(SelectedUser));
            CreateUserCommand = new RelayCommand(_ => CreateUser());

            LoadUsers();
        }

        private void LoadUsers()
        {
            using var db = new AppDbContext();
            var localIds = LocalAccounts.GetLoggedUsers();

            Users = db.Users
                      .Where(u => localIds.Contains(u.Id))
                      .ToList();
        }

        private void Login(User selectedUser)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            Properties.Settings.Default.LastUserId = selectedUser.Id;
            Properties.Settings.Default.Save();

            LoginSucceeded?.Invoke(selectedUser);
        }

        private void CreateUser()
        {
            var win = new CreateUserWindow();

            if (win.ShowDialog() == true)
            {
                var newUser = win.CreatedUser;

                if (newUser != null)
                {
                    Users.Add(newUser);
                    SelectedUser = newUser;

                    Properties.Settings.Default.LastUserId = newUser.Id;
                    Properties.Settings.Default.Save();

                    LoginSucceeded?.Invoke(newUser);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ICommand LoginNewAccountCommand => new RelayCommand((_ =>
        {
            var reg = new NewLoginWindow();
            reg.Show();
            CloseAction?.Invoke();
        }));
    }
}