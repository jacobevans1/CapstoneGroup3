using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models;

namespace TaskAppWeb.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			ViewBag.FV = 0;
			return View();
		}
		[HttpPost]
		public IActionResult Index(FutureValueModel model)
		{
			if (ModelState.IsValid)
			{
				ViewBag.FV = model.CalculateFutureValue();
			}
			else
			{
				ViewBag.FV = 0;
			}
			return View(model);
		}
	}
}
