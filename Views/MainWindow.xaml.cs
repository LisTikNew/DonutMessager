using DonutMessager.Models;
using DonutMessager.ViewModels;
using DonutMessager.Views;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media.Animation;


namespace DonutMessager.Views
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

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var settings = new SettingsWindow(vm.CurrentUser);
                settings.ShowDialog();
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChatScroll.ScrollToEnd();
        }

    }
}