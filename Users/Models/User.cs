using EntityFrameworkCore.EncryptColumn.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Food.Models;

namespace YellowCarrot.Users.Models
{
    internal class User
    {
        public int UserId { get; set; }

        [MaxLength(20)]
        public string Username { get; set; } = null!;

        [MaxLength(1000)]
        [EncryptColumn]
        public string Password { get; set; } = null!;
    }
}
