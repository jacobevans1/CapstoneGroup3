using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models;

namespace TicketAppWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly TicketAppContext _context;

		public HomeController(TicketAppContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var numbersList = _context.Numbers.OrderByDescending(n => n.Id).ToList();
			ViewBag.FV = numbersList.FirstOrDefault()?.Value ?? 0;
			
			return View(numbersList);
		}

		[HttpPost]
		public IActionResult Index(double value)
		{
			if (value < 0)
			{
				ViewBag.FV = 0;
				ModelState.AddModelError("Value", "Please enter a valid number.");
			}
			else
			{
				var sentNumber = new Number(value);
				_context.Numbers.Add(sentNumber);
				_context.SaveChanges();
			}

			var numbersList = _context.Numbers.OrderByDescending(n => n.Id).ToList();
			ViewBag.FV = value;

			return View(numbersList);
		}
	}
}