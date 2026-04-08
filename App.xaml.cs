using DonutMessager.Properties;
using DonutMessager.Views;
using System.Windows;

namespace DonutMessager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            int lastId = Settings.Default.LastUserId;

            if (lastId > 0)
            {
                using var db = new AppDbContext();
                var user = db.Users.FirstOrDefault(u => u.Id == lastId);

                if (user != null)
                {
                    new MainWindow(user).Show();
                    return;
                }
            }

            new LoginWindow().Show();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
        }
    }
}