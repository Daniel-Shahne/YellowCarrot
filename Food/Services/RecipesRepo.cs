using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Food.Data;
using YellowCarrot.Food.Models;

namespace YellowCarrot.Food.Services
{
    internal class RecipesRepo
    {
        private FoodDbContext context;

        public RecipesRepo(FoodDbContext context)
        {
            this.context = context;
        }

        /* Gets all recipes and ALL their related data */
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Tags)
                .Include(r => r.Steps)
                .ToListAsync();
        }

        /* Searches by recipe name and/or tag.
         * Also gets ALL their related data */
        public async Task<List<Recipe>> SearchRecipesAsync(string searchWord)
        {
            return await context.Recipes
                .Where(r => r.Name.Contains(searchWord) || r.Tags.Any(t => t.TagName.Contains(searchWord)))
                .Include(r => r.Ingredients)
                .Include(r => r.Tags)
                .Include(r => r.Steps)
                .ToListAsync();
        }

        /* Simply adds a recipe */
        public async Task CreateRecipeAsync(Recipe newRecipe)
        {
            await context.Recipes.AddAsync(newRecipe);
        }

        /* Removes a recipe. This however isnt async,
         * and apparently only queues the delete operation
         * which SaveChangesAsync can take care of. */
        public void RemoveRecipe(Recipe newRecipe)
        {
            context.Recipes.Remove(newRecipe);
        }

        public async Task SaveRecipesChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
