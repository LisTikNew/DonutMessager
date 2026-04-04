using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{

    // -----------------------------
    // 1. ТЕКУЩИЙ ПОЛЬЗОВАТЕЛЬ
    // -----------------------------
    public User CurrentUser { get; set; }

    // -----------------------------
    // 2. КОЛЛЕКЦИИ
    // -----------------------------
    public ObservableCollection<Contact> Contacts { get; set; } = new();
    public ObservableCollection<Message> Messages { get; set; } = new();

    // -----------------------------
    // 3. ВЫБРАННЫЙ КОНТАКТ
    // -----------------------------
    private Contact _selectedContact;
    public Contact SelectedContact
    {
        get => _selectedContact;
        set
        {
            _selectedContact = value;
            OnPropertyChanged();
            LoadMessagesAsync();   // ← ШАГ 6 вызывается здесь
        }
    }

    // -----------------------------
    // 4. ТЕКСТ ВВОДА
    // -----------------------------
    private string _messageText;
    public string MessageText
    {
        get => _messageText;
        set
        {
            _messageText = value;
            OnPropertyChanged();
        }
    }

    // -----------------------------
    // 5. КОМАНДА ОТПРАВКИ
    // -----------------------------
    public ICommand SendMessageCommand { get; }

    // -----------------------------
    // 6. КОНСТРУКТОР
    // -----------------------------
    public MainViewModel(User user)
    {
        CurrentUser = user;

        SendMessageCommand = new RelayCommand(SendMessage);

        LoadContactsAsync();   // ← ШАГ 1
    }

    // -----------------------------
    // 7. ЗАГРУЗКА КОНТАКТОВ
    // -----------------------------
    private async void LoadContactsAsync()
    {
        using var db = new AppDbContext();
        var contacts = await db.Contacts.ToListAsync();

        Contacts.Clear();
        foreach (var c in contacts)
            Contacts.Add(c);
    }

    // -----------------------------
    // 8. ЗАГРУЗКА СООБЩЕНИЙ (ШАГ 6)
    // -----------------------------
    private async void LoadMessagesAsync()
    {
        if (SelectedContact == null)
            return;

        using var db = new AppDbContext();

        var msgs = await db.Messages
    .Where(m =>
        (m.SenderId == CurrentUser.Id && m.ReceiverId == SelectedContact.Id) ||
        (m.SenderId == SelectedContact.Id && m.ReceiverId == CurrentUser.Id))
    .OrderBy(m => m.Timestamp)
    .ToListAsync();

        Messages.Clear();

        foreach (var m in msgs)
        {
            m.IsMine = m.SenderId == CurrentUser.Id;

            var sender = db.Users.First(u => u.Id == m.SenderId);
            m.SenderAvatar = sender.AvatarPath;

            Messages.Add(m);
        }
    }

    // -----------------------------
    // 9. ОТПРАВКА СООБЩЕНИЯ (ШАГ 7)
    // -----------------------------
    private async void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(MessageText) || SelectedContact == null)
            return;

        var msg = new Message
        {
            SenderId = CurrentUser.Id,
            ReceiverId = SelectedContact.Id,
            Text = MessageText,
            Timestamp = DateTime.Now,
            IsMine = true
        };

        using var db = new AppDbContext();
        db.Messages.Add(msg);
        await db.SaveChangesAsync();

        Messages.Add(msg);

        MessageText = "";
        OnPropertyChanged(nameof(MessageText));
    }

    // -----------------------------
    // 10. INotifyPropertyChanged
    // -----------------------------
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}