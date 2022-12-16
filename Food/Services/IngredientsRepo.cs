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
    internal class IngredientsRepo
    {
        private FoodDbContext context;

        public IngredientsRepo(FoodDbContext context)
        {
            this.context = context;
        }


        /* Removes an ingredient by its Id */
        public async Task removeIngredientById(int ingredientId)
        {
            Ingredient ingr = await context.Ingredients.FirstAsync(x => x.IngredientId == ingredientId);
            context.Ingredients.Remove(ingr);
        }

        /*  */
        public void removeIngredients(List<Ingredient> ingredientsToRemove)
        {
            context.Ingredients.RemoveRange(ingredientsToRemove);
        }
    }
}
