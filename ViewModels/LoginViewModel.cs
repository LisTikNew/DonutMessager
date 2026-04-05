using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
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
            LoginCommand = new RelayCommand(Login);
            CreateUserCommand = new RelayCommand(CreateUser);

            LoadUsers();
        }

        private void LoadUsers()
        {
            using var db = new AppDbContext();
            var users = db.Users.ToList();

            Users.Clear();
            foreach (var u in users)
                Users.Add(u);
        }

        private void Login()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите аккаунт!");
                return;
            }

            LoginSucceeded?.Invoke(SelectedUser);
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
                    LoginSucceeded?.Invoke(newUser);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

