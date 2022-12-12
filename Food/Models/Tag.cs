using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowCarrot.Food.Models
{
    internal class Tag
    {
        public int TagId { get; set; }

        [MaxLength(100)]
        public string TagName { get; set; } = null!;

        public List<Recipe> Recipes { get; set; } = new();
    }
}
