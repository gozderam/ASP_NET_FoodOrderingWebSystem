using System;
using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class OrderADTO
    {
        public int id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethodDTO paymentMethod { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStateDTO state { get; set; }
        public DateTime date { get; set; }
        public AddressDTO address { get; set; }
        public double originalPrice { get; set; }
        public double finalPrice { get; set; }
        public CustomerADTO customer{ get; set; }
        public RestaurantDTO restaurant { get; set; }
    }
}
