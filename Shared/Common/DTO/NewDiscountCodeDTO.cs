using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class NewDiscountCodeDTO
    {
        [Required]
        [Range(1.0, 99.0)]
        public double percent { get; set; }

        [Required]
        public string code { get; set; }

        public int? restaurantId { get; set; }

        [Required]
        public string dateFrom { get; set; }

        [Required]
        public string dateTo { get; set; }
    }
}
