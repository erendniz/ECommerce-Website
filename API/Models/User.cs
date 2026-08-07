namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool Confirmed { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Address> Addresses { get; set; } = new List<Address>();
    }
}
