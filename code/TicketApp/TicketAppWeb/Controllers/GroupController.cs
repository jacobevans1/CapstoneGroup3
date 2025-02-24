using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using System.Linq;
using TicketAppWeb.Models.DataLayer;

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

        /// <summary>
        /// Loads the group management page.
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new GroupViewModel
            {
                Groups = _groupRepository.List(new QueryOptions<Group>
                {
                    OrderBy = g => g.GroupName
                }),
                AvailableUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                }),
                AvailableGroupManagers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                })
            };

            return View(viewModel);
        }

        /// <summary>
        /// Loads the Add New Group modal with available users.
        /// </summary>
        [HttpGet]
        public IActionResult Add()
        {
            var viewModel = new GroupViewModel
            {
                AvailableUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                }),
                AvailableGroupManagers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                })
            };

            return PartialView("_AddGroupModal", viewModel);
        }

        /// <summary>
        /// Handles adding a new group.
        /// </summary>
        [HttpPost]
        public IActionResult Add(GroupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var newGroup = new Group
                {
                    GroupName = vm.Group.GroupName,
                    Description = vm.Group.Description,
                    ManagerId = vm.GroupManagerId
                };

                // Retrieve selected users and assign them to the group
                var selectedUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    Where = u => vm.SelectedUserIds.Contains(u.Id)
                }).ToList();

                newGroup.Members = selectedUsers;

                _groupRepository.Insert(newGroup);
                _groupRepository.Save();
                TempData["Success"] = "Group added successfully!";
                return RedirectToAction("Index");
            }

            // If model is invalid, reload available users for selection
            vm.AvailableUsers = _userRepository.List(new QueryOptions<TicketAppUser>
            {
                OrderBy = u => u.LastName
            });
            return View("Index", vm);
        }

        /// <summary>
        /// Loads the edit group page.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var group = _groupRepository.Get(id);
            if (group == null)
                return NotFound();

            var viewModel = new GroupViewModel
            {
                Group = group,
                AvailableUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    OrderBy = u => u.LastName
                }),
                SelectedUserIds = group.Members.Select(u => u.Id).ToArray()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles updating a group.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(GroupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var group = _groupRepository.Get(vm.Group.Id);
                if (group == null)
                    return NotFound();

                group.GroupName = vm.Group.GroupName;
                group.Description = vm.Group.Description;
                group.ManagerId = vm.GroupManagerId;

                // Update members
                var selectedUsers = _userRepository.List(new QueryOptions<TicketAppUser>
                {
                    Where = u => vm.SelectedUserIds.Contains(u.Id)
                }).ToList();
                group.Members = selectedUsers;

                _groupRepository.Update(group);
                _groupRepository.Save();
                TempData["Success"] = "Group updated successfully!";
                return RedirectToAction("Index");
            }

            // If invalid, reload users
            vm.AvailableUsers = _userRepository.List(new QueryOptions<TicketAppUser>
            {
                OrderBy = u => u.LastName
            });
            return View(vm);
        }

        /// <summary>
        /// Deletes a group.
        /// </summary>
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
