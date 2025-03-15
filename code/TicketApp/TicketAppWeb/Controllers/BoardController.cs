using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
	public class BoardController : Controller
	{
		public IActionResult Index()
		{
			return View("Index");
		}
	}
}
