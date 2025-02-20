using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
	}
}
