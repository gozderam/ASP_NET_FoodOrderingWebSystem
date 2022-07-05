using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class DiscountCodeDTO
    {
        [Required]
        public int id { get; set; }

        [Required]
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
