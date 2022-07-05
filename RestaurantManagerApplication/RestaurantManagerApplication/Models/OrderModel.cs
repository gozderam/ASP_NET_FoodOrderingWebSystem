using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class OrderModel
    {
        public enum PaymentMethod
        {
            card,
            transfer
        }

        public enum OrderState
        {
            unrealized,
            pending,
            completed,
            cancelled
        }
        public int id { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public OrderState state { get; set; }
        public DateTime date { get; set; }
        public AddressModel address { get; set; }
        public double originalPrice { get; set; }
        public double finalPrice { get; set; }
        public DiscountCodeModel discountcode { get; set; }
        public RestaurantModel restaurant { get; set; }
        public MenuPositionModel[] positions { get; set; }
        public EmployeeModel employee { get; set; }
    }
}

