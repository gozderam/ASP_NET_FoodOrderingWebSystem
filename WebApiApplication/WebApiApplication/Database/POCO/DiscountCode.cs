using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public double Percent { get; set; }
        public string Code { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<Order> Orders { get; set; }
        /// <summary>
        /// If the discount code applies to all restaurants (then AppliedToRestaurant == null).
        /// </summary>
        public bool AppliesToAllRestaurants { get; set; }
        /// <summary>
        /// Restaurant the discount code applies to (if AppliesToAllRestaurant == false, otherwise null).
        /// </summary>
        public Restaurant AppliedToRestaurant { get; set; }
    }
}
