using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class ComplaintDTO
    {
        [Required]
        public int id { get; set; }

        [Required]
        public string content { get; set; }

        public string response { get; set; }

        [Required]
        public bool open { get; set; }

        [Required]
        public int customerId { get; set; }

        [Required]
        public int orderId { get; set; }
    }
}
