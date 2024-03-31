namespace Project_ASP_NET.Models
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderViewModel>? OrderProducts { get; set; }
        public Dictionary<int, decimal>? TotalPricePerProduct { get; set; }
    }
}
