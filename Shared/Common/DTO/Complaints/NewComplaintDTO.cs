using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class NewComplaintDTO
    {
        [Required]
        public string content { get; set; }
        [Required]
        public int orderId { get; set; }
    }
}
