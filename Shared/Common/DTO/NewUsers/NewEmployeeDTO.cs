namespace Common.DTO
{
    public class NewEmployeeDTO
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public bool isRestaurateur { get; set; }
        public int? restaurantId { get; set; }
    }
}
