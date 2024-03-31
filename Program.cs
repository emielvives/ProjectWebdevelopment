using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Project_ASP_NET.Areas.Identity.Data;
using Project_ASP_NET.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SusContextConnection") ?? throw new InvalidOperationException("Connection string 'SusContextConnection' not found.");

builder.Services.AddDbContext<SusContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<SUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SusContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
