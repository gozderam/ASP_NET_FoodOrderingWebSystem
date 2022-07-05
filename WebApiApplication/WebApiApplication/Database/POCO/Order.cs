using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class Order
    {
        public int Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderState OrderState { get; set; }
        public DateTime Date { get; set; }
        public double OriginalPrice { get; set; }
        public double FinalPrice { get; set; }
        public ClientAddress Address { get; set; }
        public DiscountCode DiscountCode { get; set; }
        public Client Client { get; set; }
        public List<Order_MenuPosition> OrdersMenuPositions { get; set; }
        public Restaurant Restaurant { get; set; }
        public RestaurantEmployee ResponsibleEmployee { get; set; }
        public Complaint Complaint { get; set; }
    }

    public enum PaymentMethod
    {
        Card,
        Transfer
    }

    public enum OrderState
    {
        Unrealized,
        Pending,
        Completed,
        Cancelled
    }
}

