namespace Common.DTO
{
    public class SectionDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int restaurantId { get; set; }
        public PositionFromMenuDTO[] positions { get; set; }
    }
}
