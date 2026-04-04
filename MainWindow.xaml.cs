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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();
            DataContext = new MainViewModel(user);
        }





        public void AddContact(string name)
        {
            using (var db = new AppDbContext())
            {
                var contact = new Contact { Name = name };
                db.Contacts.Add(contact);
                db.SaveChanges();

                var vm = DataContext as MainViewModel;
                if (vm != null)
                { 
                    vm.Contacts.Add(contact);
                }
            }
        }
    }
}