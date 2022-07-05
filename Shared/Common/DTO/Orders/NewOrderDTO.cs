using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class NewOrderDTO
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethodDTO paymentMethod { get; set; }
        public string date { get; set; }
        public AddressDTO address { get; set; }
        public int? discountcodeId { get; set; }
        public int customerId{ get; set; }
        public int restaurantId { get; set; }
        public int[] positionsIds { get; set; }
    }
}
