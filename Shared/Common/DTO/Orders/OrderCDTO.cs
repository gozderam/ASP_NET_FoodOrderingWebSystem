using System;
using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class OrderCDTO
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
        public RestaurantCDTO restaurant { get; set; }
        public PositionFromMenuDTO[] positions { get; set; }
    }
}
