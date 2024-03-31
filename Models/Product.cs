using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_ASP_NET.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
    }
}
