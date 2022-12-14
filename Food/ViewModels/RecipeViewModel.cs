using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using YellowCarrot.Food.Models;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;
using YellowCarrot.Users.Services;

namespace YellowCarrot.Food.ViewModels
{
    internal class RecipeViewModel
    {
        Recipe recipe;
        User? author;

        /* CTOR not meant to be run to instantiate. The Async method
         * should be run which instantiates the object. */
        private RecipeViewModel(Recipe recipe, User? author)
        {
            this.recipe = recipe;
            this.author = author;
        }

        // Runs CTOR and instantiates async. Also retrieves an author
        public static async Task<RecipeViewModel> InstantiateAsync(Recipe recipe)
        {
            User? localAuthor;
            using (UsersDbContext context = new())
            {
                localAuthor = await new UserRepo(context).GetUserByIdAsync(recipe.UserId);
            }

            RecipeViewModel rvm = new(recipe, localAuthor);
            return rvm;
        }



        /* Needs to be supplied an author (user).
         * The entire recipe object is in the LVI's tag */
        internal ListViewItem createLVI()
        {
            ListViewItem lvi = new();
            lvi.Tag = recipe;
            string authorname = author != null ? author.Username : "unknown";
            lvi.Content = $"{recipe.Name} created by {authorname}";
            return lvi;
        }
    }
}
