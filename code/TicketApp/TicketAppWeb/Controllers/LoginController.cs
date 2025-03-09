using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Controllers
{
	[AllowAnonymous] // Allows anyone to access login without being signed in

	public class LoginController : Controller
	{
		private readonly SignInManager<TicketAppUser> _signInManager;
		private readonly UserManager<TicketAppUser> _userManager;
		private readonly SingletonService _singletonService;

		public LoginController(SingletonService singletonService, SignInManager<TicketAppUser> signInManager, UserManager<TicketAppUser> userManager)
		{
			_singletonService = singletonService;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				ViewBag.Error = "Username and password are required.";
				return View();
			}

			var user = await _userManager.FindByNameAsync(username);
			if (user == null)
			{
				ViewBag.Error = "Invalid username or password.";
				return View();
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
				return View();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			_singletonService.CurrentUser = null;
			return RedirectToAction("Index", "Login");
		}
	}
}
