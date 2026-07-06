namespace Shop.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ClientID { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; } = "Не обработан";
        public Client? Client { get; set; }
    }
}