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
using YellowCarrot.Food.Data;
using YellowCarrot.Food.Models;
using YellowCarrot.Food.Services;
using YellowCarrot.Food.ViewModels;
using YellowCarrot.Users.Models;

namespace YellowCarrot.Views
{
    /// <summary>
    /// Interaction logic for RecipeWindow.xaml
    /// </summary>
    internal partial class RecipeWindow : Window
    {
        User loggedInUser;

        internal RecipeWindow(User loggedInUser)
        {
            InitializeComponent();
            this.loggedInUser = loggedInUser;
        }


        private async void refreshRecipesLV()
        {
            List<Recipe> recipesList;
            using (FoodDbContext context = new())
            {
                recipesList = await new RecipesRepo(context).GetAllRecipesAsync();
            }

;
            foreach (Recipe recipe in recipesList)
            {
                RecipeViewModel RVM = new RecipeViewModel(recipe);
                lvRecipes.Items.Add(RVM.createLVI());
            }
        }
    }
}
