using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Reposetories;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _projectRepository;
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

        // POST: Project/Index
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
            LoadGroupsViewData(viewModel);
            return View(viewModel);
        }

        // POST: Project/Add
        [HttpPost]
        public Task<IActionResult> Add(ProjectViewModel vm)
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

            return Task.FromResult<IActionResult>(RedirectToAction("Index", "Project"));
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
            vm.SelectedGroupIds = vm.Project.Groups?.Select(
                g => g.Id).ToArray() ?? Array.Empty<string>();

            vm.AvailableGroups = _groupsRepository.List(new QueryOptions<Group>
            {
                OrderBy = g => g.GroupName ?? string.Empty
            });
        }
    }
}
