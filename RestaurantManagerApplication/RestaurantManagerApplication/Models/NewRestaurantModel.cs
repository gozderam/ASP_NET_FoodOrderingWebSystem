using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace RestaurantManagerApplication.Models
{
    public class NewRestaurantModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Contact { get; set; }
        [Required]
        public AddressModel Address { get; set; }
    }

  
}
