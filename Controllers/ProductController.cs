using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Project_ASP_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Project_ASP_NET.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_ASP_NET.Controllers
{
    public class ProductController : Controller
    {
        private readonly SusContext _dbContext;

        public ProductController(SusContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Prijzen()
        {
            var products = _dbContext.Products.ToList();

            var productViewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();

            return View(productViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrices(Dictionary<int, ProductViewModel> productViewModels)
        {
            foreach (var productId in productViewModels.Keys)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return NotFound(); // Product not found
                }

                // Update price based on form submission
                var viewModel = productViewModels[productId];
                product.Price = viewModel.Price;
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Redirect to the same page (or another page) after updating prices
            return RedirectToAction("Prijzen");
        }
    }
}
