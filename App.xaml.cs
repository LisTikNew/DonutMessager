using DonutMessager.Models;
using DonutMessager.Properties;
using DonutMessager.Views;
using System.Linq;
using System.Windows;

namespace DonutMessager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using var db = new AppDbContext();

            // 1. Автологин
            int lastId = Settings.Default.LastUserId;

            if (lastId > 0)
            {
                var user = db.Users.FirstOrDefault(u => u.Id == lastId);
                if (user != null)
                {
                    // Автоматический вход
                    var main = new MainWindow(user);
                    MainWindow = main;
                    main.Show();
                    return;
                }
            }

            // 2. Если есть аккаунты → LoginWindow
            bool hasUsers = db.Users.Any();

            if (hasUsers)
            {
                var login = new LoginWindow();
                MainWindow = login;
                login.Show();
                return;
            }

            // 3. Если нет аккаунтов → CreateUserWindow
            var create = new CreateUserWindow();
            MainWindow = create;
            create.Show();
        }
    }
}