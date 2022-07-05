using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class RestaurantDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string contactInformation { get; set; }
        public double rating { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RestaurantStateDTO state { get; set; }
        public double owing { get; set; }
        public double aggregatePayment { get; set; }
        public string dateOfJoining { get; set; }
        public AddressDTO address { get; set; }
    }
}
