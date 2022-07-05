namespace Common.DTO
{
    public class EmployeeDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public bool isRestaurateur { get; set; }
        public RestaurantDTO restaurant { get; set; }
    }
}
