using DonutMessager.Helpers;
using DonutMessager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace DonutMessager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public User CurrentUser { get; }

        public ObservableCollection<Message> Messages { get; set; } = new();

        private User _selectedChatUser;
        public User SelectedChatUser
        {
            get => _selectedChatUser;
            set
            {
                _selectedChatUser = value;
                OnPropertyChanged();
                LoadChatMessages();
            }
        }

        private string _currentMessageText;
        public string CurrentMessageText
        {
            get => _currentMessageText;
            set
            {
                _currentMessageText = value;
                OnPropertyChanged();

                IsTyping = true;
                ResetTypingTimer();
            }
        }

        private bool _isTyping;
        public bool IsTyping
        {
            get => _isTyping;
            set { _isTyping = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }

        private readonly DispatcherTimer typingTimer;

        public MainViewModel(User user)
        {
            CurrentUser = user;

            SendMessageCommand = new RelayCommand(_ => SendMessage());

            typingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            typingTimer.Tick += (_, __) =>
            {
                IsTyping = false;
                typingTimer.Stop();
            };
        }

        private void ResetTypingTimer()
        {
            typingTimer.Stop();
            typingTimer.Start();
        }

        private void LoadChatMessages()
        {
            if (SelectedChatUser == null)
                return;

            using var db = new AppDbContext();

            var msgs = db.Messages
                .Where(m =>
                    (m.SenderId == CurrentUser.Id && m.ReceiverId == SelectedChatUser.Id) ||
                    (m.SenderId == SelectedChatUser.Id && m.ReceiverId == CurrentUser.Id))
                .OrderBy(m => m.Timestamp)
                .ToList();

            Messages.Clear();
            foreach (var m in msgs)
                Messages.Add(m);
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessageText))
                return;

            if (SelectedChatUser == null)
                return;

            var msg = new Message
            {
                SenderId = CurrentUser.Id,
                ReceiverId = SelectedChatUser.Id,
                Text = CurrentMessageText,
                Timestamp = DateTime.Now
            };

            using var db = new AppDbContext();
            db.Messages.Add(msg);
            db.SaveChanges();

            Messages.Add(msg);

            CurrentMessageText = "";
        }
    }
}