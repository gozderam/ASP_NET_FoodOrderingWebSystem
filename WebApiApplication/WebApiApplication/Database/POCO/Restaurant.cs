using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public RestaurantState State { get; set; }
        public double Rate { get; set; }
        public double ToPay { get; set; }
        public double TotalPayment { get; set; }
        public DateTime DateOfJoining { get; set; }
        public int AddressForeignKey { get; set; }
        public RestaurantAddress Address { get; set; }
        public List<MenuSection> MenuSections { get; set; }
        public List<RestaurantEmployee> RestaurantEmployees { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        public List<DiscountCode> DiscountCodes { get; set; }
        public List<Client> FavouriteForClients { get; set; }
    }

    public enum RestaurantState
    {
        Disabled,
        Active,
        Deactivated,
        Blocked,
    }
}
