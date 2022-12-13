using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowCarrot.Food.Models
{
    internal class Ingredient
    {
        public int IngredientId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
