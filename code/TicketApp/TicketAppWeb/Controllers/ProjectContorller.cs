using Microsoft.AspNetCore.Mvc;

namespace TicketAppWeb.Controllers
{
    public class ProjectController : Controller
    {
        [HttpGet]
        public IActionResult ProjectManagement()
        {
            return View();
        }
    }
}
