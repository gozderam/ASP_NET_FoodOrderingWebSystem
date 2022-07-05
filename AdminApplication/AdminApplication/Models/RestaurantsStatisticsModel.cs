using Common.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.Models
{
    public class RestaurantsStatisticsModel
    {
        public List<RestaurantStatistic> statistics { get; }
        public RestaurantsStatisticsModel(OrderModel[] orders)
        {
            this.statistics = new List<RestaurantStatistic>();

            if (orders == null)
                return;

            Dictionary<int, List<OrderModel>> dictionary = new Dictionary<int, List<OrderModel>>();

            foreach (var o in orders)
            {
                if (dictionary.ContainsKey(o.Restaurant.id))
                {
                    dictionary[o.Restaurant.id].Add(o);
                }
                else
                {
                    List<OrderModel> ordersList = new List<OrderModel>();
                    ordersList.Add(o);
                    dictionary.Add(o.Restaurant.id, ordersList);
                }
            }

            foreach(var i in dictionary)
            {
                statistics.Add(new RestaurantStatistic(i.Value));
            }     
        }
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public PaymentMethodDTO PaymentMethod { get; set; }
        public OrderStateDTO State { get; set; }
        public DateTime Date { get; set; }
        public AddressDTO Address { get; set; }
        public double OriginalPrice { get; set; }
        public double FinalPrice { get; set; }
        public CustomerADTO Customer { get; set; }
        public RestaurantDTO Restaurant { get; set; }
    }

    public class RestaurantStatistic
    {
        public int RestaurantId { get; }
        public string RestaurantName { get; }
        public int Orders { get; }
        public int CompletedOrders { get; }
        public int OrdersPaidByCard { get; }
        public int OrdersPaidByTransfer { get; }
        public int OrdersWithDiscountCode { get; }
        public double Income { get; }

        public RestaurantStatistic(List<OrderModel> orders)
        {
            this.RestaurantId = orders[0].Restaurant.id;
            this.RestaurantName = orders[0].Restaurant.name;
            this.Orders = orders.Count;
            this.CompletedOrders = orders.Where(o => o.State == OrderStateDTO.completed).Count();
            this.OrdersPaidByCard = orders.Where(o => o.PaymentMethod == PaymentMethodDTO.card).Count();
            this.OrdersPaidByTransfer = orders.Where(o => o.PaymentMethod == PaymentMethodDTO.transfer).Count();
            this.OrdersWithDiscountCode = orders.Where(o => o.FinalPrice != o.OriginalPrice).Count();
            this.Income = orders.Where(o => o.State == OrderStateDTO.completed).Sum(o => o.FinalPrice);
        }
    }
}
