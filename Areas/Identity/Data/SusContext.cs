using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_ASP_NET.Areas.Identity.Data;
using Project_ASP_NET.Models;

namespace Project_ASP_NET.Areas.Identity.Data;

public class SusContext : IdentityDbContext<SUser>
{

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    public SusContext(DbContextOptions<SusContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
