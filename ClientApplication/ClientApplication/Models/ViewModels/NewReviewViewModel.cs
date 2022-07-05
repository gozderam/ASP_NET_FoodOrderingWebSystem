using System.ComponentModel.DataAnnotations;

namespace ClientApplication.Models.ViewModels
{
    public class NewReviewViewModel
    {
        [Required]
        public string Content { get; set; }

        [RegularExpression(@"(^[1-4]+(\,[0-9]+)?$)|(^5(\,0)?$)", ErrorMessage = "The rating should be one number between 1 and 5!")]
        [DataType(DataType.Text)]
        public double? Rating { get; set; }

        [Required]
        public int RestaurantId { get; set; }
    }
}
