using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInformation { get; set; }
        public double Rating { get; set; }
        public string State { get; set; }
        public double Owing { get; set; }
        public double AggregatePayment { get; set; }
        public string DateOfJoining { get; set; }
        public AddressModel Address { get; set; }
    }
}
