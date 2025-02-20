using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Reposetories;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.Controllers;

/// <summary>
/// The Controller responsible for displaying and managing projects
/// Jabesi Abwe 
/// 02/20/2025
/// </summary>
public class ProjectController : Controller
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<TicketAppUser> _usersRepository;
    private readonly IRepository<Group> _groupsRepository;

    // Dedicated Constructor
    public ProjectController(
        IProjectRepository projectRepository,
        IRepository<TicketAppUser> usersRepository,
        IRepository<Group> groupsRepository)
    {
        _projectRepository = projectRepository;
        _usersRepository = usersRepository;
        _groupsRepository = groupsRepository;
    }

    // GET: Project/Index
    public IActionResult Index()
    {
        var viewModel = new ProjectViewModel();
        LoadIndexViewData(viewModel);
        return View(viewModel);
    }

    // POST: Project/Index{viewModel
    [HttpPost]
    public IActionResult Index(ProjectViewModel viewModel)
    {
        TempData["LeadId"] = viewModel.Project.LeadId;
        TempData["ProjectName"] = viewModel.Project.ProjectName;
        TempData["Description"] = viewModel.Project.Description;

        return RedirectToAction("SelectGroups");
    }

    // GET: Project/SelectGroups
    public IActionResult SelectGroups()
    {
        var viewModel = new ProjectViewModel
        {
            ProjectLeadId = TempData["LeadId"]?.ToString()
        };

        // Load groups for assignment
        LoadGroupsViewData(viewModel);

        // Get selected groups
        var selectedGroups = viewModel.SelectedGroupIds
                                        .Select(groupId => _groupsRepository.Get(groupId!))
                                        .ToList();

        // Filter available leads: only users who are group leads within selected groups
        var availableLeads = selectedGroups
            .Where(group => group!.ManagerId != null)
            .Select(group => group!.Members.FirstOrDefault(user => user.Id == group.ManagerId))
            .Where(lead => lead != null) 
            .Distinct()
            .ToList();

        // Set the available leads in the view model
        viewModel.AvailableGroupLeads = availableLeads!;

        return View(viewModel);
    }


    // POST: Project/Add{vm}
    [HttpPost]
    public async Task<IActionResult> Add(ProjectViewModel vm)
    {
        var createdById = (string)TempData["CreatedById"]!;
        var dateCreated = (DateTime)TempData["CreatedOnDate"]!;

        vm.Project.LeadId = vm.ProjectLeadId;
        vm.Project.CreatedById = createdById;
        vm.Project.CreatedAt = dateCreated;

        if (ModelState.IsValid)
        {
            _projectRepository.Insert(vm.Project);
            _projectRepository.Save();
        }

        TempData["message"] = $"Project {vm.Project.ProjectName} added successfully.";

        return RedirectToAction("Index", "Project");
    }

    // GET: Project/Edit/{id}
    public IActionResult Edit(string id)
    {
        var project = _projectRepository.Get(id);

        if (project == null)
        {
            return NotFound();
        }

        var viewModel = new ProjectViewModel
        {
            Project = project
        };
        LoadIndexViewData(viewModel);

        return View(viewModel);
    }

    // POST: Project/Edit/{id}
    [HttpPost]
    public IActionResult Edit(ProjectViewModel vm)
    {
        if (ModelState.IsValid)
        {
            _projectRepository.Update(vm.Project);
            _projectRepository.Save();

            TempData["message"] = $"Project {vm.Project.ProjectName} updated successfully.";
            return RedirectToAction("Index", "Project");
        }

        LoadIndexViewData(vm);
        return View(vm);
    }

    // GET: Project/Delete/{id}
    public IActionResult Delete(string id)
    {
        var project = _projectRepository.Get(id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: Project/Delete/{id}
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(string id)
    {
        var project = _projectRepository.Get(id);

        if (project != null)
        {
            _projectRepository.Delete(project);
            _projectRepository.Save();
            TempData["message"] = $"Project {project.ProjectName} deleted successfully.";
        }

        return RedirectToAction("Index", "Project");
    }

    // GET: Project/List{values}
    public IActionResult List(ProjectGridData values)
    {
        var options = new QueryOptions<Project>
        {
            OrderByDirection = values.SortDirection,
            PageNumber = values.PageNumber,
            PageSize = values.PageSize
        };

        // Sorting logic
        if (values.IsSortByProjectLead)
            options.OrderBy = p => p.Lead!.FullName;
        else
            options.OrderBy = p => p.ProjectName!;

        var viewModel = new ProjectViewModel
        {
            Projects = _projectRepository.List(options),
            CurrentRoute = values,
            TotalPages = values.GetTotalPages(_projectRepository.Count),
            SelectedPageSize = values.PageSize
        };

        return RedirectToAction("Index", viewModel);
    }

    // Project/ PageSizes{currentRoute}
    [HttpPost]
    public IActionResult PageSizes(ProjectGridData currentRoute)
    {
        return RedirectToAction("Index", currentRoute.ToDictionary());
    }

    private void LoadIndexViewData(ProjectViewModel vm)
    {
        vm.AvailableGroups = _groupsRepository.List(new QueryOptions<Group>
        {
            OrderBy = g => g.GroupName ?? string.Empty
        });

        vm.AvailableGroupLeads = _usersRepository.List(new QueryOptions<TicketAppUser>
        {
            OrderBy = u => u.LastName ?? string.Empty
        });
    }

    private void LoadGroupsViewData(ProjectViewModel vm)
    {
        vm.SelectedGroupIds = vm.Project.Groups?.Select(g => g.Id).ToArray() ?? Array.Empty<string>();

        vm.AvailableGroups = _groupsRepository.List(new QueryOptions<Group>
        {
            OrderBy = g => g.GroupName ?? string.Empty
        });
    }
}
