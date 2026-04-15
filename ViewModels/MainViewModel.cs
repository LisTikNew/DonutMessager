using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
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
using System.Windows.Threading;

namespace DonutMessager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _updateTimer;
        private User _currentUser;
        private ChatModel _selectedChat;
        private string _messageText;
        private string _searchText;
        private bool _isTyping;

        public bool IsChatSelected => SelectedChat != null;
        public User CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); } }
        public ObservableCollection<ChatModel> Chats { get; set; } = new();
        public ObservableCollection<MessageModel> Messages { get; set; } = new();
        public ObservableCollection<User> FoundUsers { get; set; } = new();

        public ChatModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsChatSelected));
                Messages.Clear();
                LoadChatMessages();
            }
        }

        public string MessageText { get => _messageText; set { _messageText = value; OnPropertyChanged(); } }
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); SearchUsersInRealTime(); }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand SelectFoundUserCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;
            MessageModel.CurrentUserId = user.Id;

            // Инициализация команд
            SendMessageCommand = new RelayCommand(async _ => await SendMessage(), _ => SelectedChat != null);
            SelectFoundUserCommand = new RelayCommand(obj => SelectFoundUser(obj as User));

            // Таймер обновления
            _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _updateTimer.Tick += (s, e) => { LoadChats(); LoadChatMessages(); };
            _updateTimer.Start();

            LoadChats();
        }

        private void SelectFoundUser(User user)
        {
            if (user == null) return;

            var existingChat = Chats.FirstOrDefault(c => c.UserId == user.Id);

            if (existingChat != null)
            {
                SelectedChat = existingChat;
            }
            else
            {
                var newChat = new ChatModel
                {
                    UserId = user.Id,
                    Title = user.Username,
                    AvatarUrl = user.AvatarPath ?? "/Images/default_avatar.png"
                };
                Chats.Insert(0, newChat);
                SelectedChat = newChat; // После этой строки Grid должен стать Visible
            }

            SearchText = "";
            FoundUsers.Clear();
        }

        private void SearchUsersInRealTime()
        {
            FoundUsers.Clear();
            if (string.IsNullOrWhiteSpace(SearchText)) return;

            string query = SearchText.TrimStart('@').ToLower();
            using var db = new AppDbContext();
            var matches = db.Users
                .Where(u => u.Username.ToLower().StartsWith(query) && u.Id != CurrentUser.Id)
                .Take(5).ToList();

            foreach (var u in matches) FoundUsers.Add(u);
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText) || SelectedChat == null) return;

            using var db = new AppDbContext();
            var msg = new Message
            {
                SenderId = CurrentUser.Id,
                ReceiverId = SelectedChat.UserId,
                Text = MessageText,
                Timestamp = DateTime.UtcNow
            };

            db.Messages.Add(msg);
            await db.SaveChangesAsync();

            Messages.Add(new MessageModel
            {
                Text = MessageText,
                SenderId = CurrentUser.Id,
                Timestamp = msg.Timestamp,
                SenderAvatar = CurrentUser.AvatarPath
            });

            MessageText = "";
        }

        private void LoadChats()
        {
            using var db = new AppDbContext();
            var interactorIds = db.Messages
                .Where(m => m.SenderId == CurrentUser.Id || m.ReceiverId == CurrentUser.Id)
                .Select(m => m.SenderId == CurrentUser.Id ? m.ReceiverId : m.SenderId)
                .Distinct().ToList();

            foreach (var id in interactorIds)
            {
                if (Chats.Any(c => c.UserId == id)) continue;
                var contact = db.Users.Find(id);
                if (contact != null)
                {
                    Chats.Add(new ChatModel { UserId = contact.Id, Title = contact.Username, AvatarUrl = contact.AvatarPath ?? "/Images/default_avatar.png" });
                }
            }
        }

        private void LoadChatMessages()
        {
            if (SelectedChat == null) return;
            using var db = new AppDbContext();

            var history = db.Messages
                .Where(m => (m.SenderId == CurrentUser.Id && m.ReceiverId == SelectedChat.UserId) ||
                            (m.SenderId == SelectedChat.UserId && m.ReceiverId == CurrentUser.Id))
                .OrderBy(m => m.Timestamp).ToList();

            if (history.Count == Messages.Count) return;

            var newMessages = history.Skip(Messages.Count);
            foreach (var m in newMessages)
            {
                Messages.Add(new MessageModel
                {
                    Text = m.Text,
                    SenderId = m.SenderId,
                    Timestamp = m.Timestamp,
                    SenderAvatar = GetAvatar(m.SenderId)
                });
            }
        }

        private string GetAvatar(int userId)
        {
            using var db = new AppDbContext();
            return db.Users.Find(userId)?.AvatarPath ?? "/Images/default_avatar.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}