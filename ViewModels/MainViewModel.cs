using DonutMessager;
using DonutMessager.Helpers;
using DonutMessager.Models;
using DonutMessenger.Client.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;


namespace DonutMessager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ChatModel> Chats { get; set; } = new();
        public ObservableCollection<MessageModel> Messages { get; set; } = new();

        private bool _showChangeHint = true;
        public bool ShowChangeHint
        {
            get => _showChangeHint;
            set { _showChangeHint = value; OnPropertyChanged(); }
        }


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

        public ICommand SendMessageCommand { get; }

        private readonly SignalRService _signalR;

        public MainViewModel(User user)
        {
            CurrentUser = user;
            ShowChangeHint = false;
            MessageModel.CurrentUserId = CurrentUser.Id;

            SendMessageCommand = new RelayCommand(async _ => await SendMessage());

            _signalR = new SignalRService("http://localhost:5227");
            InitializeSignalR();

            LoadChats();
        }

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
                                Timestamp = timestamp
                            });
                        });
                    }
                });
        }

        private async void LoadChats()
        {
            Chats.Clear();
            Chats.Add(new ChatModel { Id = 1, Title = "Test chat", AvatarUrl = "/Images/default_avatar.png" });
            Chats.Add(new ChatModel { Id = 2, Title = "Another chat", AvatarUrl = "/Images/default_avatar.png" });
        }

        private async void LoadChatMessages()
        {
            if (SelectedChat == null) return;

            // TODO: запрос на сервер /api/message/{chatId}/history
        }

        private async void JoinChatRoom()
        {
            if (SelectedChat != null)
                await _signalR.JoinChat(SelectedChat.Id);
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText) || SelectedChat == null)
                return;

            await _signalR.SendMessage(SelectedChat.Id, CurrentUser.Id, MessageText);

            MessageText = "";
            OnPropertyChanged(nameof(MessageText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}