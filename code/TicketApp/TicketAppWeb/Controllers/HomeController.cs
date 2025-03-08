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

        public HomeController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Displays the home page with pending approval requests.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await GetPendingApprovalsViewModel());
        }

        /// <summary>
        /// Fetches the pending approvals view content (for AJAX refresh).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var model = await GetPendingApprovalsViewModel();
            return PartialView("_PendingApprovalsPartial", model);
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to get the pending approvals view model.
        /// </summary>
        private async Task<PendingApprovalsViewModel> GetPendingApprovalsViewModel()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pendingRequests = await _projectRepository.GetPendingGroupApprovalRequestsAsync(userId!);

            return new PendingApprovalsViewModel
            {
                PendingRequests = pendingRequests
            };
        }
    }
}
