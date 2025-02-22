using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
	public class UserController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult CreateUser()
		{
			return View();
		}

		[HttpGet]
		public IActionResult EditUser(int id)
		{
			return View();
		}

		[HttpGet]
		public IActionResult DeleteUser(int id)
		{
			return View();
		}
	}
}
