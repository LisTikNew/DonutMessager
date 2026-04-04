using DonutMessager.Helpers;
using DonutMessager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace DonutMessager.ViewModels
{
    public class CreateUserViewModel : INotifyPropertyChanged
    {
        public string Username { get; set; }
        public string AvatarPath { get; set; }

        public ICommand SelectAvatarCommand { get; }
        public ICommand CreateCommand { get; }

        public event Action<User> UserCreated;

        public CreateUserViewModel()
        {
            SelectAvatarCommand = new RelayCommand(SelectAvatar);
            CreateCommand = new RelayCommand(CreateUser);
        }

        private void SelectAvatar()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Images|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                AvatarPath = dialog.FileName;
                OnPropertyChanged(nameof(AvatarPath));
            }
        }

        private void CreateUser()
        {
            if (string.IsNullOrWhiteSpace(Username))
                return;

            using var db = new AppDbContext();

            var user = new User
            {
                Username = Username,
                AvatarPath = AvatarPath
            };

            db.Users.Add(user);
            db.SaveChanges();

            UserCreated?.Invoke(user);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
