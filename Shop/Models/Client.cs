namespace Shop.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

