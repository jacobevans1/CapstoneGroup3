using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<TicketAppUser> _signInManager;
        private readonly UserManager<TicketAppUser> _userManager;

        public HomeController(SignInManager<TicketAppUser> signInManager, UserManager<TicketAppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Password, isPersistent: false, lockoutOnFailure: false);
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
