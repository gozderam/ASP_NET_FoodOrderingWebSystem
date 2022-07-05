using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class RestaurantViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public AddressViewModel address { get; set; }
        public RestaurantStateModel state { get; set; }

    }

    public class AddressViewModel
    {
        public string city { get; set; }
        public string street { get; set; }
        public string postCode { get; set; }
    }

    public enum RestaurantStateModel
    {
        Disabled,
        Active,
        Deactivated,
        Blocked
    }
}
