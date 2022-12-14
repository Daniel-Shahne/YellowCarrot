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
using YellowCarrot.Food.Models;
using YellowCarrot.Users.Models;

namespace YellowCarrot.Views
{
    /// <summary>
    /// Interaction logic for AddRecipeWindow.xaml
    /// </summary>
    public partial class AddRecipeWindow : Window
    {
        private User loggedInUser;

        private bool recipeNameLenOk;
        private bool tagNameLenOk;

        internal AddRecipeWindow(User loggedInUser)
        {
            InitializeComponent();
            this.loggedInUser = loggedInUser;
        }

        // Limited to max 50 characters
        private void txbRecipeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbRecipeName.Text.Length >= 3 && txbRecipeName.Text.Length <= 50)
            {
                recipeNameLenOk = true;
                txbRecipeName.Foreground = Brushes.Black;
            }
            else
            {
                recipeNameLenOk = false;
                txbRecipeName.Foreground = Brushes.Red;
            }
        }

        // Limited to max 20 characters
        private void txbTagName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbTagName.Text.Length >= 3 && txbTagName.Text.Length <= 20)
            {
                tagNameLenOk = true;
                txbTagName.Foreground = Brushes.Black;
            }
            else
            {
                tagNameLenOk = false;
                txbTagName.Foreground = Brushes.Red;
            }
        }

        /* Creates a new listviewitem with the Food Tag object in the
         * lvi's Tag property as an object. */
        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            if (tagNameLenOk)
            {
                ListViewItem lvi = new();
                lvi.Tag = new Tag() { TagName = txbTagName.Text };
                lvi.Content = txbTagName.Text;
                lvTags.Items.Add(lvi);
            }
            else
            {
                MessageBox.Show("Tag name needs to be between 4-20 characters long");
            }
        }

        // Closes window and returns to recipes
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RecipeWindow recwin = new(loggedInUser);
            recwin.Show();
            this.Close();
        }
    }
}
