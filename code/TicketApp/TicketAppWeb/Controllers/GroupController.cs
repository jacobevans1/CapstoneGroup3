using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

[Authorize]
public class GroupController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public GroupController(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var groups = await _groupRepository.GetAllAsync();

        var model = new GroupViewModel
        {
            Groups = groups.ToList()
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
    public async Task<IActionResult> CreateGroup(AddGroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var users = await _userRepository.GetAllUsersAsync();
            model.AllUsers = users.ToList();
            return View("AddGroup", model);
        }

        // Step 1: Create the group
        var newGroup = new Group
        {
            GroupName = model.GroupName,
            Description = model.Description,
            ManagerId = model.GroupLeadId,
        };

        // Step 2: Assign members
        if (model.SelectedUserIds != null && model.SelectedUserIds.Any())
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

        // Step 3: Assign group lead (must be in the group)
        var groupLead = newGroup.Members.FirstOrDefault(u => u.Id == model.GroupLeadId);
        if (groupLead != null)
        {
            newGroup.ManagerId = groupLead.Id;
            newGroup.Manager = groupLead;
        }
        else
        {
            ModelState.AddModelError("GroupLeadId", "The group lead must be in the group.");
            var users = await _userRepository.GetAllUsersAsync();
            model.AllUsers = users.ToList();
            return View("AddGroup", model);
        }

        // Step 4: Save to database
        await _groupRepository.InsertAsync(newGroup);
        await _groupRepository.SaveAsync();

        // Step 5: Redirect to Group Management Page
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
        Console.WriteLine("UpdateGroup action triggered"); // Debugging step

        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model state is invalid");
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"Validation Error - {key}: {error.ErrorMessage}");
                }
            }
            var users = await _userRepository.GetAllUsersAsync();
            model.AllUsers = users.ToList();
            return View("EditGroup", model);
        }

        var group = await _groupRepository.GetAsync(model.GroupId);
        if (group == null)
        {
            Console.WriteLine("Group not found");
            return NotFound();
        }

        Console.WriteLine($"Updating group {group.GroupName} with new lead: {model.GroupLeadId}");

        group.GroupName = model.GroupName;
        group.Description = model.Description;
        group.ManagerId = model.GroupLeadId;

        group.Members.Clear();
        foreach (var userId in model.SelectedUserIds)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user != null)
            {
                group.Members.Add(user);
            }
        }

        await _groupRepository.SaveAsync();
        Console.WriteLine("Group update successful!");

        return RedirectToAction("Index");
    }

}