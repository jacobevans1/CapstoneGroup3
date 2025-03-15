using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TicketAppWeb.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProjectRepository _projectRepository;
    private readonly IGroupRepository _groupRepository;

    public HomeController(IProjectRepository projectRepository, IGroupRepository groupRepository)
    {
        _projectRepository = projectRepository;
        _groupRepository = groupRepository;
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
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error processing approval: " + ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }


    /// <summary>
    /// Rejects a group for a project. If the group manager is set lead of project, 
    /// redirects to edit the project to set a new lead
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RejectGroupForProject(string projectId, string groupId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null) return RedirectToAction("Index", "Home");

        var group = await _groupRepository.GetAsync(groupId);
        if (group == null) return RedirectToAction("Index", "Home");

        if (project.LeadId == group.ManagerId)
        {
            return RedirectToAction("EditProject", "Project", new { id = projectId, leadChangeRequired = true });
        }

        try
        {
            await _projectRepository.RejectGroupForProjectAsync(projectId, groupId);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error processing request: " + ex.Message;
            return RedirectToAction("Index", "Home");
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
