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

		[HttpPost]
		public async Task<IActionResult> CreateUser(UserViewModel vm)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _usersRepository.CreateUser(vm.User, vm.SelectedRole);
					_usersRepository.Save();
				}
				catch (Exception e)
				{
					TempData["ErrorMessage"] = $"Sorry, {e.Message}";
					return RedirectToAction("Index", "User");
				}

				TempData["SuccessMessage"] = $"{vm.User.FullName}'s account added successfully.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Sorry, user creation failed.";
			}

			return RedirectToAction("Index", "User");
		}

		[HttpGet]
		public IActionResult EditUser(string id)
		{
			var user = _usersRepository.Get(id);

			if (user == null)
			{
				TempData["ErrorMessage"] = "Sorry, user not found}";
				return RedirectToAction("Index", "User");
			}

			var viewModel = new UserViewModel
			{
				User = user
			};

			LoadIndexViewData(viewModel);

			return View(viewModel);
		}

		[HttpGet]
		public IActionResult DeleteUser(string id)
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

			vm.Roles = _usersRepository.GetRolesAsync().Result;
			vm.UserRoles = _usersRepository.GetUserRolesAsync().Result;
		}
	}
}
