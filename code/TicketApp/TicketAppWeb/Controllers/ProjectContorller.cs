using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers;

/// <summary>
/// The Controller responsible for displaying and managing projects
/// Jabesi Abwe 
/// 02/20/2025
/// </summary>
public class ProjectController : Controller
{
	private readonly IProjectRepository _projectRepository;

	public ProjectController(IProjectRepository projectRepository)
	{
		_projectRepository = projectRepository;
	}

	// GET: Projects
	public IActionResult Index(QueryOptions<Project> options)
	{
		var viewModel = new ProjectViewModel();
		LoadIndexViewData(viewModel);
		return View(viewModel);
	}

    [HttpGet]
    public async Task<IActionResult> AddProject()
    {
        var model = new ProjectViewModel
        {
            AvailableGroups = await _projectRepository.GetAvailableGroupsAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreatProject(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
            return View("AddProject", model);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var project = new Project
        {
            ProjectName = model.ProjectName,
            Description = model.Description,
            LeadId = model.ProjectLeadId,
            CreatedById = userId
        };

        try
        {
            var assignedGroups = model.SelectedGroupIds;
            await _projectRepository.AddProjectAsync(project, assignedGroups);
            TempData["SuccessMessage"] = $"Project {project.ProjectName} saved successfuly";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
            return View("AddProject", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditProject(string id)
    {
        var project = await _projectRepository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var model = new ProjectViewModel
        {
            ProjectName = project.ProjectName,
            Description = project.Description,
            ProjectLeadId = project.LeadId,
            SelectedGroupIds = project.Groups.Select(g => g.Id).ToList(),
            AvailableGroups = await _projectRepository.GetAvailableGroupsAsync(),
            AssignedGroups = project.Groups.ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProject(ProjectViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
            return View(model);
        }

        var project = await _projectRepository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        project.ProjectName = model.ProjectName;
        project.Description = model.Description;
        project.LeadId = model.ProjectLeadId;

        try
        {
            await _projectRepository.UpdateProjectAsync(project, model.SelectedGroupIds);
            TempData["SuccessMessage"] = $"Project {project.ProjectName} updated successfully";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
            return View(model);
        }
    }

    // Method to return group leads based on selected groups
    [HttpGet]
    public async Task<JsonResult> GetGroupLeads(string groupIds)
    {
        if (string.IsNullOrEmpty(groupIds))
            return Json(new List<object>());

        var groupIdArray = groupIds.Split(",").ToList();
        var leads = await _projectRepository.GetGroupLeadsAsync(groupIdArray);

        var result = leads.Select(l => new { id = l.Id, fullName = l.FullName }).Distinct();
        return Json(result);
    }

	private void LoadIndexViewData(ProjectViewModel vm)
	{
		vm.Projects = _projectRepository.GetProjectsAndGroups().Result.Keys;
		vm.ProjectGroups = _projectRepository.GetProjectsAndGroups().Result;
		vm.AvailableGroups = _projectRepository.GetAvailableGroupsAsync().Result;
	}
}
