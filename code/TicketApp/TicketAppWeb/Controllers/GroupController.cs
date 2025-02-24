using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TicketAppWeb.Controllers
{
    public class GroupController : Controller
    {
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<TicketAppUser> _userRepository;

        public GroupController(IRepository<Group> groupRepository, IRepository<TicketAppUser> userRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new GroupViewModel
            {
                Groups = _groupRepository.List(new QueryOptions<Group>
                {
                    OrderBy = g => g.GroupName
                }),
                AvailableGroupManagers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Add(GroupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Retrieve selected users from form
                var selectedUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    Where = u => vm.SelectedUserIds.Contains(u.Id)
                }).ToList();

                vm.Group.Members = selectedUsers;

                _groupRepository.Insert(vm.Group);
                _groupRepository.Save();
                TempData["Success"] = "Group added successfully!";
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            var group = _groupRepository.Get(id);
            if (group == null)
                return NotFound();

            var viewModel = new GroupViewModel { Group = group };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(GroupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _groupRepository.Update(vm.Group);
                _groupRepository.Save();
                TempData["Success"] = "Group updated successfully!";
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var group = _groupRepository.Get(id);
            if (group != null)
            {
                _groupRepository.Delete(group);
                _groupRepository.Save();
                TempData["Success"] = "Group deleted successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
