using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class CustomerCDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public RestaurantCDTO[] favouriteRestaurants { get; set; }
        public AddressDTO address { get; set; }
    }
}
