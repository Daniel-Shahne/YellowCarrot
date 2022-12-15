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
    /// Interaction logic for AddRecipeWindow.xaml
    /// </summary>
    public partial class AddRecipeWindow : Window
    {
        private Regex quantityRegex = new Regex(@"^(\d{1,5})\s(\w{1,15})$");
        private User loggedInUser;
        private bool recipeNameLenOk;
        private bool tagNameLenOk;
        private bool ingredientNameLenOk;
        private bool quantityTxbOk;
        private bool stepOrderOk;
        private bool descriptionLenOk;
        private int highestStepOrderNr = 0;

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
         * lvi's Tag property as an object. 
         * Cant add a Tag multiple times */
        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            if (tagNameLenOk)
            {
                foreach (ListViewItem lvii in lvTags.Items)
                {
                    Tag tag = (Tag)lvii.Tag;
                    if (txbTagName.Text.Equals(tag.TagName))
                    {
                        MessageBox.Show("This tag has already been added");
                        return;
                    }
                }
                
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

        // Removes a tag
        private void btnRemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (lvTags.SelectedItem != null)
            {
                lvTags.Items.Remove(lvTags.SelectedItem);
            }
            else
            {
                MessageBox.Show("Need to select a tag to remove");
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

        private void txbOrderNr_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Checks if OrderNr input even is a valid number, not two above highest current order, and above 0
            if (Int32.TryParse(txbOrderNr.Text, out int quantity) && quantity < highestStepOrderNr + 2 && quantity > 0)
            {
                stepOrderOk = true;
                txbOrderNr.Foreground = Brushes.Black;
            }
            else
            {
                stepOrderOk = false;
                txbOrderNr.Foreground = Brushes.Red;
            }
        }

        /* Behemoth of a method that can add a step in several ways.
         * 1. Simply adding a new step at the end of the order 
         * 2. Inserting somewhere inside order and having to rearrange orders*/
        private void btnAddStep_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new();

            if (!descriptionLenOk) errors.Add("Step description need to be between 10 and 1000 characters long");
            if (!stepOrderOk) errors.Add("Order number needs to be above 0, and can only be one step above the highest");

            // If any errors are found, display and cancel add
            if (errors.Count > 0)
            {
                string errorMsg = String.Join("\n", errors);
                MessageBox.Show(errorMsg, "Cant add Step");
                return;
            }
            // If trying to append to end of orders
            else if (Int32.Parse(txbOrderNr.Text) == highestStepOrderNr + 1)
            {
                Step newStep = new()
                {
                    Description = txbStepDescription.Text,
                    Order = Int32.Parse(txbOrderNr.Text)
                };

                ListViewItem lvi = new();
                lvi.Tag = newStep;
                lvi.Content = $"Step {newStep.Order}: {newStep.Description}";
                lvSteps.Items.Add(lvi);

                highestStepOrderNr = newStep.Order;
                txbOrderNr.Text = $"{newStep.Order + 1}";
            }
            // If trying to insert in the middle of the steps
            else if (Int32.Parse(txbOrderNr.Text) <= highestStepOrderNr)
            {
                int newStepIndex = Int32.Parse(txbOrderNr.Text) - 1;

                Step newStep = new()
                {
                    Description = txbStepDescription.Text,
                    Order = Int32.Parse(txbOrderNr.Text)
                };

                // Inserts new lvi at its index
                ListViewItem lvi = new();
                lvi.Tag = newStep;
                lvi.Content = $"Step {newStep.Order}: {newStep.Description}";
                lvSteps.Items.Insert(newStepIndex, lvi);
                
                // Increments highest step order nr
                highestStepOrderNr++;

                // Increases the step number of every step above its index
                for (int i = newStepIndex + 1; i < highestStepOrderNr; i++)
                {
                    ListViewItem lvii = (ListViewItem)lvSteps.Items[i];
                    Step step = (Step)lvii.Tag;
                    step.Order += 1;
                    lvii.Content = $"Step {step.Order}: {step.Description}";
                }
            }
        }
        
        // Description need to be atleast 10 characters long and max 1000
        private void txbStepDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbStepDescription.Text.Length >= 10 && txbStepDescription.Text.Length <= 1000)
            {
                descriptionLenOk = true;
                txbStepDescription.Foreground = Brushes.Black;
            }
            else
            {
                descriptionLenOk = false;
                txbStepDescription.Foreground = Brushes.Red;
            }
        }

        /* Removes a step and modifies order numbers to be correct */
        private void btnRemoveStep_Click(object sender, RoutedEventArgs e)
        {
            if (lvSteps.SelectedItem != null)
            {
                int indexRemovedAt = lvSteps.SelectedIndex;
                lvSteps.Items.Remove(lvSteps.SelectedItem);
                highestStepOrderNr--;

                /* All the items above and AT the index of removal need
                 * to be updated */
                for (int i = indexRemovedAt; i < highestStepOrderNr; i++)
                {
                    ListViewItem lvii = (ListViewItem)lvSteps.Items[i];
                    Step step = (Step)lvii.Tag;
                    step.Order -= 1;
                    lvii.Content = $"Step {step.Order}: {step.Description}";
                }

            }
            else MessageBox.Show("Need to select an item to remove");
        }

        /* Adds an ingredient if the input fields are ok according
         * to the field variables, which are modified by their corresponding
         * controls text changed events. */
        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
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
            }
        }

        // Simply removes an ingredient from the lv
        private void btnRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (lvIngredients.SelectedItem is not null)
            {
                lvIngredients.Items.Remove(lvIngredients.SelectedItem);
            }
            else MessageBox.Show("Select an ingredient to remove first");
        }


        private async void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new();

            if (!ingredientNameLenOk) errors.Add("Ingredient name needs to be between 3 and 20 characters long");
            if (lvTags.Items.Count == 0) errors.Add("Recipe needs tag(s)");
            if (lvIngredients.Items.Count == 0) errors.Add("What kind of recipe has no ingredients?");
            if (lvSteps.Items.Count == 0) errors.Add("If we didnt want steps we wouldn't be looking for recipes");

            if (errors.Count > 0)
            {
                string errorMsg = String.Join("\n", errors);
                MessageBox.Show(errorMsg);
            }
            else
            {
                // Lists for all LVI's tag objects
                List<Step> stepsList = new();
                List<Ingredient> ingredientsList = new();
                List<Tag> tagsListPrimitive = new();

                // Getting all LVI tag objects
                foreach (ListViewItem lvi in lvSteps.Items)
                {
                    Step step = (Step)lvi.Tag;
                    stepsList.Add(step);
                }
                foreach (ListViewItem lvi in lvIngredients.Items)
                {
                    Ingredient ingr = (Ingredient)lvi.Tag;
                    ingredientsList.Add(ingr);
                }
                foreach (ListViewItem lvi in lvTags.Items)
                {
                    Tag tag = (Tag)lvi.Tag;
                    tagsListPrimitive.Add(tag);
                }



                /* Tags have to be checked for if they already exist
                 * The two lists then have to be combined somehow*/
                List<Tag> existingTagsList = new();
                using (FoodDbContext context = new())
                {
                    existingTagsList = await new TagsRepo(context).GetMatchingTagsByName(tagsListPrimitive);
                }



                Recipe newRecipe = new()
                {
                    Name = txbRecipeName.Text,
                    Steps = stepsList,
                    //Tags = existingTagsList, //TODO Change this
                    Ingredients = ingredientsList,
                    UserId = loggedInUser.UserId
                };


                /* Should try updating recipe with tags afterwards */
                using (FoodDbContext context = new())
                {
                    await new RecipesRepo(context).CreateRecipeAsync(newRecipe);
                    await context.SaveChangesAsync();

                    MessageBox.Show($"Added recipe {newRecipe.Name}!");
                }
            }
        }   
    }
}
