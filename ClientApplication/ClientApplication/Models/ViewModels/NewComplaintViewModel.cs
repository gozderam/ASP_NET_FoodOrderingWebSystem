using System.ComponentModel.DataAnnotations;

namespace ClientApplication.Models.ViewModels
{
    public class NewComplaintViewModel
    {
        [Required]
        public int orderId { get; set; }

        [Required]
        public string content { get; set; }
    }
}
