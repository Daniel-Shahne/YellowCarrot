using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Food.Models;
using YellowCarrot.Food.Services;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;
using YellowCarrot.Users.Services;

namespace YellowCarrot.Food.Data
{
    internal class FoodDbContext : DbContext
    {
        public FoodDbContext()
        {

        }

        public FoodDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            ob.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=YellowCarrotFoodDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // ------------ RELATIONS --------------

            /* Defines Recipe-Ingredient many-one */
            mb.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .OnDelete(DeleteBehavior.Cascade);

            /* Defines Recipe-Tags Many-Many */
            mb.Entity<Recipe>()
                .HasMany(r => r.Tags)
                .WithMany(t => t.Recipes);

            /* Defines Recipe-Step One-Many */
            mb.Entity<Step>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.Steps)
                .OnDelete(DeleteBehavior.Cascade);




            // ---------- SEEDING -------------

            /* Tags are shared across recipes*/
            mb.Entity<Tag>().HasData(
                new Tag()
                {
                    TagId = 1,
                    TagName = "Italian food"
                },
                new Tag()
                {
                    TagId = 2,
                    TagName = "Meat"
                },
                new Tag()
                {
                    TagId = 3,
                    TagName = "Vegetarian"
                }
            );

            mb.Entity<Ingredient>().HasData(
                /* First recipe's ingredients */
                new Ingredient()
                {
                    IngredientId = 1,
                    Name = "Tomatssås",
                    Quantity = 1,
                    QuantityUnit = "st",
                    RecipeId = 1
                },
                new Ingredient()
                {
                    IngredientId = 2,
                    Name = "Örter",
                    Quantity = 1,
                    QuantityUnit = "burk",
                    RecipeId = 1
                },
                new Ingredient()
                {
                    IngredientId = 3,
                    Name = "Spaghetti",
                    Quantity = 500,
                    QuantityUnit = "gram",
                    RecipeId = 1
                },
                /* Second recipe's ingredients */
                new Ingredient()
                {
                    IngredientId = 4,
                    Name = "Pizza Dough",
                    Quantity = 1,
                    QuantityUnit = "st",
                    RecipeId = 2
                },
                new Ingredient()
                {
                    IngredientId = 5,
                    Name = "Mozzarella",
                    Quantity = 1,
                    QuantityUnit = "kilogram",
                    RecipeId = 2
                },
                new Ingredient()
                {
                    IngredientId = 6,
                    Name = "Italian spices",
                    Quantity = 100,
                    QuantityUnit = "gram",
                    RecipeId = 2
                }
            );

            mb.Entity<Step>().HasData(
                /* First recipe's steps */
                new Step()
                {
                    StepId = 1,
                    Description = "Blanda ihop såsen med örter, och koka spaghettin",
                    Order = 1,
                    RecipeId = 1
                },
                new Step()
                {
                    StepId = 2,
                    Description = "Häll såsen på spaghettin",
                    Order = 2,
                    RecipeId = 1
                },
                /* Second recipe's steps */
                new Step()
                {
                    StepId = 3,
                    Description = "Toppa pizzadegen",
                    Order = 1,
                    RecipeId = 2
                },
                new Step()
                {
                    StepId = 4,
                    Description = "Sätt i ugnen",
                    Order = 2,
                    RecipeId = 2
                }
            );

            mb.Entity<Recipe>().HasData(
                new Recipe()
                {
                    RecipeId = 1,
                    Name = "Spaghetti med köttfärssås",
                    UserId = 1
                },
                new Recipe()
                {
                    RecipeId = 2,
                    Name = "Margherita",
                    UserId = 2
                }
            );
        }
    }
}
