using DonutMessager.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DonutMessager
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _vm = new MainViewModel(currentUser);
            DataContext = _vm;
        }

        public MainWindow() : this(new User { Username = "Designer", AvatarUrl = "/Images/default_avatar.png" })
        {
        }
    }
}
