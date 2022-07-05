using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class OrderViewModel
    {
        public OrderModel[] PendingOrders { get; set; }
        public OrderModel[] UnrealisedOrders { get; set; }
    }
}
