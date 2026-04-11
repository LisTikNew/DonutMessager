using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessenger.Client.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DonutMessager.Views;

namespace DonutMessager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // -----------------------------
        //  PROPERTIES
        // -----------------------------

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ChatModel> Chats { get; set; } = new();
        public ObservableCollection<MessageModel> Messages { get; set; } = new();

        private ChatModel _selectedChat;
        public ChatModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                if (_selectedChat != value)
                {
                    _selectedChat = value;
                    OnPropertyChanged();

                    LoadChatMessages();
                    JoinChatRoom();
                }
            }
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        private bool _isTyping;
        public bool IsTyping
        {
            get => _isTyping;
            set { _isTyping = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }

        private readonly SignalRService _signalR;

        // -----------------------------
        //  CONSTRUCTOR
        // -----------------------------

        public MainViewModel(User user)
        {
            CurrentUser = user;
            if (CurrentUser == null)
                MessageBox.Show("CurrentUser == null");

            MessageModel.CurrentUserId = CurrentUser.Id;

            SendMessageCommand = new RelayCommand(async _ => await SendMessage());

            _signalR = new SignalRService("http://localhost:5227");
            InitializeSignalR();

            LoadChats();
        }

        // -----------------------------
        //  SIGNALR
        // -----------------------------

        private async void InitializeSignalR()
        {
            await _signalR.StartAsync();

            _signalR.Connection.On("ReceiveMessage",
                (int chatId, int senderId, string text, DateTime timestamp) =>
                {
                    if (SelectedChat != null && SelectedChat.Id == chatId)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Messages.Add(new MessageModel
                            {
                                ChatId = chatId,
                                SenderId = senderId,
                                Text = text,
                                Timestamp = timestamp,
                                SenderAvatar = GetAvatar(senderId)
                            });
                        });
                    }
                });
        }

        // -----------------------------
        //  LOAD CHATS
        // -----------------------------

        private void LoadChats()
        {
            Chats.Clear();

            // Заглушки — заменишь на реальные чаты
            Chats.Add(new ChatModel { Id = 1, Title = "Test chat", UserId = 2, AvatarUrl = "/Images/default_avatar.png" });
            Chats.Add(new ChatModel { Id = 2, Title = "Another chat", UserId = 3, AvatarUrl = "/Images/default_avatar.png" });
        }

        // -----------------------------
        //  LOAD MESSAGES
        // -----------------------------

        private void LoadChatMessages()
        {
            if (SelectedChat == null)
                return;

            Messages.Clear();

            using var db = new AppDbContext();

            var msgs = db.Messages
                .Where(m => m.ChatId == SelectedChat.Id)
                .OrderBy(m => m.Timestamp)
                .ToList();

            foreach (var m in msgs)
            {
                Messages.Add(new MessageModel
                {
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    Timestamp = m.Timestamp,
                    SenderAvatar = GetAvatar(m.SenderId)
                });
            }
        }

        // -----------------------------
        //  JOIN CHAT ROOM
        // -----------------------------

        private async void JoinChatRoom()
        {
            if (SelectedChat != null)
                await _signalR.JoinChat(SelectedChat.Id);
        }

        // -----------------------------
        //  SEND MESSAGE
        // -----------------------------

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText))
                return;

            if (SelectedChat == null)
                return;

            var msg = new Message
            {
                ChatId = SelectedChat.Id,
                SenderId = CurrentUser.Id,
                Text = MessageText,
                Timestamp = DateTime.UtcNow
            };

            using var db = new AppDbContext();
            db.Messages.Add(msg);
            db.SaveChanges();

            Messages.Add(new MessageModel
            {
                ChatId = msg.ChatId,
                SenderId = msg.SenderId,
                Text = msg.Text,
                Timestamp = msg.Timestamp,
                SenderAvatar = CurrentUser.AvatarPath
            });

            MessageText = "";
        }

        // -----------------------------
        //  HELPERS
        // -----------------------------

        private string GetAvatar(int userId)
        {
            using var db = new AppDbContext();
            return db.Users.FirstOrDefault(u => u.Id == userId)?.AvatarPath
                   ?? "/Images/default_avatar.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}