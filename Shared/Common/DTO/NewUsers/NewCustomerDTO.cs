using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class NewCustomerDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string surname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }
        public AddressDTO address {get;set;}
    }
}
