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
	}
}