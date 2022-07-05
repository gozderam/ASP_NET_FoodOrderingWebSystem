using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AdminApplication.Models
{
    public class NewAdminModel
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 1)]
        public string Email { get; set; }
    }
}
