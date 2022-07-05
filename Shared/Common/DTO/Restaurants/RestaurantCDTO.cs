using System.Text.Json.Serialization;

namespace Common.DTO
{
    public class RestaurantCDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string contactInformation { get; set; }
        public double rating { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RestaurantStateDTO state { get; set; }
        public AddressDTO address { get; set; }

    }
}
