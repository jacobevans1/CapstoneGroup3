using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pendingRequests = await _projectRepository.GetPendingGroupApprovalRequestsAsync(userId!);

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
                await _projectRepository.ApproveGroupForProjectAsync(projectId, groupId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving group: {ex.Message}";
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
                await _projectRepository.RejectGroupForProjectAsync(projectId, groupId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting group: {ex.Message}";
                return View("Error");
            }
        }
    }
}