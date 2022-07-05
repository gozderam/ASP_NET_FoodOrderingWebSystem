using System.Collections.Generic;

namespace WebApiApplication.Database.POCO
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public ClientAddress Address { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Complaint> Complaints { get; set; }
        public List<Restaurant> FavouriteRestaurants { get; set; }

    }
}