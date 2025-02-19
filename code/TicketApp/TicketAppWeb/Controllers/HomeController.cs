using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;

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
			return View();
		}
	}
}
