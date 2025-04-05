using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers
{
	public class TestBoardController
	{
		private readonly Mock<SingletonService> _mockSingletonService;
		private readonly Mock<IProjectRepository> _mockProjectRepository;
		private readonly Mock<IBoardRepository> _mockBoardRepository;
		private readonly BoardController _controller;

		public TestBoardController()
		{
			_mockSingletonService = new Mock<SingletonService>();
			_mockProjectRepository = new Mock<IProjectRepository>();
			_mockBoardRepository = new Mock<IBoardRepository>();
			_controller = new BoardController(
				_mockSingletonService.Object,
				_mockProjectRepository.Object,
				_mockBoardRepository.Object
			);

			_controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
		}

		[Fact]
		public void Index_ShouldReturnViewResult()
		{
			// Arrange
			var projectId = "project1";
			var viewModel = new BoardViewModel
			{
				CurrentUserRole = "Admin",
				Project = new Project { Id = projectId },
				Board = new Board { Id = "board1" },
				Stages = new List<Stage>(),
			};

			_mockBoardRepository
			.Setup(b => b.GetBoardByProjectIdAsync(It.IsAny<string>()))
			.ReturnsAsync(new Board
			{
				Id = "board1",
				Project = new Project { ProjectName = "TestProject", LeadId = "lead1" }
			});

			_mockBoardRepository
				.Setup(b => b.GetStages(It.IsAny<string>()))
				.Returns(new List<Stage>());

			_mockBoardRepository
				.Setup(b => b.GetBoardStageGroups(It.IsAny<string>()))
				.Returns(new Dictionary<string, string>());

			_mockProjectRepository
				.Setup(r => r.GetProjectByNameAndLeadAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new Project { Id = "project1" });


			// Act
			var result = _controller.Index(projectId);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<BoardViewModel>(viewResult.Model);
			Assert.Equal(projectId, model.Project.Id);
			Assert.NotNull(model.Stages);
		}


		[Fact]
		public void RenameStage_ShouldRedirectWithSuccessMessage()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				SelectedStageId = "stage1",
				NewStageName = "Renamed Stage",
				Project = new Project { Id = "project1" }
			};

			// Mock the RenameStage method in the repository
			_mockBoardRepository.Setup(b => b.RenameStage(It.IsAny<string>(), It.IsAny<string>()));

			// Act
			var result = _controller.RenameStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);

			// Assert that TempData contains the success message
			Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
			Assert.Equal("Renamed Renamed Stage stage successfully.", _controller.TempData["SuccessMessage"]);
		}

		[Fact]
		public void RenameStage_ShouldReturnError_WhenExceptionIsThrown()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				SelectedStageId = "stage1",
				NewStageName = "Renamed Stage",
				Project = new Project { Id = "project1" }
			};

			// Mock the RenameStage method to throw an exception
			_mockBoardRepository.Setup(b => b.RenameStage(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Database error"));

			// Act
			var result = _controller.RenameStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.Equal("Sorry, renaming stage failed.", _controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public void AddStage_ShouldRedirectWithSuccessMessage()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				NewStageName = "New Stage",
				SelectedGroupId = "group1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.AddStage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

			// Act
			var result = _controller.AddStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
			Assert.Equal("New Stage stage added successfully.", _controller.TempData["SuccessMessage"]);
		}

		[Fact]
		public void AddStage_ShouldReturnError_WhenExceptionIsThrown()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				NewStageName = "New Stage",
				SelectedGroupId = "group1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.AddStage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Database error"));

			// Act
			var result = _controller.AddStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.Equal("Sorry, stage creation failed.", _controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public void AssignGroupToStage_ShouldRedirectWithSuccessMessage()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				SelectedGroupId = "group1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.AssignGroupToStage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

			// Act
			var result = _controller.AssignGroupToStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
			Assert.Equal("Reassigned stage successfully.", _controller.TempData["SuccessMessage"]);
		}

		[Fact]
		public void AssignGroupToStage_ShouldReturnError_WhenExceptionIsThrown()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				SelectedGroupId = "group1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.AssignGroupToStage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Database error"));

			// Act
			var result = _controller.AssignGroupToStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.Equal("Sorry, reassigning stage failed.", _controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public void DeleteStage_ShouldRedirectWithSuccessMessage()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.DeleteStage(It.IsAny<string>(), It.IsAny<string>()));

			// Act
			var result = _controller.DeleteStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
			Assert.Equal("Deleted stage successfully.", _controller.TempData["SuccessMessage"]);
		}

		[Fact]
		public void DeleteStage_ShouldReturnError_WhenExceptionIsThrown()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.DeleteStage(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Database error"));

			// Act
			var result = _controller.DeleteStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
			Assert.Equal("Sorry, deleting stage failed.", _controller.TempData["ErrorMessage"]);
		}

		[Fact]
		public void MoveStage_ShouldRedirectWhenValid()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				SelectedDirection = "right",
				Project = new Project { Id = "project1" }
			};

			var boardStages = new List<BoardStage>
			{
				new BoardStage { StageId = "stage1", StageOrder = 1 },
				new BoardStage { StageId = "stage2", StageOrder = 2 }
			};
			_mockBoardRepository.Setup(b => b.GetBoardStages(It.IsAny<string>())).Returns(boardStages);
			_mockBoardRepository.Setup(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()));

			// Act
			var result = _controller.MoveStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
		}

		[Fact]
		public void MoveStage_ShouldRedirectWhenStageIdNotFound()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Project = new Project { Id = "project1" },
				SelectedStageId = "nonExistentStage",
				SelectedDirection = "left"
			};

			var boardStages = new List<BoardStage>
			{
				new BoardStage { StageId = "stage1", StageOrder = 1},
				new BoardStage { StageId = "stage2", StageOrder = 2 }
			};

			_mockBoardRepository.Setup(b => b.GetBoardStages(It.IsAny<string>())).Returns(boardStages);

			// Act
			var result = _controller.MoveStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
		}


		[Fact]
		public void MoveStage_ShouldRedirectWhenExceptionIsThrown()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				SelectedDirection = "right",
				Project = new Project { Id = "project1" }
			};

			_mockBoardRepository.Setup(b => b.GetBoardStages(It.IsAny<string>())).Throws(new Exception("Database error"));

			// Act
			var result = _controller.MoveStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
		}

		[Fact]
		public void MoveStage_ShouldMoveLeft_WhenValid()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage2",
				SelectedDirection = "left",
				Project = new Project { Id = "project1" }
			};

			var boardStages = new List<BoardStage>
			{
				new BoardStage { StageId = "stage1", StageOrder = 0 },
				new BoardStage { StageId = "stage2", StageOrder = 1 }
			};

			_mockBoardRepository.Setup(b => b.GetBoardStages(It.IsAny<string>())).Returns(boardStages);
			_mockBoardRepository.Setup(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()));

			// Act
			var result = _controller.MoveStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
		}

		[Fact]
		public void MoveStage_ShouldMoveRight_WhenValid()
		{
			// Arrange
			var viewModel = new BoardViewModel
			{
				Board = new Board { Id = "board1" },
				SelectedStageId = "stage1",
				SelectedDirection = "right",
				Project = new Project { Id = "project1" }
			};

			var boardStages = new List<BoardStage>
			{
				new BoardStage { StageId = "stage1", StageOrder = 0 },
				new BoardStage { StageId = "stage2", StageOrder = 1 }
			};

			_mockBoardRepository.Setup(b => b.GetBoardStages(It.IsAny<string>())).Returns(boardStages);
			_mockBoardRepository.Setup(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()));

			// Act
			var result = _controller.MoveStage(viewModel) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Board", result.ControllerName);
		}

	}
}
