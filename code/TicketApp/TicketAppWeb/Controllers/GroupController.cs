using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
	public class GroupController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
	}
}
