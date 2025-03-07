using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace TicketAppWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public HomeController(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Displays the home page with pending approval requests.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity!.Name;
            var user = await _userRepository.GetAsync(userId!); // Get the user info

            // Get all the pending approval requests for projects that the user is managing groups for
            var pendingRequests = await _projectRepository.GetPendingGroupApprovalRequestsAsync(userId!);

            // If no pending requests, return the empty view
            if (!pendingRequests.Any())
            {
                return View("NoPendingApprovals");
            }

            var pendingApprovalViewModel = new PendingApprovalsViewModel
            {
                PendingRequests = pendingRequests
            };

            return View(pendingApprovalViewModel);
        }

        /// <summary>
        /// Approves a group for a project.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApproveGroupForProject(string projectId, string groupId)
        {
            try
            {
                var userId = User.Identity!.Name; // Get the current user's ID (group manager)

                // Approve the group for the project
                await _projectRepository.ApproveGroupForProjectAsync(projectId, groupId, userId!);

                // Redirect to the home page after the approval action
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception and display an error message (logging omitted for simplicity)
                ViewBag.ErrorMessage = $"Error approving group: {ex.Message}";
                return View("Error");
            }
        }

        /// <summary>
        /// Rejects a group for a project.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RejectGroupForProject(string projectId, string groupId)
        {
            try
            {
                // Reject the group for the project
                await _projectRepository.RejectGroupForProjectAsync(projectId, groupId);

                // Redirect to the home page after the rejection action
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception and display an error message
                ViewBag.ErrorMessage = $"Error rejecting group: {ex.Message}";
                return View("Error");
            }
        }

        /// <summary>
        /// Displays an error page if an exception occurs.
        /// </summary>
        public IActionResult Error()
        {
            return View();
        }
    }
}
