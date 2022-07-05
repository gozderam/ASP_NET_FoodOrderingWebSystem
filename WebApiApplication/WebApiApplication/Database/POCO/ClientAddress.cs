using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class ClientAddress
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public List<Order> DeliveredOrders { get; set; }
        public List<Client> Clients { get; set; }

    }
}
