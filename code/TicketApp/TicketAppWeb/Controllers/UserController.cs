using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Controllers;

/// <summary>
/// The UserController class represents a controller for user-related actions.
/// </summary>
public class UserController : Controller
{
	private readonly SingletonService _singletonService;
	private readonly IUserRepository _usersRepository;
	private readonly IGroupRepository _groupsRepository;

	private static string? selectedUserId;
	private static string? selectedUsername;

	/// <summary>
	/// Initializes a new instance of the UserController class.
	/// </summary>
	/// <param name="usersRepository"></param>
	public UserController(SingletonService singletonService, IUserRepository usersRepository, IGroupRepository groupRepository)
	{
		_singletonService = singletonService;
		_usersRepository = usersRepository;
		_groupsRepository = groupRepository;
	}

	/// <summary>
	/// Displays the user management index view.
	/// </summary>
	[HttpGet]
	public IActionResult Index(string? userName)
	{
		var viewModel = new UserViewModel();

		viewModel.UserNameSearchString = userName;
		viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

		LoadIndexViewData(viewModel);

		return View(viewModel);
	}

	/// <summary>
	/// Displays the user creation view.
	/// </summary>
	[HttpGet]
	public IActionResult CreateUser()
	{
		return View();
	}

	/// <summary>
	/// Creates a new user.
	/// </summary>
	/// <param name="vm"></param>
	[HttpPost]
	public async Task<IActionResult> CreateUser(UserViewModel vm)
	{
		if (ModelState.IsValid)
		{
			try
			{
				await _usersRepository.CreateUser(vm.User, vm.SelectedRoleName!);
				_usersRepository.Save();
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = $"Sorry, user update failed.";
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

	/// <summary>
	/// Retrieves the user data for editing.
	/// </summary>
	/// <param name="id"></param>
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

	/// <summary>
	/// Displays the user edit view.
	/// </summary>
	/// <param name="vm"></param>
	[HttpPost]
	public async Task<IActionResult> EditUser(UserViewModel vm)
	{
		if (ModelState.IsValid)
		{
			try
			{
				vm.User.Id = selectedUserId!;
				vm.User.UserName = selectedUsername;
				await _usersRepository.UpdateUser(vm.User, vm.SelectedRoleName!);
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = $"Sorry, user update failed.";
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

	/// <summary>
	/// Retrieves the user data for deletion.
	/// </summary>
	/// <param name="id"></param>
	[HttpGet]
	public IActionResult DeleteUser(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return BadRequest("User ID is required.");
		}

		var user = _usersRepository.Get(id);

		var userData = new
		{
			fullName = user?.FullName
		};

		return Json(userData);
	}

	/// <summary>
	/// Deletes a user.
	/// </summary>
	/// <param name="id"></param>
	[HttpPost]
	public async Task<IActionResult> DeleteConfirmed(string id)
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
			var affectedGoroups = await GetConflictingGroupsForUser(user);

			try
			{
				if (affectedGoroups.Any())
				{
					TempData["ErrorMessage"] = $"Cannot delete user {user.FullName} because they manages the following groups: " +
						$"{string.Join(", ", affectedGoroups.Select(p => p.GroupName))}. " +
						$"Please reassign Group Managers before you cant continue this action.";
					return RedirectToAction("Index");
				}
				else
				{
					_usersRepository.Delete(user);
					_usersRepository.Save();
					TempData["SuccessMessage"] = $"User {user.FullName} deleted successfully.";
				}
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = $"Sorry, deleting user failed for unkwon reasons";
			}
		}

		return RedirectToAction("Index", "User");
	}

	private void LoadIndexViewData(UserViewModel vm)
	{
		vm.Users = _usersRepository.List(new QueryOptions<TicketAppUser>
		{
			OrderBy = u => u.LastName ?? string.Empty
		});

		vm.AvailableRoles = _usersRepository.GetRolesAsync().Result;
		vm.UserRoles = _usersRepository.GetUserRolesAsync().Result;
	}

    protected virtual async Task<List<Group>> GetConflictingGroupsForUser(TicketAppUser user)
    {
        var projectsLedByManager = await _groupsRepository.GetGroupByManagerIdAsync(user.Id)
                                   ?? new List<Group>();
        return projectsLedByManager.Where(g => g.Members != null).ToList();
    }
}