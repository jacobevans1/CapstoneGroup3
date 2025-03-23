using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Xunit;

namespace TestTicketAppWeb.Controllers
{
    public class TestGroupController
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly GroupController _controller;

        public TestGroupController()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();

            var singletonService = new SingletonService
            {
                CurrentUser = new TicketAppUser { Id = "user1", UserName = "TestUser" },
                CurrentUserRole = "Admin"
            };

            _controller = new GroupController(
                singletonService,
                _mockUserRepository.Object,
                _mockGroupRepository.Object,
                _mockProjectRepository.Object
            );

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Index_ShouldReturnViewResult_WithGroupViewModel()
        {
            var groups = new List<Group>
            {
                new Group { Id = "1", GroupName = "Group A" },
                new Group { Id = "2", GroupName = "Group B" }
            };

            _mockGroupRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(groups);

            var result = await _controller.Index(null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GroupViewModel>(viewResult.Model);
            Assert.Equal(2, model.Groups.Count());
            Assert.Equal("Admin", model.CurrentUserRole);
        }

        [Fact]
        public async Task CreateGroup_Get_ShouldReturnView_WithUsersList()
        {
            var users = new List<TicketAppUser>
            {
                new TicketAppUser { Id = "1", UserName = "User1" },
                new TicketAppUser { Id = "2", UserName = "User2" }
            };

            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.CreateGroup();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddGroupViewModel>(viewResult.Model);
            Assert.Equal(2, model.AllUsers.Count);
        }

        [Fact]
        public async Task CreateGroup_Post_ShouldRedirectToIndex_WhenModelIsValid()
        {
            var model = new AddGroupViewModel
            {
                GroupName = "Test Group",
                Description = "Test Description",
                GroupLeadId = "1",
                SelectedUserIds = new List<string> { "2", "3" }
            };

            _mockUserRepository.Setup(repo => repo.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new TicketAppUser { Id = id });

            _mockGroupRepository.Setup(repo => repo.InsertAsync(It.IsAny<Group>())).Returns(Task.CompletedTask);
            _mockGroupRepository.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            var result = await _controller.CreateGroup(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateGroup_Post_ShouldReturnView_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("GroupName", "Required");

            var model = new AddGroupViewModel();

            var result = await _controller.CreateGroup(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddGroupViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task EditGroup_ShouldReturnView_WhenGroupExists()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "EditGroup",
                Description = "Edit Desc",
                ManagerId = "mgr1",
                Members = new HashSet<TicketAppUser> { new TicketAppUser { Id = "u1" } }
            };

            var users = new List<TicketAppUser>
            {
                new TicketAppUser { Id = "u1", UserName = "User1" },
                new TicketAppUser { Id = "u2", UserName = "User2" }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.EditGroup("1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddGroupViewModel>(viewResult.Model);
            Assert.Equal("EditGroup", model.GroupName);
        }

        [Fact]
        public async Task EditGroup_ShouldReturnNotFound_WhenGroupNotExists()
        {
            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null);

            var result = await _controller.EditGroup("1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateGroup_ShouldReturnView_WhenModelInvalid()
        {
            _controller.ModelState.AddModelError("GroupName", "Required");

            var model = new AddGroupViewModel { GroupId = "1" };
            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(new List<TicketAppUser>());

            var result = await _controller.UpdateGroup(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditGroup", viewResult.ViewName);
        }

        [Fact]
        public async Task UpdateGroup_ShouldUpdateGroup_WhenValid()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Old",
                Description = "Old Desc",
                ManagerId = "mgr1",
                Members = new HashSet<TicketAppUser> { new TicketAppUser { Id = "u1" } }
            };

            var model = new AddGroupViewModel
            {
                GroupId = "1",
                GroupName = "New",
                Description = "New Desc",
                GroupLeadId = "mgr2",
                SelectedUserIds = new List<string> { "u2" }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockUserRepository.Setup(r => r.GetAsync("u2")).ReturnsAsync(new TicketAppUser { Id = "u2" });

            var result = await _controller.UpdateGroup(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteGroup_ShouldRedirect_WhenConflictingProjectsExist()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Test Group",
                ManagerId = "m1",
                Manager = new TicketAppUser { FirstName = "Test", LastName = "Manager" }
            };

            var conflictingProject = new Project
            {
                Id = "p1",
                ProjectName = "Project X",
                Groups = new List<Group> { group }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("m1"))
                .ReturnsAsync(new List<Project> { conflictingProject });

            var result = await _controller.DeleteGroup("1");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.True(_controller.TempData.ContainsKey("ErrorMessage"));
        }

        [Fact]
        public async Task DeleteGroup_ShouldReturnView_WhenNoConflicts()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Test Group",
                ManagerId = "m1",
                Manager = new TicketAppUser { FirstName = "Test", LastName = "Manager" }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("m1"))
                .ReturnsAsync(new List<Project>());

            var result = await _controller.DeleteGroup("1");

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DeleteGroupViewModel>(view.Model);
            Assert.Equal("Test Group", model.GroupName);
        }

        [Fact]
        public async Task ConfirmDeleteGroup_ShouldRedirect_WhenDeleted()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Test Group",
                ManagerId = "m1",
                Manager = new TicketAppUser { FirstName = "Test", LastName = "Manager" }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("m1"))
                .ReturnsAsync(new List<Project>());
            _mockGroupRepository.Setup(r => r.DeleteGroupAsync(group)).Returns(Task.CompletedTask);

            var result = await _controller.ConfirmDeleteGroup("1");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task ConfirmDeleteGroup_ShouldReturnView_WhenExceptionThrown()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Test Group",
                ManagerId = "m1",
                Manager = new TicketAppUser { FirstName = "Test", LastName = "Manager" }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("m1"))
                .ReturnsAsync(new List<Project>());
            _mockGroupRepository.Setup(r => r.DeleteGroupAsync(group))
                .ThrowsAsync(new Exception("Error"));

            var result = await _controller.ConfirmDeleteGroup("1");

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(group, view.Model);
        }
    }
}