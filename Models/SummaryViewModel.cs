namespace Project_ASP_NET.Models
{
    public class SummaryViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int TotalQuantityOrdered { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
