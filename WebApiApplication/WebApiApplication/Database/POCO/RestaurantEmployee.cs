using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class RestaurantEmployee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsRestaurateur { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<Order> ResponsibleForOrders { get; set; }
        public List<Complaint> AttendedComplaints { get; set; }
    }
}
