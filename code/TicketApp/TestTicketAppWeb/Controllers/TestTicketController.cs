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
	public class TicketControllerTests
	{
		private Mock<IProjectRepository> _mockProjectRepository;
		private Mock<IBoardRepository> _mockBoardRepository;
		private Mock<ITicketRepository> _mockTicketRepository;
		private Mock<IUserRepository> _mockUserRepository;
		private Mock<SingletonService> _mockSingletonService;
		private TicketController _controller;

		public TicketControllerTests()
		{
			_mockProjectRepository = new Mock<IProjectRepository>();
			_mockBoardRepository = new Mock<IBoardRepository>();
			_mockTicketRepository = new Mock<ITicketRepository>();
			_mockUserRepository = new Mock<IUserRepository>();
			_mockSingletonService = new Mock<SingletonService>();

			_controller = new TicketController(
				_mockSingletonService.Object,
				_mockProjectRepository.Object,
				_mockBoardRepository.Object,
				_mockTicketRepository.Object,
				_mockUserRepository.Object
			);
		}

		private static void SetupTempData(Controller controller)
		{
			var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
			controller.TempData = tempData;
		}

		[Fact]
		public async Task AddTicket_Get_ShouldReturnView_WithValidData()
		{
			// Arrange
			var mockSingletonService = new Mock<SingletonService>();
			var mockProjectRepository = new Mock<IProjectRepository>();
			var mockBoardRepository = new Mock<IBoardRepository>();
			var mockUserRepository = new Mock<IUserRepository>();

			var project = new Project { Id = "project1", Groups = new List<Group> { new Group { Id = "group1" } } };
			var board = new Board { Id = "board1" };

			mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "user1" });
			mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");

			mockProjectRepository.Setup(p => p.GetProjectByIdAsync(It.IsAny<string>())).ReturnsAsync(project);
			mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync(It.IsAny<string>())).ReturnsAsync(board);
			mockUserRepository.Setup(u => u.GetUsersByGroupId(It.IsAny<string>())).Returns(new List<TicketAppUser> { new TicketAppUser { Id = "user1" } });

			var controller = new TicketController(mockSingletonService.Object, mockProjectRepository.Object, mockBoardRepository.Object, null, mockUserRepository.Object);
			SetupTempData(controller);

			// Act
			var result = controller.AddTicket("project1", "stage1");

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<TicketViewModel>(viewResult.Model);
			Assert.Equal(project, model.Project);
			Assert.Equal(board, model.Board);
			Assert.Equal("stage1", model.SelectedStageId);
			Assert.NotNull(model.CurrentUser);
			Assert.Equal("Admin", model.CurrentUserRole);
			Assert.NotEmpty(model.Project.Groups);
			Assert.NotNull(model.Project.Groups.First().Members);
		}

		[Fact]
		public async Task AddTicket_Post_ShouldRedirectToBoardIndex_OnSuccess()
		{
			// Arrange
			var mockSingletonService = new Mock<SingletonService>();
			var mockProjectRepository = new Mock<IProjectRepository>();
			var mockBoardRepository = new Mock<IBoardRepository>();
			var mockTicketRepository = new Mock<ITicketRepository>();
			var mockUserRepository = new Mock<IUserRepository>();

			var viewModel = new TicketViewModel
			{
				Project = new Project { Id = "project1", Groups = new List<Group>() },
				Board = new Board { Id = "board1" },
				Ticket = new Ticket { Title = "New Ticket", Description = "Ticket Description" },
				SelectedStageId = "stage1"
			};

			mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "user1" });
			mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");

			mockProjectRepository.Setup(p => p.GetProjectByIdAsync(It.IsAny<string>())).ReturnsAsync(viewModel.Project);
			mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync(It.IsAny<string>())).ReturnsAsync(viewModel.Board);
			mockTicketRepository.Setup(t => t.AddTicket(It.IsAny<Ticket>())).Verifiable();
			mockUserRepository.Setup(u => u.GetUsersByGroupId(It.IsAny<string>())).Returns(new List<TicketAppUser>());

			var controller = new TicketController(mockSingletonService.Object, mockProjectRepository.Object, mockBoardRepository.Object, mockTicketRepository.Object, mockUserRepository.Object);
			SetupTempData(controller);


			// Act
			var result = controller.AddTicket(viewModel) as RedirectToActionResult;

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Equal("Board", redirectToActionResult.ControllerName);
			Assert.Equal($"{viewModel.Ticket.Title} added successfully.", controller.TempData["SuccessMessage"]);

			mockTicketRepository.Verify(t => t.AddTicket(It.IsAny<Ticket>()), Times.Once);
		}


		[Fact]
		public async Task AddTicket_Post_ShouldRedirectToBoardIndex_OnError()
		{
			// Arrange
			var mockSingletonService = new Mock<SingletonService>();
			var mockProjectRepository = new Mock<IProjectRepository>();
			var mockBoardRepository = new Mock<IBoardRepository>();
			var mockTicketRepository = new Mock<ITicketRepository>();
			var mockUserRepository = new Mock<IUserRepository>();

			var viewModel = new TicketViewModel
			{
				Project = new Project { Id = "project1", Groups = new List<Group>() },
				Board = new Board { Id = "board1" },
				Ticket = new Ticket { Title = "New Ticket", Description = "Ticket Description" },
				SelectedStageId = "stage1"
			};

			mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "user1" });
			mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");

			mockProjectRepository.Setup(p => p.GetProjectByIdAsync(It.IsAny<string>())).ReturnsAsync(viewModel.Project);
			mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync(It.IsAny<string>())).ReturnsAsync(viewModel.Board);
			mockTicketRepository.Setup(t => t.AddTicket(It.IsAny<Ticket>())).Throws(new Exception());
			mockUserRepository.Setup(u => u.GetUsersByGroupId(It.IsAny<string>())).Returns(new List<TicketAppUser>());

			var controller = new TicketController(mockSingletonService.Object, mockProjectRepository.Object, mockBoardRepository.Object, mockTicketRepository.Object, mockUserRepository.Object);
			SetupTempData(controller);

			// Act
			var result = controller.AddTicket(viewModel) as RedirectToActionResult;

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Equal("Board", redirectToActionResult.ControllerName);
			Assert.Equal("Sorry, ticket creation failed.", controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public void DeleteTicket_ShouldRedirectToBoardIndex_OnSuccess()
		{
			// Arrange
			var viewModel = new BoardViewModel { SelectedTicketId = "ticket1", Project = new Project { Id = "project1" } };

			_mockTicketRepository.Setup(repo => repo.DeleteTicket(It.IsAny<string>()));
			SetupTempData(_controller);

			// Act
			var result = _controller.DeleteTicket(viewModel) as RedirectToActionResult;

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Board", redirectResult.ControllerName);
		}
	}
}

