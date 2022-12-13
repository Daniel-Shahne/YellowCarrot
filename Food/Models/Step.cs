using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowCarrot.Food.Models
{
    internal class Step
    {
        public int StepId { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        public int Order { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
