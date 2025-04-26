using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

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
        public async Task Index_ReturnsView_WithGroupViewModel()
        {
            _mockGroupRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Group> { new Group(), new Group() });

            var result = await _controller.Index(null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<GroupViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task CreateGroup_Get_ReturnsViewWithUsers()
        {
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(new List<TicketAppUser> { new TicketAppUser(), new TicketAppUser() });

            var result = await _controller.CreateGroup();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddGroupViewModel>(viewResult.Model);
            Assert.Equal(2, model.AllUsers.Count);
        }

        [Fact]
        public async Task CreateGroup_Post_ValidModel_RedirectsToIndex()
        {
            var model = new AddGroupViewModel
            {
                GroupName = "Group 1",
                Description = "Desc",
                GroupLeadId = "1",
                SelectedUserIds = new List<string> { "2" }
            };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new TicketAppUser());

            var result = await _controller.CreateGroup(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task CreateGroup_Post_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("GroupName", "Required");

            var result = await _controller.CreateGroup(new AddGroupViewModel());

            Assert.IsType<ViewResult>(result);
        }

		[Fact]
		public async Task CreateGroup_Post_WithNullSelectedUserIds_ShouldStillSucceed()
		{
			var model = new AddGroupViewModel
			{
				GroupName = "Test Group",
				Description = "Test Desc",
				GroupLeadId = "1",
				SelectedUserIds = null!
			};

			var result = await _controller.CreateGroup(model);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
		}

		[Fact]
		public async Task CreateGroup_SetsSuccessMessage()
		{
			var model = new AddGroupViewModel
			{
				GroupName = "G1",
				Description = "D1",
				GroupLeadId = "1",
				SelectedUserIds = new List<string>()
			};

			var result = await _controller.CreateGroup(model);

			Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
		}

		[Fact]
        public async Task EditGroup_ValidId_ReturnsViewWithModel()
        {
            var group = new Group
            {
                Id = "1",
                GroupName = "Test",
                Description = "Description",
                ManagerId = "1",
                Members = new List<TicketAppUser> { new TicketAppUser { Id = "2" } }
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(new List<TicketAppUser> { new TicketAppUser { Id = "2" } });

            var result = await _controller.EditGroup("1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddGroupViewModel>(viewResult.Model);
            Assert.Equal("Test", model.GroupName);
        }

        [Fact]
        public async Task EditGroup_NullOrInvalidId_ReturnsNotFound()
        {
            var result1 = await _controller.EditGroup(null!);
            Assert.IsType<NotFoundResult>(result1);

            _mockGroupRepository.Setup(r => r.GetAsync("1"))
                .ReturnsAsync((Group)null!);

            var result2 = await _controller.EditGroup("1");
            Assert.IsType<NotFoundResult>(result2);
        }

        [Fact]
        public async Task UpdateGroup_ValidModel_RedirectsToIndex()
        {
            var model = new AddGroupViewModel
            {
                GroupId = "1",
                GroupName = "Updated",
                Description = "Updated Desc",
                GroupLeadId = "1",
                SelectedUserIds = new List<string> { "2" }
            };

            var group = new Group
            {
                Id = "1",
                Members = new List<TicketAppUser>()
            };

            _mockGroupRepository.Setup(r => r.GetAsync("1"))
                .ReturnsAsync(group);
            _mockUserRepository.Setup(r => r.GetAsync("2"))
                .ReturnsAsync(new TicketAppUser { Id = "2" });

            var result = await _controller.UpdateGroup(model);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task UpdateGroup_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("GroupName", "Required");

            var result = await _controller.UpdateGroup(new AddGroupViewModel());

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditGroup", viewResult.ViewName);
        }

        [Fact]
        public async Task UpdateGroup_GroupNotFound_ReturnsNotFound()
        {
            var model = new AddGroupViewModel { GroupId = "999" };

            _mockGroupRepository.Setup(r => r.GetAsync("999")).ReturnsAsync((Group)null!);

            var result = await _controller.UpdateGroup(model);
            Assert.IsType<NotFoundResult>(result);
        }

		[Fact]
		public async Task UpdateGroup_InvalidModel_ReturnsViewWithModel()
		{
			_controller.ModelState.AddModelError("GroupName", "Required");

			var model = new AddGroupViewModel
			{
				GroupId = "1",
				GroupName = "Updated",
				Description = "Updated Desc",
				GroupLeadId = "1",
				SelectedUserIds = new List<string> { "2" }
			};

			_mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<TicketAppUser> { new TicketAppUser(), new TicketAppUser() });

			var result = await _controller.UpdateGroup(model);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("EditGroup", viewResult.ViewName);
			var returnedModel = Assert.IsType<AddGroupViewModel>(viewResult.Model);
			Assert.Equal(2, returnedModel.AllUsers.Count);
		}

		[Fact]
		public async Task UpdateGroup_GroupNotFound_ReturnsNotFoundV3()
		{
			var model = new AddGroupViewModel
			{
				GroupId = "1",
				GroupName = "Updated",
				Description = "Updated Desc",
				GroupLeadId = "1",
				SelectedUserIds = new List<string> { "2" }
			};

			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null!);

			var result = await _controller.UpdateGroup(model);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task UpdateGroup_ValidModel_UpdatesGroupAndRedirects()
		{
			var model = new AddGroupViewModel
			{
				GroupId = "1",
				GroupName = "Updated",
				Description = "Updated Desc",
				GroupLeadId = "1",
				SelectedUserIds = new List<string> { "2", "3" }
			};

			var group = new Group
			{
				Id = "1",
				GroupName = "Old Name",
				Description = "Old Desc",
				ManagerId = "Old Manager",
				Members = new List<TicketAppUser> { new TicketAppUser { Id = "2" }, new TicketAppUser { Id = "4" } }
			};

			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockUserRepository.Setup(r => r.GetAsync("3")).ReturnsAsync(new TicketAppUser { Id = "3" });
			_mockUserRepository.Setup(r => r.GetAsync("2")).ReturnsAsync(new TicketAppUser { Id = "2" });
			_mockUserRepository.Setup(r => r.GetAsync("4")).ReturnsAsync(new TicketAppUser { Id = "4" });

			var result = await _controller.UpdateGroup(model);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal($"Group '{group.GroupName}' updated successfully!", _controller.TempData["SuccessMessage"]);
		}


		[Fact]
		public async Task UpdateGroup_ValidModel_RedirectsToIndexV2()
		{
			var model = new AddGroupViewModel
			{
				GroupId = "1",
				GroupName = "Updated",
				Description = "Updated Desc",
				GroupLeadId = "1",
				SelectedUserIds = new List<string> { "2" }
			};

			var group = new Group
			{
				Id = "1",
				Members = new List<TicketAppUser>()
			};

			_mockGroupRepository.Setup(r => r.GetAsync("1"))
				.ReturnsAsync(group);
			_mockUserRepository.Setup(r => r.GetAsync("2"))
				.ReturnsAsync(new TicketAppUser { Id = "2" });

			var result = await _controller.UpdateGroup(model);
			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
		}

		[Fact]
		public async Task UpdateGroup_InvalidModel_ReturnsViewV2()
		{
			_controller.ModelState.AddModelError("GroupName", "Required");

			var result = await _controller.UpdateGroup(new AddGroupViewModel());

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("EditGroup", viewResult.ViewName);
		}

		[Fact]
		public async Task DeleteGroup_GroupExists_ReturnsView()
		{
			var group = new Group { Id = "1", GroupName = "Test Group", ManagerId = "M1", Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" } };
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1")).ReturnsAsync(new List<Project>());

			var result = await _controller.DeleteGroup("1");
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_NotFound_ReturnsNotFound()
		{
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null!);

			var result = await _controller.DeleteGroup("1");
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_GroupExists_ReturnsViewV2()
		{
			var group = new Group { Id = "1", GroupName = "Test Group", ManagerId = "M1", Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" } };
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1")).ReturnsAsync(new List<Project>());

			var result = await _controller.DeleteGroup("1");
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_NotFound_ReturnsNotFoundV2()
		{
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null!);

			var result = await _controller.DeleteGroup("1");
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_IdIsNull_ReturnsNotFound()
		{
			var result = await _controller.DeleteGroup(null!);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_IdIsEmpty_ReturnsNotFound()
		{
			var result = await _controller.DeleteGroup(string.Empty);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteGroup_ManagerAssignedToProjectWithGroup_ShouldRedirect()
		{
			var group = new Group
			{
				Id = "1",
				GroupName = "Group A",
				ManagerId = "M1",
				Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" }
			};

			var project = new Project
			{
				ProjectName = "Project 1",
				Groups = new List<Group> { new Group { Id = "1" } }
			};

			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1")).ReturnsAsync(new List<Project> { project });

			var result = await _controller.DeleteGroup("1");
			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_IdIsNullOrEmpty_ReturnsNotFound()
		{
			var result = await _controller.ConfirmDeleteGroup(null!);
			Assert.IsType<NotFoundResult>(result);

			result = await _controller.ConfirmDeleteGroup(string.Empty);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_GroupNotFound_ReturnsNotFound()
		{
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null!);

			var result = await _controller.ConfirmDeleteGroup("1");
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_GroupExists_DeletesAndRedirects()
		{
			var group = new Group { Id = "1", GroupName = "Group", ManagerId = "M1", Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" } };
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1")).ReturnsAsync(new List<Project>());
			_mockGroupRepository.Setup(r => r.DeleteGroupAsync(group)).Returns(Task.CompletedTask);

			var result = await _controller.ConfirmDeleteGroup("1");
			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal($"Group '{group.GroupName}' deleted successfully!", _controller.TempData["SuccessMessage"]);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_ThrowsException_ReturnsView()
		{
			var group = new Group { Id = "1", GroupName = "Group", ManagerId = "M1", Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" } };
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1")).ReturnsAsync(new List<Project>());
			_mockGroupRepository.Setup(r => r.DeleteGroupAsync(group)).ThrowsAsync(new Exception("DB error"));

			var result = await _controller.ConfirmDeleteGroup("1");
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("DeleteGroup", viewResult.ViewName);
			Assert.Equal($"Error deleting group: DB error", _controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_NotFound_ReturnsNotFound()
		{
			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync((Group)null!);

			var result = await _controller.ConfirmDeleteGroup("1");
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ConfirmDeleteGroup_ManagerOnOtherProject_ShouldAllowDelete()
		{
			var group = new Group
			{
				Id = "1",
				GroupName = "Group A",
				ManagerId = "M1",
				Manager = new TicketAppUser { FirstName = "Emma", LastName = "Boss" }
			};

			// Project exists, but this group is not in it
			var unrelatedProject = new Project
			{
				ProjectName = "Unrelated",
				Groups = new List<Group> { new Group { Id = "999" } }
			};

			_mockGroupRepository.Setup(r => r.GetAsync("1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.GetProjectsByLeadAsync("M1"))
				.ReturnsAsync(new List<Project> { unrelatedProject });
			_mockGroupRepository.Setup(r => r.DeleteGroupAsync(group)).Returns(Task.CompletedTask);

			var result = await _controller.ConfirmDeleteGroup("1");
			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
		}

	}
}
