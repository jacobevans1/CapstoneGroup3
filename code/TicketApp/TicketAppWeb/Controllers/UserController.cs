using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult UserManagement()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View(); 
        }
   
    }
}
