using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsOpened { get; set; }
        public string Answer { get; set; }
        public Client Client { get; set; }
        public int OrderForeignKey { get; set; }
        public Order Order { get; set; }
        public RestaurantEmployee AttendingEmployee { get; set; }
    }
}
