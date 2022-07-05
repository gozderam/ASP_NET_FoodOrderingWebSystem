using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagerApplication.Models
{
    public class NewDiscountCodeModel
    {
        [Required]
        [Range(1, 99)]
        public int Percent { get; set; }

        [Required]
        public string Code { get; set; }

        public int? RestaurantId { get; set; }

        [Required]
        public string DateFrom { get; set; }

        [Required]
        public string DateTo { get; set; }
    }
}
