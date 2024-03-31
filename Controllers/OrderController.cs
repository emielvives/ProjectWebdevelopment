using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_ASP_NET.Areas.Identity.Data;
using Project_ASP_NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Project_ASP_NET.Controllers
{
    public class OrderController : Controller
    {
        private readonly SusContext _dbContext;
        private readonly UserManager<SUser> _userManager;

        public OrderController(SusContext dbContext, UserManager<SUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult BestellingPlaatsen()
        {
            // Retrieve the current user
            var currentUser = _userManager.GetUserAsync(User).Result;

            // Retrieve products from the database
            var products = _dbContext.Products.ToList();

            // Map products to the view model
            var orderViewModel = products.Select(product => new OrderViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = 0
            }).ToList();

            // Pass the current user to the view
            ViewData["CurrentUser"] = currentUser;

            return View(orderViewModel);
        }

        [HttpPost]
        public IActionResult PlaceOrder(List<OrderViewModel> orderViewModel)
        {
            // Retrieve the current user
            var currentUser = _userManager.GetUserAsync(User).Result;

            // Create a new order
            var order = new Order
            {
                OrderPlaced = DateTime.Now,
                CustomerId = currentUser.Id, // Assign the current user's ID to the order
                SUser = currentUser, // Assign the current user to the order
                OrderDetails = new List<OrderDetail>()
            };

            // Save the order to the database to get a valid OrderId
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            foreach (var item in orderViewModel)
            {
                if (item.Quantity > 0)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.Id,
                        Quantity = item.Quantity,
                        OrderId = order.Id // Use the valid OrderId obtained from the saved order
                    };
                    order.OrderDetails.Add(orderDetail);
                }
            }

            // Save the updated order details
            _dbContext.SaveChanges();

            // Redirect to the order confirmation page
            return RedirectToAction("OrderConfirmation");
        }
        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Samenvatting()
        {
            var orderSummary = _dbContext.OrderDetails
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new SummaryViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantityOrdered = g.Sum(od => od.Quantity),
                    TotalPrice = g.Sum(od => od.Quantity * od.Product.Price)
                })
                .ToList();

            decimal totalPriceForAllProducts = orderSummary.Sum(s => s.TotalPrice);

            ViewBag.TotalPriceForAllProducts = totalPriceForAllProducts;

            return View(orderSummary);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult BestellingenBekijken()
        {
            // Retrieve orders with their details and related customer information
            var orders = _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.SUser)
                .ToList();

            // Map orders to view model
            var orderViewModels = orders.Select(o => new OrderDetailViewModel
            {
                OrderId = o.Id,
                CustomerName = o.SUser.UserName,
                TotalPrice = o.OrderDetails.Sum(od => od.Quantity * od.Product.Price),
                OrderProducts = o.OrderDetails.Select(od => new OrderViewModel
                {
                    Id = od.ProductId,
                    Name = od.Product.Name,
                    Price = od.Product.Price,
                    Quantity = od.Quantity
                }).ToList(),
                TotalPricePerProduct = o.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(od => od.Quantity * od.Product.Price))
            }).ToList();

            return View(orderViewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BestellingenBeheren()
        {
            // Retrieve orders with their details and related customer information
            var orders = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.SUser)
                .ToListAsync();

            // Map orders to view model
            var orderViewModels = orders.Select(o => new OrderDetailViewModel
            {
                OrderId = o.Id,
                CustomerName = o.SUser.UserName,
                TotalPrice = o.OrderDetails.Sum(od => od.Quantity * od.Product.Price),
                OrderProducts = o.OrderDetails.Select(od => new OrderViewModel
                {
                    Id = od.ProductId,
                    Name = od.Product.Name,
                    Price = od.Product.Price,
                    Quantity = od.Quantity
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges(Dictionary<int, List<OrderViewModel>> orderViewModels)
        {
            foreach (var orderId in orderViewModels.Keys)
            {
                // Retrieve the order from the database
                var order = await _dbContext.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return NotFound(); // Order not found
                }

                // Update order details based on form submission
                foreach (var viewModel in orderViewModels[orderId])
                {
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == viewModel.Id);
                    if (orderDetail != null)
                    {
                        // Update quantity
                        orderDetail.Quantity = viewModel.Quantity;
                    }
                }
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Redirect to the same page (or another page) after saving changes
            return RedirectToAction("BestellingenBeheren");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSelected(List<int> selectedOrders)
        {
            if (selectedOrders == null || !selectedOrders.Any())
            {
                return BadRequest("No orders selected for removal.");
            }

            // Retrieve the selected orders from the database
            var ordersToRemove = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .Where(o => selectedOrders.Contains(o.Id))
                .ToListAsync();

            // Check if any orders were found
            if (ordersToRemove == null || !ordersToRemove.Any())
            {
                return NotFound("No orders found for the selected IDs.");
            }

            // Remove the selected orders and their associated details
            _dbContext.Orders.RemoveRange(ordersToRemove);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Redirect to the same page (or another page) after removing orders
            return RedirectToAction("BestellingenBeheren");
        }
    }
}
