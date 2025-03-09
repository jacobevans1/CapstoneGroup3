using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

[Authorize]
public class GroupController : Controller
{
	private readonly SingletonService _singletonService;
	private readonly IUserRepository _userRepository;
	private readonly IGroupRepository _groupRepository;

	public GroupController(SingletonService singletonService, IUserRepository userRepository, IGroupRepository groupRepository)
	{
		_singletonService = singletonService;
		_userRepository = userRepository;
		_groupRepository = groupRepository;
	}

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

	[HttpGet]
	public async Task<IActionResult> AddGroup()
	{
		var users = await _userRepository.GetAllUsersAsync();

		var model = new AddGroupViewModel
		{
			AllUsers = users.ToList(),
			SelectedUserIds = new List<string>()
		};

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> AddGroup(AddGroupViewModel model)
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

		// Update group details
		group.GroupName = model.GroupName;
		group.Description = model.Description;
		group.ManagerId = model.GroupLeadId;

		// Get current members of the group
		var existingMemberIds = group.Members.Select(m => m.Id).ToList();

		// Find members to **remove** (they are in the DB but not in the new selection)
		var membersToRemove = existingMemberIds.Except(model.SelectedUserIds).ToList();
		foreach (var userId in membersToRemove)
		{
			var user = group.Members.FirstOrDefault(m => m.Id == userId);
			if (user != null)
			{
				group.Members.Remove(user); // Remove deselected members
			}
		}

		// Find members to **add** (they are newly selected but were not in the group before)
		var membersToAdd = model.SelectedUserIds.Except(existingMemberIds).ToList();
		foreach (var userId in membersToAdd)
		{
			var user = await _userRepository.GetAsync(userId);
			if (user != null)
			{
				group.Members.Add(user); // Add newly selected members
			}
		}

		await _groupRepository.SaveAsync();

		TempData["SuccessMessage"] = $"Group '{group.GroupName}' updated successfully!";

		return RedirectToAction("Index");
	}





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

		return View(group);
	}

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

		try
		{
			await _groupRepository.DeleteGroupAsync(group);

			TempData["SuccessMessage"] = $"Group '{group.GroupName}' deleted successfully!";

			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "Error deleting group: " + ex.Message);

			TempData["ErrorMessage"] = $"Error deleting group: {ex.Message}";

			return View("DeleteGroup", group);
		}
	}



}