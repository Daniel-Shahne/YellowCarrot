using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YellowCarrot.Food.Data;
using YellowCarrot.Food.Models;
using YellowCarrot.Food.Services;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;
using YellowCarrot.Users.Services;

namespace YellowCarrot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }






        /* This was just to test if EFC works as expected (successful).
         * TODO need to turn this into a proper seeding */
        private async Task seedDataAsync()
        {
            Tag italianTag = new()
            {
                TagName = "Italiansk mat"
            };
            Tag kottTag = new()
            {
                TagName = "Kött"
            };

            Ingredient recipe1ingredient1 = new()
            {
                Name = "Tomatssås",
                Quantity = 1
            };
            Ingredient recipe1ingredient2 = new()
            {
                Name = "Spaghetti",
                Quantity = 500
            };
            Step recipe1step1 = new()
            {
                Description = "Blanda såsen och koka spaghettin",
                Order = 1
            };
            Step recipe1step2 = new()
            {
                Description = "Häll såsen på spaghettin",
                Order = 2
            };
            User? recipe1author;
            using (UsersDbContext uDC = new())
            {
                recipe1author = await new UserRepo(uDC).GetUserByUsernameAsync("user");
            }
            Recipe recipe1 = new()
            {
                Name = "Spaghetti köttfärssås",
                Steps = new() { recipe1step1, recipe1step2 },
                Ingredients = new() { recipe1ingredient1, recipe1ingredient2 },
                Tags = new() { kottTag, italianTag },
                UserId = recipe1author
            };
            using (FoodDbContext fDC = new())
            {
                RecipesRepo rRepo = new(fDC);
                await rRepo.CreateRecipe(recipe1);
                await rRepo.SaveRecipesChangesAsync();
            }
        }
    }
}
