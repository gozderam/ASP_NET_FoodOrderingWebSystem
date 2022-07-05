using System;

namespace ClientApplication.Models.ViewModels
{
    public class OrderViewModel
    {
        public int id { get; set; }
        public PaymentMethodModel paymentMethod { get; set; }
        public OrderStateModel state { get; set; }
        public DateTime date { get; set; }
        public double originalPrice { get; set; }
        public double finalPrice { get; set; }
        public RestaurantViewModel restaurant { get; set; }
        public MenuPositionViewModel[] positions { get; set; }
    }
    public enum PaymentMethodModel
    {
        Card,
        Transfer
    }

    public enum OrderStateModel
    {
        Unrealized,
        Pending,
        Completed,
        Cancelled
    }
}
