namespace Common.DTO
{
    public class CustomerADTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public AddressDTO address { get; set; }
    }
}
