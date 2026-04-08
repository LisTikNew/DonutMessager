using DonutMessager.Models;
using DonutMessager.ViewModels;
using DonutMessager.Views;
using System.Windows;
using System.Windows.Media.Animation;

namespace DonutMessager
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow(User user)
        {
            InitializeComponent();
            DataContext = new MainViewModel(user);
        }

        private void ChangeAccount_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LastUserId = 0;
            Properties.Settings.Default.Save();
            new LoginWindow().Show();
            this.Close();
        }

        private bool _hintPlayed = false;

        public bool ShowChangeHint { get; private set; }

        private void Hint_Loaded(object sender, RoutedEventArgs e)
        {
            if (_hintPlayed) return;
            _hintPlayed = true;

            var sb = (Storyboard)FindResource("HintFadeStoryboard");
            sb.Begin();

            if (ShowChangeHint)
                ShowChangeHint = false;
        }

        public MainWindow() : this(new User { Username = "Designer", AvatarUrl = "/Images/default_avatar.png" })
        {
        }
    }
}