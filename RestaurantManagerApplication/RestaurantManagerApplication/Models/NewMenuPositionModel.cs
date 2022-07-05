using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagerApplication.Models
{
    public class NewMenuPositionModel
    {
        [Required]
        public string Name { get; set; }
        public int sectionID { get; set; }
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
