using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessager.ViewModels;
using DonutMessager.Views;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

public class CreateUserViewModel : BaseViewModel
{
    public Action CloseAction { get; set; }

    private string _username;
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _avatarPath;
    public string AvatarPath
    {
        get => _avatarPath;
        set { _avatarPath = value; OnPropertyChanged(); }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public ICommand SelectAvatarCommand { get; }
    public ICommand CreateUserCommand { get; }

    public event Action<User> UserCreated;

    public CreateUserViewModel()
    {
        SelectAvatarCommand = new RelayCommand(_ => SelectAvatar());
        CreateUserCommand = new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using var db = new AppDbContext();

            var user = new User
            {
                Username = Username,
                Password = Password // позже можно хешировать
            };

            db.Users.Add(user);
            db.SaveChanges();

            MessageBox.Show("Аккаунт создан!");

            new LoginWindow().Show();
            CloseAction?.Invoke();
        });

    }

    private void SelectAvatar()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Images|*.png;*.jpg;*.jpeg"
        };

        if (dialog.ShowDialog() == true)
        {
            AvatarPath = dialog.FileName;
        }
    }

    private void CreateUser()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            MessageBox.Show("Введите имя пользователя");
            return;
        }

        using var db = new AppDbContext();

        var user = new User
        {
            Username = Username,
            AvatarPath = AvatarPath,   // может быть null — это нормально
            PasswordHash = null        // пока не используем
        };

        db.Users.Add(user);
        db.SaveChanges();

        UserCreated?.Invoke(user);
    }
}