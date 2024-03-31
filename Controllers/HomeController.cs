using Microsoft.AspNetCore.Mvc;
using Project_ASP_NET.Models;
using System.Diagnostics;

namespace Project_ASP_NET.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult BestellingPlaatsen()
		{
			return View();
		}

		public IActionResult Samenvatting()
		{
			return View();
		}

		public IActionResult BestellingenBekijken()
		{
			return View();
		}

		public IActionResult Prijzen()
		{
			return View();
		}

		public IActionResult AdminCreate()
		{
			return View();
		}

		public IActionResult Gebruikers()
		{
			return View();
		}

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}