using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models
{
    public class UserLoginModel
    {
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 1)]
        public string Email { get; set; }
    }
}
