using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
	private readonly SingletonService _singletonService;
	private readonly IProjectRepository _projectRepository;
	private readonly IBoardRepository _boardRepository;

	/// <summary>
	/// Initializes a new instance of the Project Controller class.
	/// </summary>
	/// <param name="singletonService">The singleton service.</param>
	/// <param name="projectRepository">The project repository.</param>
	public ProjectController(SingletonService singletonService, IProjectRepository projectRepository, IBoardRepository boardRepository)
	{
		_singletonService = singletonService;
		_projectRepository = projectRepository;
		_boardRepository = boardRepository;
	}

	/// <summary>
	/// The start of project management page
	/// </summary>
	/// <param name="projectName">Name of the project.</param>
	/// <param name="projectLead">The project lead.</param>
	public IActionResult Index(string? projectName, string? projectLead)
	{
		var viewModel = new ProjectViewModel();
		LoadIndexViewData(viewModel);

		viewModel.SearchProjectName = projectName;
		viewModel.SearchProjectLead = projectLead;
		viewModel.CurrentUser = _singletonService.CurrentUser;
		viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

		return View(viewModel);
	}

	/// <summary>
	/// Prepare resources need to adds a project.
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> AddProject()
	{
		var model = new ProjectViewModel
		{
			AvailableGroups = await _projectRepository.GetAvailableGroupsAsync()
		};
		return View(model);
	}

	/// <summary>
	/// Creates the project and saves it to the database.
	/// </summary>
	/// <param name="model">The project management view model.</param>
	[HttpPost]
	public async Task<IActionResult> CreateProject(ProjectViewModel model)
	{
		if (!ModelState.IsValid)
		{
			model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
			return View("AddProject", model);
		}

		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		var isAdmin = User.IsInRole("Admin");

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
			await _projectRepository.AddProjectAsync(project, assignedGroups, isAdmin);
			_boardRepository.AddBoard(project, isAdmin);
			TempData["SuccessMessage"] = $"Project {project.ProjectName} saved successfully";
			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
			return View("AddProject", model);
		}
	}

	/// <summary>
	/// Gets the project to edit by Id of the project.
	/// </summary>
	/// <param name="id">The project identifier.</param>
	/// <param name="leadChangeRequired">The lead change required, the flag used to check the why the edit is performed.</param>
	[HttpGet]
	public async Task<IActionResult> EditProject(string id, bool? leadChangeRequired = null)
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
			AssignedGroups = project.Groups.ToList(),
			PendingGroups = await _projectRepository.GetPendingGroupsForProject(id),
			LeadChangeRequired = leadChangeRequired ?? false

		};

		return View(model);
	}

	/// <summary>
	/// Edits the project and saves the changes to the database
	/// </summary>
	/// <param name="model">The model.</param>
	/// <param name="id">The identifier.</param>
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

		var isAdmin = User.IsInRole("Admin");

		try
		{

			if (model.LeadChangeRequired)
			{
				await _projectRepository.UpdateProjectAsync(project, model.SelectedGroupIds, isAdmin);
				TempData["SuccessMessage"] = $"Project Lead updated successfully. Proced with The privous action ";
				return RedirectToAction("Index", "Home");
			}
			await _projectRepository.UpdateProjectAsync(project, model.SelectedGroupIds, isAdmin);
			TempData["SuccessMessage"] = $"Project {project.ProjectName} updated successfully.";
			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			TempData["ErrorMessage"] += $"Unable to update project beacuse of " + ex.Message;
			model.AvailableGroups = await _projectRepository.GetAvailableGroupsAsync();
			return View(model);
		}
	}

	/// <summary>
	/// Gets the projet to be deleted by Id.
	/// </summary>
	/// <param name="id">The identifier.</param>
	[HttpGet]
	public async Task<IActionResult> DeleteProject(string id)
	{
		var project = await _projectRepository.GetProjectByIdAsync(id);
		if (project == null)
		{
			return NotFound();
		}
		return View(project);
	}

	/// <summary>
	/// Confirms the project delete action and remove the project from the database
	/// </summary>
	/// <param name="id">The project identifier.</param>
	[HttpPost]
	public async Task<IActionResult> ConfirmDelete(string id)
	{
		var project = await _projectRepository.GetProjectByIdAsync(id);
		if (project == null)
		{
			return NotFound();
		}

		try
		{
			await _projectRepository.DeleteProjectAsync(project);
			//_boardRepository.RemoveStatus(project);
			TempData["SuccessMessage"] = $"Project {project.ProjectName} deleted successfully";
			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = $"Error deleting project: {ex.Message}";
			return RedirectToAction("Index");
		}
	}

	/// <summary>
	/// Gets the group leads based on selected groups (the managers of the selected groups)
	/// </summary>
	/// <param name="groupIds">The group ids.</param>
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
		var projectData = _projectRepository.GetFilteredProjectsAndGroups(vm.SearchProjectName, vm.SearchProjectLead).Result;
		vm.Projects = projectData.Keys;
		vm.ProjectGroups = projectData;
		vm.AvailableGroups = _projectRepository.GetAvailableGroupsAsync().Result;
	}
}
