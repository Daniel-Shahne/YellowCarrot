using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using YellowCarrot.Users.Models;

namespace YellowCarrot.Views
{
    /// <summary>
    /// Interaction logic for RecipeDetailsWindow.xaml
    /// </summary>
    public partial class RecipeDetailsWindow : Window
    {
        private Recipe recipe;
        private User loggedInUser;
        private Regex quantityRegex = new Regex(@"^(\d{1,5})\s(\w{1,15})$");
        private bool ingredientNameLenOk;
        private bool quantityTxbOk;
        private List<Ingredient> ingredientsToRemoveList = new();
        private string previousRecipeName;

        internal RecipeDetailsWindow(Recipe recipe, User loggedInUser)
        {
            InitializeComponent();
            this.recipe = recipe;
            this.loggedInUser = loggedInUser;
            previousRecipeName = recipe.Name;
            OnWindowCreated();
        }

        /* Called when window is instantiated, in CTOR. Fills
         * the listviews with the recipes information and enables/disables
         * remove recipe button */
        private void OnWindowCreated()
        {
            // Tags wont be changed so no need for getting whole tag object in LVI
            foreach (Tag tag in recipe.Tags)
            {
                ListViewItem lvi = new();
                lvi.Content = tag.TagName;
                lvTags.Items.Add(lvi);
            }

            // Same as above but for steps
            foreach (Step step in recipe.Steps)
            {
                ListViewItem lvi = new();
                lvi.Content = step.Description;
                lvSteps.Items.Add(lvi);
            }

            // Ingredients can however be changed so entire object is referenced in LVI tag
            foreach (Ingredient ingredient in recipe.Ingredients)
            {
                ListViewItem lvi = new();
                lvi.Tag = ingredient;
                lvi.Content = ingredient.Name;
                lvIngredients.Items.Add(lvi);
            }

            // If recipes and logged in users id's match then enable remove button
            if (recipe.UserId == loggedInUser.UserId)
            {
                // btnRemoveRecipe.IsEnabled = true;
                btnUnlock.IsEnabled = true;
            }
        }


        // Limited to 20 characters
        private void txbIngredientName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbIngredientName.Text.Length >= 3 && txbIngredientName.Text.Length <= 20)
            {
                ingredientNameLenOk = true;
                txbIngredientName.Foreground = Brushes.Black;
            }
            else
            {
                ingredientNameLenOk = false;
                txbIngredientName.Foreground = Brushes.Red;
            }
        }

        // Quantity should be separate quantifier and unit, matched and verified by regex.
        private void txbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            Match regMatch = quantityRegex.Match(txbQuantity.Text);
            if (regMatch.Success)
            {
                txbQuantity.Foreground = Brushes.Black;
                quantityTxbOk = true;
            }
            else
            {
                txbQuantity.Foreground = Brushes.Red;
                quantityTxbOk = false;
            }
        }

        private void btnUnlock_Click(object sender, RoutedEventArgs e)
        {
            btnUpdateRecipe.IsEnabled = true;
            btnAddIngredient.IsEnabled = true;
            btnRemoveIngredient.IsEnabled = true;
            txbIngredientName.IsEnabled = true;
            txbQuantity.IsEnabled = true;
            txbRecipeName.IsEnabled = true;

            txbRecipeName.Text = previousRecipeName;

            btnUnlock.IsEnabled = false;
        }

        /* Unlike addrecipewindow, changes in the recipe object is made as
         * objects in LV is added/removed, instead of all changes at once at
         * the end depending on what objects are in LV. Otherwise identical to
         * the method in that window. */
        private async void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new();
            if (!ingredientNameLenOk) errors.Add("Ingredient name needs to be between 3 and 20 characters long");
            if (!quantityTxbOk) errors.Add("Quantity needs to be an integer (1-5 chars) and unit (1-15 chars) separated by space");

            if (errors.Count > 0)
            {
                string errorMsg = String.Join("\n", errors);
                MessageBox.Show(errorMsg, "Add ingredient failed");
                return;
            }
            else
            {
                Match regmatch = quantityRegex.Match(txbQuantity.Text);

                Ingredient newIngredient = new()
                {
                    Name = txbIngredientName.Text,
                    Quantity = Int32.Parse(regmatch.Groups[1].Value),
                    QuantityUnit = regmatch.Groups[2].Value
                };

                ListViewItem lvi = new();
                lvi.Tag = newIngredient;
                lvi.Content = $"{newIngredient.Quantity} {newIngredient.QuantityUnit} of {newIngredient.Name}";
                lvIngredients.Items.Add(lvi);


                // Changes are only visually represented by LV, actual changes are made here
                // recipe.Ingredients.Add(newIngredient); //Removed this since it required updating the entity

                using (FoodDbContext context = new())
                {
                    Recipe actualRecipe = await new RecipesRepo(context).GetRecipeByIdAsync(this.recipe.RecipeId);
                    actualRecipe.Ingredients.Add(newIngredient);
                    await context.SaveChangesAsync();
                }
            }
        }

        private async void btnUpdateRecipe_Click(object sender, RoutedEventArgs e)
        {
            using (FoodDbContext context = new())
            {
                if (txbRecipeName.Text != previousRecipeName)
                {
                    recipe.Name = txbRecipeName.Text;
                }
                new IngredientsRepo(context).removeIngredients(ingredientsToRemoveList); // No need to update the recipe after removing an ingredient
                ingredientsToRemoveList.Clear();
                context.Recipes.Update(recipe); // This has some weird interaction with Tags, attempting to insert the tags again?
                await context.SaveChangesAsync();
            }

            MessageBox.Show("Updated recipe!");
        }

        /* Removes an ingredient from the listview and from the
         * recipe */
        private async void btnRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (lvIngredients.SelectedItem is null)
            {
                MessageBox.Show("Select an ingredient first");
                return;
            }

            ListViewItem lvi = (ListViewItem)lvIngredients.SelectedItem;
            Ingredient ingredientToRemove = (Ingredient)lvi.Tag;

            // ingredientsToRemoveList.Add(ingredientToRemove);
            
            using (FoodDbContext context = new())
            {
                await new IngredientsRepo(context).removeIngredientById(ingredientToRemove.IngredientId);
                await context.SaveChangesAsync();
            }
            
            lvIngredients.Items.Remove(lvi);
        }

        // Returns to recipe window, passing the logged in user
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RecipeWindow recwin = new(loggedInUser);
            recwin.Show();
            this.Close();
        }

        /* This method is removed in practice as it cannot be reached. I had to
         * move it to RecipeWindow since i misread the instructions. */
        private async void btnRemoveRecipe_Click(object sender, RoutedEventArgs e)
        {
            using (FoodDbContext context = new())
            {
                new RecipesRepo(context).RemoveRecipe(recipe);
                await context.SaveChangesAsync();
                
                MessageBox.Show("Removed this recipe");
                
                RecipeWindow recwin = new(loggedInUser);
                recwin.Show();
                this.Close();
            }
        }
    }
}
