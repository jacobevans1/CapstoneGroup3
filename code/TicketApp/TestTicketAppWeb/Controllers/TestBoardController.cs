using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers;

public class TestBoardController
{
	private readonly Mock<SingletonService> _mockSingletonService;
	private readonly Mock<IProjectRepository> _mockProjectRepository;
	private readonly Mock<IBoardRepository> _mockBoardRepository;
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly BoardController _controller;

	public TestBoardController()
	{
		_mockSingletonService = new Mock<SingletonService>();
		_mockSingletonService.SetupAllProperties();
		_mockSingletonService.Object.CurrentUserRole = "Admin";
		_mockSingletonService.Object.CurrentUser = new TicketAppUser { Id = "user1", UserName = "TestUser" };

		_mockProjectRepository = new Mock<IProjectRepository>();
		_mockBoardRepository = new Mock<IBoardRepository>();
		_mockUserRepository = new Mock<IUserRepository>();

		_controller = new BoardController(
			_mockSingletonService.Object,
			_mockProjectRepository.Object,
			_mockBoardRepository.Object,
			_mockUserRepository.Object
		);

		_controller.TempData = new TempDataDictionary(
			new DefaultHttpContext(),
			Mock.Of<ITempDataProvider>()
		);
	}

	[Fact]
	public void IndexShouldReturnViewResult()
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
			.Returns(new Dictionary<string, List<Group>>());

		_mockProjectRepository
			.Setup(r => r.GetProjectByNameAndLeadAsync(It.IsAny<string>(), It.IsAny<string>()))
			.ReturnsAsync(new Project { Id = "project1" });

		_mockUserRepository.Setup(u => u.Get(It.IsAny<int>()))
			.Returns(new TicketAppUser { Id = "user1", UserName = "TestUser" });

		_mockBoardRepository.Setup(b => b.GetBoardStageTickets(It.IsAny<string>()))
			.Returns(new Dictionary<string, List<Ticket>>
			{
				{
					"Stage1", new List<Ticket>
					{
						new Ticket { AssignedTo = "user1" }
					}
				}
			});

		// Act
		var result = _controller.Index(projectId);

		// Assert
		var viewResult = Assert.IsType<ViewResult>(result);
		var model = Assert.IsAssignableFrom<BoardViewModel>(viewResult.Model);
		Assert.Equal(projectId, model.Project.Id);
		Assert.NotNull(model.Stages);
	}
	
	[Fact]
	public void EditBoard_UserAuthorized_ReturnsViewWithViewModel()
	{
		// Arrange
		var projectId = "1";
		var board = new Board
		{
			Id = "1",
			Project = new Project { ProjectName = "Test Project", LeadId = "user1" }
		};
		var stages = new List<Stage>
		{
			new Stage { Id = "1", Name = "Stage 1" }
		};
		var assignedGroups = new Dictionary<string, List<Group>>
		{
			{ "1", new List<Group> { new Group { Id = "1", GroupName = "Group 1" } } }
		};
		var assignedTickets = new Dictionary<string, List<Ticket>>
		{
			{ "1", new List<Ticket> { new Ticket { Id = "1", Title = "Ticket 1" } } }
		};
		var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" };

		_mockBoardRepository
			.Setup(r => r.GetBoardByProjectIdAsync(projectId))
			.ReturnsAsync(board);
		_mockBoardRepository
			.Setup(r => r.GetStages(board.Id))
			.Returns(stages);
		_mockBoardRepository
			.Setup(r => r.GetBoardStageGroups(board.Id))
			.Returns(assignedGroups);
		_mockBoardRepository
			.Setup(r => r.GetBoardStageTickets(board.Id))
			.Returns(assignedTickets);
		_mockProjectRepository
			.Setup(r => r.GetProjectByNameAndLeadAsync(board.Project.ProjectName, board.Project.LeadId))
			.ReturnsAsync(project);

		// Act
		var result = _controller.EditBoard(projectId);

		// Assert
		var viewResult = Assert.IsType<ViewResult>(result);
		var model = Assert.IsType<BoardViewModel>(viewResult.Model);
		Assert.Equal(board, model.Board);
		Assert.Equal(project, model.Project);
		Assert.Equal(stages, model.Stages);
		Assert.Equal(assignedGroups, model.AssignedGroups);
		Assert.Equal(assignedTickets, model.AssignedTickets);
	}

	[Fact]
	public void EditBoard_UserNotAuthorized_ReturnsRedirectToIndex()
	{
		// Arrange
		var projectId = "1";
		var singletonService = new SingletonService
		{
			CurrentUser = new TicketAppUser { Id = "user2", UserName = "TestUser2" },
			CurrentUserRole = "User"
		};
		var controller = new BoardController(
			singletonService,
			_mockProjectRepository.Object,
			_mockBoardRepository.Object,
			_mockUserRepository.Object
		);
		controller.TempData = new TempDataDictionary(
			new DefaultHttpContext(),
			Mock.Of<ITempDataProvider>()
		);

		var board = new Board { Id = "1", Project = new Project { ProjectName = "Test Project", LeadId = "user1" } };
		var stages = new List<Stage> { new Stage { Id = "1", Name = "Stage 1" } };
		var assignedGroups = new Dictionary<string, List<Group>>
		{
			{ "1", new List<Group> { new Group { Id = "1", GroupName = "Group 1" } } }
		};
		var assignedTickets = new Dictionary<string, List<Ticket>>
		{
			{ "1", new List<Ticket> { new Ticket { Id = "1", Title = "Ticket 1" } } }
		};
		var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" };

		_mockBoardRepository
			.Setup(r => r.GetBoardByProjectIdAsync(projectId))
			.ReturnsAsync(board);
		_mockBoardRepository
			.Setup(r => r.GetStages(board.Id))
			.Returns(stages);
		_mockBoardRepository
			.Setup(r => r.GetBoardStageGroups(board.Id))
			.Returns(assignedGroups);
		_mockBoardRepository
			.Setup(r => r.GetBoardStageTickets(board.Id))
			.Returns(assignedTickets);
		_mockProjectRepository
			.Setup(r => r.GetProjectByNameAndLeadAsync(board.Project.ProjectName, board.Project.LeadId))
			.ReturnsAsync(project);

		// Act
		var result = controller.EditBoard(projectId);

		// Assert
		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirect.ActionName);
		Assert.Equal(projectId, redirect.RouteValues!["projectId"]);
		Assert.Equal("You are not authorized to edit this board.", controller.TempData["ErrorMessage"]);
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

		_mockBoardRepository.Setup(b => b.RenameStage(It.IsAny<string>(), It.IsAny<string>()));

		// Act
		var result = _controller.RenameStage(viewModel) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
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

		_mockBoardRepository.Setup(b => b.RenameStage(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Database error"));

		// Act
		var result = _controller.RenameStage(viewModel) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal("Sorry, renaming stage failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void AddStage_Get_ReturnsViewWithViewModel()
	{
		var projectId = "1";
		var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" };
		var board = new Board { Id = "1", Project = project };

		_mockProjectRepository.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
		_mockBoardRepository.Setup(r => r.GetBoardByProjectIdAsync(projectId)).ReturnsAsync(board);

		var result = _controller.AddStage(projectId);

		var viewResult = Assert.IsType<ViewResult>(result);
		var model = Assert.IsType<BoardViewModel>(viewResult.Model);
		Assert.Equal(project, model.Project);
		Assert.Equal(board, model.Board);
		Assert.Equal(projectId, model.Project.Id);
	}

	[Fact]
	public void AddStage_Post_StageCreationFails_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			NewStageName = "New Stage",
			SelectedGroupIds = new List<string> { "1", "2" }
		};

		_mockBoardRepository.Setup(r => r.AddStage(viewModel.Board.Id, viewModel.NewStageName, viewModel.SelectedGroupIds)).Throws(new Exception("DB error"));

		var result = _controller.AddStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
		Assert.Equal("Sorry, stage creation failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void AddStage_Post_StageCreationSucceeds_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			NewStageName = "New Stage",
			SelectedGroupIds = new List<string> { "1", "2" }
		};

		_mockBoardRepository.Setup(r => r.AddStage(viewModel.Board.Id, viewModel.NewStageName, viewModel.SelectedGroupIds)).Callback(() => { });

		var result = _controller.AddStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
		Assert.Equal("New Stage stage added successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void AddStage_Post_StageCreationSucceeds_ReturnsRedirectToEditBoardV2()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			NewStageName = "New Stage",
			SelectedGroupIds = new List<string> { "1", "2" }
		};

		_mockBoardRepository.Setup(r => r.AddStage(viewModel.Board.Id, viewModel.NewStageName, viewModel.SelectedGroupIds)).Callback(() => { });

		var result = _controller.AddStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
		Assert.Equal("New Stage stage added successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void AssignGroupToStage_Get_PopulatesViewModel()
	{

		// Arrange

		var projectId = "project1";
		var boardId = "board1";
		var stageId = "stage1";
		var project = new Project { Id = projectId };
		var board = new Board { Id = boardId };
		var groups = new List<Group> { new Group { Id = "g1" }, new Group { Id = "g2" } };

		var assigned = new Dictionary<string, List<Group>> { { stageId, groups } };

		_mockProjectRepository.Setup(r => r.GetProjectByIdAsync(projectId))
			.ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync(projectId))
			.ReturnsAsync(board);
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups(boardId))
			.Returns(assigned);

		// Act
		var result = _controller.AssignGroupToStage(projectId, boardId, stageId) as ViewResult;

		// Assert
		Assert.NotNull(result);
		var vm = Assert.IsType<BoardViewModel>(result.Model);
		Assert.Equal(project, vm.Project);
		Assert.Equal(board, vm.Board);
		Assert.Equal(assigned, vm.AssignedGroups);
		Assert.Equal(stageId, vm.SelectedStageId);
		Assert.Equal(new[] { "g1", "g2" }, vm.SelectedGroupIds);
	}

	[Fact]
	public void AssignGroupToStage_Post_Succeeds_ReturnsRedirectWithSuccess()
	{
		// Arrange
		var vm = new BoardViewModel
		{

			Project = new Project { Id = "project1" },
			Board = new Board { Id = "board1" },
			SelectedStageId = "stage1",
			SelectedGroupIds = new List<string> { "g1", "g2" }
		};

		_mockBoardRepository.Setup(b => b.AssignGroupToStage(vm.Board.Id, vm.SelectedStageId, vm.SelectedGroupIds));

		// Act
		var result = _controller.AssignGroupToStage(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal(vm.Project.Id, result.RouteValues!["projectId"]);
		Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
		Assert.Equal("Reassigned stage successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void AssignGroupToStage_Post_Fails_ReturnsRedirectWithError()
	{

		// Arrange
		var vm = new BoardViewModel
		{

			Project = new Project { Id = "project1" },
			Board = new Board { Id = "board1" },
			SelectedStageId = "stage1",
			SelectedGroupIds = new List<string> { "g1", "g2" }

		};

		_mockBoardRepository.Setup(b => b.AssignGroupToStage(vm.Board.Id, vm.SelectedStageId, vm.SelectedGroupIds))
			.Throws(new Exception("DB error"));

		// Act
		var result = _controller.AssignGroupToStage(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal(vm.Project.Id, result.RouteValues!["projectId"]);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
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
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);
	}

	[Fact]
	public void MoveStage_StageNotFound_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			SelectedStageId = "stage1",
			SelectedDirection = "left"
		};

		_mockBoardRepository.Setup(r => r.GetBoardStages(viewModel.Board.Id)).Returns(new List<BoardStage>());

		var result = _controller.MoveStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
	}

	[Fact]
	public void MoveStage_MoveLeft_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			SelectedStageId = "stage2",
			SelectedDirection = "left"
		};

		var boardStages = new List<BoardStage>
		{
			new BoardStage { StageId = "stage1", StageOrder = 1 },
			new BoardStage { StageId = "stage2", StageOrder = 2 }
		};

		_mockBoardRepository.Setup(r => r.GetBoardStages(viewModel.Board.Id)).Returns(boardStages);

		var result = _controller.MoveStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
	}

	[Fact]
	public void MoveStage_MoveRight_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			SelectedStageId = "stage1",
			SelectedDirection = "right"
		};

		var boardStages = new List<BoardStage>
		{
			new BoardStage { StageId = "stage1", StageOrder = 1 },
			new BoardStage { StageId = "stage2", StageOrder = 2 }
		};

		_mockBoardRepository.Setup(r => r.GetBoardStages(viewModel.Board.Id)).Returns(boardStages);

		var result = _controller.MoveStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
	}

	[Fact]
	public void MoveStage_InvalidDirection_CallsSaveAndRedirects()
	{

		// Arrange
		var vm = new BoardViewModel
		{
			Project = new Project { Id = "project1" },
			Board = new Board { Id = "board1" },
			SelectedStageId = "stage1",
			SelectedDirection = "up"
		};

		var boardStages = new List<BoardStage>{
			new BoardStage { StageId = "stage1", StageOrder = 0 },
			new BoardStage { StageId = "stage2", StageOrder = 1 }
		};

		_mockBoardRepository
			.Setup(b => b.GetBoardStages(vm.Board.Id))
			.Returns(boardStages);

		_mockBoardRepository
			.Setup(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()));

		// Act
		var result = _controller.MoveStage(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);

		_mockBoardRepository.Verify(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()), Times.Once);

	}

	[Fact]
	public void MoveStage_SaveBoardStagesThrows_CatchesAndRedirects()
	{

		// Arrange
		var vm = new BoardViewModel
		{

			Project = new Project { Id = "project1" },
			Board = new Board { Id = "board1" },
			SelectedStageId = "stage1",
			SelectedDirection = "right"
		};

		var boardStages = new List<BoardStage>{
			new BoardStage { StageId = "stage1", StageOrder = 0 },
			new BoardStage { StageId = "stage2", StageOrder = 1 }
		};

		_mockBoardRepository
			.Setup(b => b.GetBoardStages(vm.Board.Id))
			.Returns(boardStages);

		_mockBoardRepository
			.Setup(b => b.SaveBoardStages(It.IsAny<List<BoardStage>>()))
			.Throws(new Exception("DB error"));

		// Act
		var result = _controller.MoveStage(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("EditBoard", result.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal(vm.Project.Id, result.RouteValues!["projectId"]);
	}

	[Fact]
	public void DeleteStage_StageDeletionFails_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			SelectedStageId = "stage1"
		};

		_mockBoardRepository.Setup(r => r.DeleteStage(viewModel.Board.Id, viewModel.SelectedStageId)).Throws(new Exception("DB error"));

		var result = _controller.DeleteStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
		Assert.Equal("Sorry, deleting stage failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void DeleteStage_StageDeletionSucceeds_ReturnsRedirectToEditBoard()
	{
		var viewModel = new BoardViewModel
		{
			Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" },
			Board = new Board { Id = "1", Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "user1" } },
			SelectedStageId = "stage1"
		};

		_mockBoardRepository.Setup(r => r.DeleteStage(viewModel.Board.Id, viewModel.SelectedStageId)).Callback(() => { });

		var result = _controller.DeleteStage(viewModel);

		var redirect = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("EditBoard", redirect.ActionName);
		Assert.Equal(viewModel.Project.Id, redirect.RouteValues!["projectId"]);
		Assert.Equal("Deleted stage successfully.", _controller.TempData["SuccessMessage"]);
	}
}