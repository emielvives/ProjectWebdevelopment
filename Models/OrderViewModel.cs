// OrderViewModel.cs
namespace Project_ASP_NET.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
