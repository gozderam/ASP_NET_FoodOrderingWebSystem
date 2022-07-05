namespace Common.DTO
{
    public class ReviewDTO
    {
        public int id { get; set; }
        public string content { get; set; }
        public double rating { get; set; }
        public int customerId { get; set; }
        public int restaurantId { get; set; }
    }
}
