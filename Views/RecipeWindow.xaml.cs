﻿using System;
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
    public partial class RecipeWindow : Window
    {
        User loggedInUser;

        internal RecipeWindow(User loggedInUser)
        {
            InitializeComponent();
            this.loggedInUser = loggedInUser;
            GetAllRecipesLV();
        }


        private async void GetAllRecipesLV()
        {
            List<Recipe> recipesList;
            using (FoodDbContext context = new())
            {
                recipesList = await new RecipesRepo(context).GetAllRecipesAsync();
            }

;           
            foreach (Recipe recipe in recipesList)
            {
                RecipeViewModel rvm = await RecipeViewModel.InstantiateAsync(recipe);
                ListViewItem lvi = rvm.createLVI();
                lvRecipes.Items.Add(lvi);
            }
        }

        private void btnRecipeDetails_Click(object sender, RoutedEventArgs e)
        {
            if (lvRecipes.SelectedItem is null)
            {
                MessageBox.Show("Need to select a recipe first");
                return;
            }

            ListViewItem lvi = (ListViewItem)lvRecipes.SelectedItem;
            Recipe selectedRecipe = (Recipe)lvi.Tag;
            
            RecipeDetailsWindow rdw = new(selectedRecipe, loggedInUser);
            rdw.Show();
            this.Close();
        }

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            AddRecipeWindow adw = new(loggedInUser);
            adw.Show();
            this.Close();
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = new();
            mainwin.Show();
            this.Close();
        }

        private void lvRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvRecipes.SelectedItem is not null) 
            {
                btnRecipeDetails.IsEnabled = true;
                ListViewItem lvi = (ListViewItem)lvRecipes.SelectedItem;
                Recipe recipe = (Recipe)lvi.Tag;

                if (loggedInUser.UserId == recipe.UserId) btnRemoveRecipe.IsEnabled = true;
                else btnRemoveRecipe.IsEnabled = false;
            }
            else btnRecipeDetails.IsEnabled = false;

        }

        private async void btnRemoveRecipe_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete this recipe?", "Deletion warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.No) return;

            using (FoodDbContext context = new())
            {
                ListViewItem lvi = (ListViewItem)lvRecipes.SelectedItem;
                Recipe recipe = (Recipe)lvi.Tag;

                new RecipesRepo(context).RemoveRecipe(recipe);
                await context.SaveChangesAsync();
            }

            lvRecipes.Items.Remove(lvRecipes.SelectedItem);
        }
    }
}
