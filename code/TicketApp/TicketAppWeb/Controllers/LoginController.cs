using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Controllers
{
	public class LoginController : Controller
	{
		private readonly SignInManager<TicketAppUser> _signInManager;
		private readonly UserManager<TicketAppUser> _userManager;

		public LoginController(SignInManager<TicketAppUser> signInManager, UserManager<TicketAppUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
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
				return RedirectToAction("ProjectManagement", "Project");
			}
			else
			{
				ViewBag.Error = "Invalid username or password.";
				return View();
			}
		}
	}
}
