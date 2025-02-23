using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _usersRepository;

		public UserController(IUserRepository usersRepository)
		{
			_usersRepository = usersRepository;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var viewModel = new UserViewModel();
			LoadIndexViewData(viewModel);
			return View(viewModel);
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

		[HttpPost]
		public IActionResult PageSizes(UserGridData currentRoute)
		{
			return RedirectToAction("Index", currentRoute.ToDictionary());
		}

		private void LoadIndexViewData(UserViewModel vm)
		{
			vm.Users = _usersRepository.List(new QueryOptions<TicketAppUser>
			{
				OrderBy = u => u.LastName ?? string.Empty
			});

			vm.UserRoles = _usersRepository.GetUsersAndRoleAsync().Result;
		}
	}
}
