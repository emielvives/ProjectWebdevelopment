using Project_ASP_NET.Areas.Identity.Data;

namespace Project_ASP_NET.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderPlaced { get; set; }
        public string? CustomerId { get; set; }
        public SUser SUser { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = null!;
    }
}
