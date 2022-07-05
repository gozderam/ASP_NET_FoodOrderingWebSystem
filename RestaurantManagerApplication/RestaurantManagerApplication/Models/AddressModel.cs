using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagerApplication.Models
{
    public class AddressModel
    {
        public string City { get; set; }
        public string Street { get; set; }
        [RegularExpression(@"\d{2}\-\d{3}", ErrorMessage = "Post code should be in format 00-000")]
        public string PostalCode { get; set; }
    }
}
