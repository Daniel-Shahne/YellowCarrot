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
using System.Windows.Shapes;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;
using YellowCarrot.Users.Services;

namespace YellowCarrot.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private bool nameLenOk;
        private bool passwordLenOk;
        
        public RegisterWindow()
        {
            InitializeComponent();
        }

        /* Has built in error check before registering a user */
        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new();

            // Checks if lenghts are ok
            if (!nameLenOk) errors.Add("Name needs to be between 4 and 20 characters long");
            if (!passwordLenOk) errors.Add("Password needs to be between 4 and 20 characters long");

            // Checks if usernames could be retrieved, and if its taken in that case
            List<string> usernames = new();
            using (UsersDbContext context = new())
            {
                usernames = await new UserRepo(context).GetUsernamesAsync();
            }
            if (usernames.Count == 0) errors.Add("Could not find any usernames in database");
            else if (usernames.Contains(txbUsername.Text)) errors.Add("Username is already taken");

            // Displays any found errors, or creates an user if none found
            if (errors.Count > 0)
            {
                string errorMsg = String.Join("\n", errors);
                MessageBox.Show(errorMsg, "Could not create user", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using (UsersDbContext context = new())
                {
                    User newUser = new()
                    {
                        Username = txbUsername.Text,
                        Password = pswPassword.Password
                    };

                    await new UserRepo(context).CreateUserAsync(newUser);
                    await context.SaveChangesAsync();

                    MessageBox.Show($"Created user {newUser.Username}!", "User successfully created");
                }
            }
        }

        /* Makes sure username lenght is ok */
        private void txbUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbUsername.Text.Length >= 4 && txbUsername.Text.Length <= 20)
            {
                nameLenOk = true;
                // TODO figure out an ok color
                txbUsername.Foreground = Brushes.Black;
            }
            else 
            {
                nameLenOk = false;
                txbUsername.Foreground = Brushes.Red;
            }
        }

        /* Makes sure password lenght is ok */
        private void pswPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pswPassword.Password.Length >= 4 && pswPassword.Password.Length <= 20)
            {
                passwordLenOk = true;
                // TODO figure out an ok color
                pswPassword.Foreground = Brushes.Black;
            }
            else
            {
                passwordLenOk = false;
                pswPassword.Foreground = Brushes.Red;
            }
        }

        // Returns to mainwindow
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = new();
            mainwin.Show();
            this.Close();
        }
    }
}
