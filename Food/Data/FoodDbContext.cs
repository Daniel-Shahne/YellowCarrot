using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Food.Models;

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
            /* En ingredient ska inte kunna tages bort?
             * TODO OnDelete säger bara vad som ska hända
             * med dependants när principal tages bort, men
             * inte tvärtom?
             * Definierar Recipe-Ingredient Many-one */
            mb.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .OnDelete(DeleteBehavior.Cascade);

             /* TODO enda delete behavior är att alla rader i
              * kombinationstabellen med en viss recipe eller tag
              * tages bort om den recipe eller tag tages bort?
              * Definierar Recipe-Tags Many-Many */
            mb.Entity<Recipe>()
                .HasMany(r => r.Tags)
                .WithMany(t => t.Recipes);

            /* Definierar Recipe-Step One-Many */
            mb.Entity<Step>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.Steps)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
