using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _usersRepository;
		private static string selectedUserId;
		private static string selectedUsername;

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
					await _usersRepository.CreateUser(vm.User, vm.SelectedRoleName);
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
		public IActionResult GetUserData(string id)
		{
			var user = _usersRepository.Get(id);

			if (user == null)
			{
				TempData["ErrorMessage"] = "Sorry, user not found.";
				return RedirectToAction("Index", "User");
			}

			selectedUserId = id;
			selectedUsername = user.FirstName + user.LastName;

			var userData = new
			{
				firstName = user.FirstName,
				lastName = user.LastName,
				email = user.Email,
				phoneNumber = user.PhoneNumber,
				roleId = _usersRepository.GetUserRolesAsync().Result[user]
			};

			return Json(userData);
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(UserViewModel vm)
		{
			if (ModelState.IsValid)
			{
				try
				{
					vm.User.Id = selectedUserId;
					vm.User.UserName = selectedUsername;
					await _usersRepository.UpdateUser(vm.User, vm.SelectedRoleName);
				
				}
				catch (Exception e)
				{
					TempData["ErrorMessage"] = $"Sorry, {e.Message}";
					return RedirectToAction("Index", "User");
				}

				TempData["SuccessMessage"] = $"{vm.User.FullName}'s account updated successfully.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Sorry, user update failed.";
			}

			return RedirectToAction("Index", "User");
		}

		[HttpGet]
		public IActionResult DeleteUser(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return BadRequest("User ID is required.");
			}

			var user = _usersRepository.Get(id);

			if (user == null)
			{
				return NotFound(new { message = "User not found." });
			}

			var userData = new
			{
				fullName = user.FullName
			};

			return Json(userData);
		}

		[HttpPost]
		public IActionResult DeleteConfirmed(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				TempData["ErrorMessage"] = "Invalid user ID.";
				return RedirectToAction("Index", "User");
			}

			var user = _usersRepository.Get(id);

			if (user == null)
			{
				TempData["ErrorMessage"] = "User not found.";
			}
			else
			{
				try
				{
					_usersRepository.Delete(user);
					_usersRepository.Save();
					TempData["SuccessMessage"] = "User deleted successfully.";
				}
				catch (Exception e)
				{
					TempData["ErrorMessage"] = $"Error deleting user: {e.Message}";
				}
			}

			return RedirectToAction("Index", "User");
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
