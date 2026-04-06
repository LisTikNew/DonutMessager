using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DonutMessager.ViewModels;

public class CreateUserViewModel : BaseViewModel
{
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

    public ICommand SelectAvatarCommand { get; }
    public ICommand CreateUserCommand { get; }

    public event Action<User> UserCreated;

    public CreateUserViewModel()
    {
        SelectAvatarCommand = new RelayCommand(_ => SelectAvatar());
        CreateUserCommand = new RelayCommand(_ => CreateUser());

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