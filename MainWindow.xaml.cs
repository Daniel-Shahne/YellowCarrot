using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YellowCarrot.Food.Data;
using YellowCarrot.Food.Models;
using YellowCarrot.Food.Services;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;
using YellowCarrot.Users.Services;
using YellowCarrot.Views;

namespace YellowCarrot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Opens register window and closes this
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regwin = new();
            regwin.Show();
            this.Close();
        }

        /* Attempts a login */
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            User? attemptedUser;

            using (UsersDbContext context = new())
            {
                attemptedUser = await new UserRepo(context).GetUserByUsernameAsync(txbUsername.Text);
            }

            if (attemptedUser is null)
            {
                MessageBox.Show($"Could not find any user {txbUsername.Text}", "Login failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else if (!attemptedUser.Password.Equals(pswPassword.Password))
            {
                MessageBox.Show("Wrong password", "Login failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else if (attemptedUser.Password.Equals(pswPassword.Password))
            {
                MessageBox.Show("can login");
            }
        }
    }
}
