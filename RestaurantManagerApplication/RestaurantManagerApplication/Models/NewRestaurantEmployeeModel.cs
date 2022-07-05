using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagerApplication.Models
{
    public class NewRestaurantEmployeeModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required.")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, MinimumLength = 1)]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsRestaurateur { get; set; }
        public int? RestaurantId { get; set; }
    }
}
