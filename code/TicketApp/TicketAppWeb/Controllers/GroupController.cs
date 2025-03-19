using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers;


/// <summary>
/// The class represents the controller of group management funtcionality (add, edit delete...)
/// Emma
/// 03/?/2025
/// </summary>
[Authorize]
public class GroupController : Controller
{
	private readonly SingletonService _singletonService;
	private readonly IUserRepository _userRepository;
	private readonly IGroupRepository _groupRepository;
    private readonly IProjectRepository _projectRepository;

	/// <summary>
	/// Initializes a new instance of the Group Controller class.
	/// </summary>
	/// <param name="singletonService">The singleton service.</param>
	/// <param name="userRepository">The user repository.</param>
	/// <param name="groupRepository">The group repository.</param>
	/// <param name="projectRepository">The project repository.</param>
	public GroupController(SingletonService singletonService, IUserRepository userRepository, IGroupRepository groupRepository, IProjectRepository projectRepository)
	{
		_singletonService = singletonService;
		_userRepository = userRepository;
		_groupRepository = groupRepository;
		_projectRepository = projectRepository;

    }

	/// <summary>
	/// The index action method for the index page of group management page
	/// </summary>
	/// <param name="groupName">Name of the group.</param>
	/// <param name="groupLead">The group lead.</param>
	[HttpGet]
	public async Task<IActionResult> Index(string? groupName, string? groupLead)
	{
		var groups = await _groupRepository.GetAllAsync();

		var model = new GroupViewModel
		{
			Groups = groups.ToList(),
			SearchGroupName = groupName,
			SearchGroupLead = groupLead,
			CurrentUser = _singletonService.CurrentUser,
			CurrentUserRole = _singletonService.CurrentUserRole
		};

		return View(model);
	}

	/// <summary>
	/// Gets the group info ready before creating a new group and saving it to the database
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> CreateGroup()
	{
		var users = await _userRepository.GetAllUsersAsync();

		var model = new AddGroupViewModel
		{
			AllUsers = users.ToList(),
			SelectedUserIds = new List<string>()
		};

		return View(model);
	}

	/// <summary>
	/// Creates the group and saves it to the database
	/// </summary>
	/// <param name="model">The model.</param>
	[HttpPost]
	public async Task<IActionResult> CreateGroup(AddGroupViewModel model)
	{
		if (!ModelState.IsValid)
		{
			model.AllUsers = (await _userRepository.GetAllUsersAsync()).ToList();
			return View(model);
		}

		var newGroup = new Group
		{
			GroupName = model.GroupName,
			Description = model.Description,
			ManagerId = model.GroupLeadId,
			Members = new HashSet<TicketAppUser>()
		};

		if (model.SelectedUserIds != null)
		{
			foreach (var userId in model.SelectedUserIds)
			{
				var user = await _userRepository.GetAsync(userId);
				if (user != null)
				{
					newGroup.Members.Add(user);
				}
			}
		}

		await _groupRepository.InsertAsync(newGroup);
		await _groupRepository.SaveAsync();

		TempData["SuccessMessage"] = $"Group '{newGroup.GroupName}' created successfully!";

		return RedirectToAction("Index");
	}

	/// <summary>
	///  Gets group info ready before editing the group and saving it to the database.
	/// </summary>
	/// <param name="id">The identifier.</param>
	[HttpGet]
	public async Task<IActionResult> EditGroup(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return NotFound();
		}

		var group = await _groupRepository.GetAsync(id);
		if (group == null)
		{
			return NotFound();
		}

		var users = await _userRepository.GetAllUsersAsync();

		var model = new AddGroupViewModel
		{
			GroupId = group.Id,
			GroupName = group.GroupName,
			Description = group.Description,
			GroupLeadId = group.ManagerId,
			AllUsers = users.ToList(),
			SelectedUserIds = group.Members.Select(m => m.Id).ToList()
		};

		return View(model);
	}

	/// <summary>
	/// Edits the group's info and saves the changes to the database.
	/// </summary>
	/// <param name="model">The model.</param>
	[HttpPost]
	public async Task<IActionResult> UpdateGroup(AddGroupViewModel model)
	{
		if (!ModelState.IsValid)
		{
			var users = await _userRepository.GetAllUsersAsync();
			model.AllUsers = users.ToList();
			return View("EditGroup", model);
		}

		var group = await _groupRepository.GetAsync(model.GroupId);
		if (group == null)
		{
			return NotFound();
		}

		group.GroupName = model.GroupName;
		group.Description = model.Description;
		group.ManagerId = model.GroupLeadId;

		var existingMemberIds = group.Members.Select(m => m.Id).ToList();

		var membersToRemove = existingMemberIds.Except(model.SelectedUserIds).ToList();
		foreach (var userId in membersToRemove)
		{
			var user = group.Members.FirstOrDefault(m => m.Id == userId);
			if (user != null)
			{
				group.Members.Remove(user); 
			}
		}

		var membersToAdd = model.SelectedUserIds.Except(existingMemberIds).ToList();
		foreach (var userId in membersToAdd)
		{
			var user = await _userRepository.GetAsync(userId);
			if (user != null)
			{
				group.Members.Add(user); 
			}
		}

		await _groupRepository.SaveAsync();

		TempData["SuccessMessage"] = $"Group '{group.GroupName}' updated successfully!";

		return RedirectToAction("Index");
	}

	/// <summary>
	/// Gets the group's info ready for deletion.
	/// </summary>
	/// <param name="id">The identifier.</param>
	[HttpGet]
    public async Task<IActionResult> DeleteGroup(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var group = await _groupRepository.GetAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        var affectedProjects = await _projectRepository.GetProjectsByLeadAsync(group.ManagerId);
        var managerName = group.Manager?.FullName ?? "Unknown";

        var model = new DeleteGroupViewModel
        {
            GroupId = group.Id,
            GroupName = group.GroupName,
            ManagerId = group.ManagerId,
            ManagerName = managerName,
            AffectedProjects = affectedProjects
        };

        if (affectedProjects.Any())
        {
            TempData["ErrorMessage"] = $"Cannot delete group '{group.GroupName}' because the manager: {managerName} is still assigned to the project(s): {string.Join(", ", affectedProjects.Select(p => p.ProjectName))}. Please reassign project leads before deleting.";
            return RedirectToAction("Index");  
        }

        return View(model);
    }

	/// <summary>
	/// Confirms the groups deletion and removes it from the database.
	/// </summary>
	/// <param name="id">The identifier.</param>
	[HttpPost]
    public async Task<IActionResult> ConfirmDeleteGroup(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var group = await _groupRepository.GetAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        var affectedProjects = await _projectRepository.GetProjectsByLeadAsync(group.ManagerId);
        var managerName = group.Manager?.FullName ?? "Unknown";

        if (affectedProjects.Any())
        {
            TempData["ErrorMessage"] = $"Cannot delete group '{group.GroupName}' because the manager: {managerName} is still assigned to projects: {string.Join(", ", affectedProjects.Select(p => p.ProjectName))}. Please reassign project leads before deleting.";
            return RedirectToAction("Index");
        }

        try
        {
            await _groupRepository.DeleteGroupAsync(group);
            TempData["SuccessMessage"] = $"Group '{group.GroupName}' deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting group: {ex.Message}";
            return View("DeleteGroup", group);
        }
    }
}