﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Users.Models;

namespace YellowCarrot.Food.Models
{
    internal class Recipe
    {
        public int RecipeId { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public List<Step> Steps = new();

        public List<Tag> Tags = new();

        public List<Ingredient> Ingredients = new();
        
        public User Author = null!;
    }
}
