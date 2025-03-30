using Microsoft.AspNetCore.Mvc;
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
        private readonly Mock<SingletonService> _mockSingletonService;
        private readonly GroupController _controller;

        public TestGroupController()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();

		//    var singletonService = new SingletonService
		//    {
		//        CurrentUser = new TicketAppUser { Id = "user1", UserName = "TestUser" },
		//        CurrentUserRole = "Admin"
		//    };

            _controller = new GroupController(
                singletonService,  
                _mockUserRepository.Object,
                _mockGroupRepository.Object
            );

		//    _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
		//}



        [Fact]
        public async Task Index_ShouldReturnViewResult_WithGroupViewModel()
        {
            // Arrange
            var groups = new List<Group>
    {
        new Group { Id = "1", GroupName = "Group A" },
        new Group { Id = "2", GroupName = "Group B" }
    };

			_mockGroupRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(groups);


            // Act
            var result = await _controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GroupViewModel>(viewResult.Model);
            Assert.Equal(2, model.Groups.Count());
            Assert.Equal("Admin", model.CurrentUserRole);  
        }


        [Fact]
        public async Task AddGroup_ShouldReturnView_WithUsersList()
        {
            // Arrange
            var users = new List<TicketAppUser>
            {
                new TicketAppUser { Id = "1", UserName = "User1" },
                new TicketAppUser { Id = "2", UserName = "User2" }
            };

			_mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.CreateGroup();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddGroupViewModel>(viewResult.Model);
            Assert.Equal(2, model.AllUsers.Count);
        }

        [Fact]
        public async Task AddGroup_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
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

            // Act
            var result = await _controller.CreateGroup(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddGroup_ShouldReturnView_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("GroupName", "Required");

			var model = new AddGroupViewModel();

            // Act
            var result = await _controller.CreateGroup(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddGroupViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task DeleteGroup_ShouldReturnView_WhenGroupExists()
        {
            // Arrange
            var group = new Group { Id = "1", GroupName = "Test Group" };
            _mockGroupRepository.Setup(repo => repo.GetAsync("1")).ReturnsAsync(group);

            // Act
            var result = await _controller.DeleteGroup("1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(group, viewResult.Model);
        }

        [Fact]
        public async Task DeleteGroup_ShouldReturnNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetAsync("1")).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.DeleteGroup("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ConfirmDeleteGroup_ShouldRedirectToIndex_WhenGroupDeleted()
        {
            // Arrange
            var group = new Group { Id = "1", GroupName = "Test Group" };
            _mockGroupRepository.Setup(repo => repo.GetAsync("1")).ReturnsAsync(group);
            _mockGroupRepository.Setup(repo => repo.DeleteGroupAsync(group)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ConfirmDeleteGroup("1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task ConfirmDeleteGroup_ShouldReturnNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetAsync("1")).ReturnsAsync((Group)null);

            // Act
            var result = await _controller.ConfirmDeleteGroup("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ConfirmDeleteGroup_ShouldReturnView_WhenExceptionIsThrown()
        {
            // Arrange
            var group = new Group { Id = "1", GroupName = "Test Group" };
            _mockGroupRepository.Setup(repo => repo.GetAsync("1")).ReturnsAsync(group);
            _mockGroupRepository.Setup(repo => repo.DeleteGroupAsync(It.IsAny<Group>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ConfirmDeleteGroup("1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(group, viewResult.Model);
        }
    }
}
