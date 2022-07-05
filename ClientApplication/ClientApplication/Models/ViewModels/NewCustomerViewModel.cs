using System.ComponentModel.DataAnnotations;

namespace ClientApplication.Models.ViewModels
{
    public class NewCustomerViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100,MinimumLength = 1)]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 1)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string City { get; set; }
        [DataType(DataType.Text)]
        public string Street { get; set; }

        [RegularExpression(@"\d{2}\-\d{3}", ErrorMessage ="Post code should be in format 00-000")]
        [DataType(DataType.Text)]
        public string Postcode { get; set; }
    }
}
