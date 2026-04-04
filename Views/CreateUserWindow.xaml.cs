using DonutMessager.Models;
using DonutMessager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DonutMessager.Views
{
    public partial class CreateUserWindow : Window
    {
        public User CreatedUser { get; private set; }

        public CreateUserWindow()
        {
            InitializeComponent();

            var vm = new CreateUserViewModel();
            vm.UserCreated += OnUserCreated;

            DataContext = vm;
        }

        private void OnUserCreated(User user)
        {
            CreatedUser = user;
            DialogResult = true;
            Close();
        }
    }
}

