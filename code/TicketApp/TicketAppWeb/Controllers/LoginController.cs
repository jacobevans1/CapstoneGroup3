using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Controllers;

/// <summary>
/// The login controller handles logging the user into the system
/// Emma
/// 02/?/2025
/// </summary>
[AllowAnonymous]
public class LoginController : Controller
{
	private readonly SignInManager<TicketAppUser> _signInManager;
	private readonly UserManager<TicketAppUser> _userManager;
	private readonly SingletonService _singletonService;

	/// <summary>
	/// Initializes a new instance of the LoginController class.
	/// </summary>
	/// <param name="singletonService">The singleton service.</param>
	/// <param name="signInManager">The sign in manager.</param>
	/// <param name="userManager">The user manager.</param>
	public LoginController(SingletonService singletonService, SignInManager<TicketAppUser> signInManager, UserManager<TicketAppUser> userManager)
	{
		_singletonService = singletonService;
		_signInManager = signInManager;
		_userManager = userManager;
	}

	/// <summary>
	/// Gets and return the index view of the login page.
	/// </summary>
	[HttpGet]
	public IActionResult Index()
	{
		return View("Index");
	}

	/// <summary>
	/// Logs the specified user into the system by verifying their username and password.
	/// </summary>
	/// <param name="username">The username.</param>
	/// <param name="password">The password.</param>
	[HttpPost]
	public async Task<IActionResult> Index(string username, string password)
	{
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
		{
			ViewBag.Error = "Username and password are required.";
			return View("Index");
		}

		var user = await _userManager.FindByNameAsync(username);
		if (user == null)
		{
			ViewBag.Error = "Invalid username or password.";
			return View("Index");
		}

		var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
		if (result.Succeeded)
		{
			_singletonService.CurrentUser = user;
			_singletonService.CurrentUserRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
			return RedirectToAction("Index", "Home");
		}
		else
		{
			ViewBag.Error = "Invalid username or password.";
			return View("Index");
		}
	}


	/// <summary>
	/// Logs the user out of the system.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		_singletonService.CurrentUser = null;
		return RedirectToAction("Index", "Login");
	}
}
