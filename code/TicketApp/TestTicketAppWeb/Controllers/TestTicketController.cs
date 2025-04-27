using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers;

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

	private static void InitTempData(Controller controller)
	{
		controller.TempData = new TempDataDictionary(
			new DefaultHttpContext(),
			Mock.Of<ITempDataProvider>()
		);
	}

	private static void SetupTempData(Controller controller)
	{
		var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
		controller.TempData = tempData;
	}

	[Fact]
	public void AddTicket_Get_ShouldReturnView_WithValidData()
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

		var controller = new TicketController(mockSingletonService.Object, mockProjectRepository.Object, mockBoardRepository.Object, null!, mockUserRepository.Object);
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
	public void AddTicket_Post_ShouldRedirectToBoardIndex_OnSuccess()
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
	public void AddTicket_Post_ShouldRedirectToBoardIndex_OnError()
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
	public void EditTicket_Get_TicketNotFound_RedirectsWithError()
	{
		// Arrange
		_mockTicketRepository.Setup(r => r.Get("missing")).Returns((Ticket?)null);
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("missing", "p", "b", "s") as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result!.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal("Ticket not found.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void EditTicket_Get_Valid_PopulatesEligibleAssignees_ForManager()
	{
		// Arrange
		var ticket = new Ticket { Id = "t1", AssignedTo = "uX", Stage = "s1" };
		_mockTicketRepository.Setup(r => r.Get("t1")).Returns(ticket);
		var project = new Project
		{
			Id = "p1",
			LeadId = "mgr",
			Groups = new List<Group>{
			new Group { Id="g1", ManagerId="mgr", Members = new List<TicketAppUser>{
				new TicketAppUser{ Id="uA" },
				new TicketAppUser{ Id="uB" }
			} }
		}
		};
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("p1")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("p1")).ReturnsAsync(new Board { Id = "b1" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("b1")).Returns(new Dictionary<string, 
			List<Group>>{         { "s1", project.Groups.ToList() }});
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "mgr" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("t1", "p1", "b1", "s1") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert: both members added
		Assert.Equal(2, vm.EligibleAssignees.Count);
		Assert.Contains(vm.EligibleAssignees, u => u.Id == "uA");
		Assert.Contains(vm.EligibleAssignees, u => u.Id == "uB");
	}

	[Fact]
	public void EditTicket_Post_InvalidModel_ReturnsViewWithModel()
	{
		// Arrange
		var vm = new TicketViewModel { Project = new Project { Id = "p" }, Board = new Board { Id = "b" }, Ticket = new Ticket { Id = "t" } };
		_controller.ModelState.AddModelError("x", "err");
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("p")).ReturnsAsync(vm.Project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("p")).ReturnsAsync(vm.Board);

		// Act
		var result = _controller.EditTicket(vm) as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.Same(vm, result!.Model);
	}

	[Fact]
	public void EditTicket_Post_TicketNotFound_RedirectsWithError()
	{
		// Arrange
		var vm = new TicketViewModel { Project = new Project { Id = "p" }, Board = new Board(), Ticket = new Ticket { Id = "t" } };
		_mockTicketRepository.Setup(r => r.Get("t")).Returns((Ticket?)null);
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result!.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal("Ticket not found.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void EditTicket_Post_Valid_SetsSuccessMessageAndRedirects()
	{
		var existing = new Ticket
		{
			Id = "t",
			BoardId = "b",
			CreatedBy = "u",
			CreatedDate = DateTime.Now,
			Stage = "s2",
			History = new List<TicketHistory>()
		};

		var vm = new TicketViewModel
		{
			Project = new Project { Id = "p" },
			Board = new Board { Id = "b" },
			Ticket = new Ticket { Id = "t", Title = "X", Description = "D" },
			SelectedUserId = "u2",
			SelectedStageId = "s2"

		};

		_mockTicketRepository
			.Setup(r => r.Get("t"))
			.Returns(existing);

		_mockProjectRepository
			.Setup(p => p.GetProjectByIdAsync("p"))
			.ReturnsAsync(vm.Project);

		_mockBoardRepository
			.Setup(b => b.GetBoardByProjectIdAsync("p"))
			.ReturnsAsync(vm.Board);

		_mockUserRepository
			.Setup(u => u.Get("u2"))
			.Returns(new TicketAppUser { Id = "u2", FirstName = "Assign", LastName = "Gee" });

		_mockSingletonService
			.Setup(s => s.CurrentUser)
			.Returns(new TicketAppUser { Id = "u", FirstName = "John", LastName = "Doe" });

		_mockSingletonService
			.Setup(s => s.CurrentUserRole)
			.Returns("Admin");

		SetupTempData(_controller);

		// Act
		var result = _controller.EditTicket(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result!.ActionName);
		Assert.Equal("Board", result.ControllerName);
		Assert.Equal("Ticket updated successfully.", _controller.TempData["SuccessMessage"]);

	}

	[Fact]
	public void EditTicket_Post_UpdateThrows_ReturnsViewWithError()
	{
		// Arrange
		var existing = new Ticket { Id = "t", BoardId = "b", CreatedBy = "u", CreatedDate = DateTime.Now };
		var vm = new TicketViewModel
		{
			Project = new Project { Id = "p" },
			Board = new Board { Id = "b" },
			Ticket = new Ticket { Id = "t", Title = "X", Description = "D" },
			SelectedUserId = "u2",
			SelectedStageId = "s2"
		};
		_mockTicketRepository.Setup(r => r.Get("t")).Returns(existing);
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("p")).ReturnsAsync(vm.Project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("p")).ReturnsAsync(vm.Board);
		_mockTicketRepository.Setup(r => r.UpdateTicket(existing, It.IsAny<Ticket>()))
			.Throws(new Exception());
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket(vm) as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Ticket update failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void EditTicket_Get_Valid_PopulatesEligibleAssignees_ForMember()
	{
		// Arrange
		var ticket = new Ticket { Id = "tM", AssignedTo = "uMember", Stage = "stageM" };
		_mockTicketRepository.Setup(r => r.Get("tM")).Returns(ticket);

		// project with one group, whose Members list contains currentUser, but ManagerId ≠ currentUser.Id
		var member = new TicketAppUser { Id = "uMember" };
		var group = new Group
		{
			Id = "gM",
			ManagerId = "notManager",
			Members = new List<TicketAppUser> { member }
		};
		var project = new Project { Id = "pM", LeadId = "lead", Groups = new List<Group> { group } };

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pM")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pM")).ReturnsAsync(new Board { Id = "bM" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bM"))
						   .Returns(new Dictionary<string, List<Group>> { { "stageM", new List<Group> { group } } });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(member);
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tM", "pM", "bM", "stageM") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert: only the current user was added
		Assert.Single(vm.EligibleAssignees);
		Assert.Equal("uMember", vm.EligibleAssignees[0].Id);
	}

	[Fact]
	public void EditTicket_Get_Valid_PopulatesEligibleAssignees_ForAdmin()
	{
		// Arrange
		var ticket = new Ticket { Id = "tA", AssignedTo = "uX", Stage = "stageA" };
		_mockTicketRepository.Setup(r => r.Get("tA")).Returns(ticket);

		// project with one group, whose Members does NOT contain currentUser, ManagerId ≠ currentUser.Id
		var member1 = new TicketAppUser { Id = "u1" };
		var member2 = new TicketAppUser { Id = "u2" };
		var group = new Group
		{
			Id = "gA",
			ManagerId = "notMgr",
			Members = new List<TicketAppUser> { member1, member2 }
		};
		var project = new Project { Id = "pA", LeadId = "lead", Groups = new List<Group> { group } };

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pA")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pA")).ReturnsAsync(new Board { Id = "bA" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bA"))
						   .Returns(new Dictionary<string, List<Group>> { { "stageA", new List<Group> { group } } });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "someoneElse" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tA", "pA", "bA", "stageA") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert: all group members were added by Admin
		Assert.Equal(2, vm.EligibleAssignees.Count);
		Assert.Contains(vm.EligibleAssignees, u => u.Id == "u1");
		Assert.Contains(vm.EligibleAssignees, u => u.Id == "u2");
	}

	[Fact]
	public void EditTicket_Get_NoAssignedGroups_SkipsEligibleAssignees()
	{
		// Arrange
		var ticket = new Ticket { Id = "tN", AssignedTo = "uX", Stage = "stageN" };
		_mockTicketRepository.Setup(r => r.Get("tN")).Returns(ticket);

		var project = new Project { Id = "pN", LeadId = "lead", Groups = new List<Group>() };
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pN")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pN")).ReturnsAsync(new Board { Id = "bN" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bN")).Returns((Dictionary<string, List<Group>>)null!);

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uX" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tN", "pN", "bN", "stageN") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert: no one was added
		Assert.Empty(vm.EligibleAssignees);
	}

	[Fact]
	public void EditTicket_Get_ProjectGroupsNull_DoesNotThrowAndEligibleEmpty()
	{
		// Arrange
		var ticket = new Ticket { Id = "tX", AssignedTo = "uX", Stage = "sX" };
		_mockTicketRepository.Setup(r => r.Get("tX")).Returns(ticket);

		var project = new Project { Id = "pX", Groups = null! };
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pX")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pX")).ReturnsAsync(new Board { Id = "bX" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bX")).Returns(new Dictionary<string, List<Group>>());
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uX" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tX", "pX", "bX", "sX") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert: no groups -> no eligibles
		Assert.Empty(vm.EligibleAssignees);  
	}

	[Fact]
	public void EditTicket_Get_AssignedGroupsMissingKey_EligibleEmpty()
	{
		// Arrange
		var ticket = new Ticket { Id = "tY", AssignedTo = "uY", Stage = "sY" };
		_mockTicketRepository.Setup(r => r.Get("tY")).Returns(ticket);

		var grp = new Group { Id = "gY", ManagerId = "mgrY", Members = new List<TicketAppUser>() };
		var project = new Project { Id = "pY", Groups = new List<Group> { grp } };
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pY")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pY")).ReturnsAsync(new Board { Id = "bY" });
		// stageId "sY" is not a key
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bY"))
							.Returns(new Dictionary<string, List<Group>> { { "otherStage", new List<Group> { grp } } });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uY" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tY", "pY", "bY", "sY") as ViewResult;
		var vm = Assert.IsType<TicketViewModel>(result!.Model);

		// Assert
		Assert.Empty(vm.EligibleAssignees);
	}

	[Fact]
	public void EditTicket_Get_GroupMembersPrePopulated_DoesNotCallUserRepo()
	{
		// Arrange
		var ticket = new Ticket { Id = "tZ", AssignedTo = "uZ", Stage = "sZ" };
		_mockTicketRepository.Setup(r => r.Get("tZ")).Returns(ticket);

		var preMembers = new List<TicketAppUser> { new TicketAppUser { Id = "uZ" } };
		var grp = new Group { Id = "gZ", ManagerId = "mgrZ", Members = preMembers };
		var project = new Project { Id = "pZ", Groups = new List<Group> { grp } };

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pZ")).ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("pZ")).ReturnsAsync(new Board { Id = "bZ" });
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bZ"))
							.Returns(new Dictionary<string, List<Group>> { { "sZ", new List<Group> { grp } } });
		_mockUserRepository.Setup(u => u.GetUsersByGroupId(It.IsAny<string>()))
						   .Throws(new Exception("Should not be called"));

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "mgrZ" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		InitTempData(_controller);

		// Act
		var result = _controller.EditTicket("tZ", "pZ", "bZ", "sZ");

		// Assert no exception -> userRepo.GetUsersByGroupId was not invoked
		Assert.IsType<ViewResult>(result);
	}

	[Fact]
	public void MoveTicket_NoGroupForNewStage_RetainsAssignedAndSucceeds()
	{
		// Arrange
		var ticket = new Ticket { Id = "tA", BoardId = "bA", AssignedTo = "uA", Stage = "oldA" };
		_mockTicketRepository.Setup(r => r.Get("tA")).Returns(ticket);
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bA"))
							.Returns(new Dictionary<string, List<Group>> { { "otherStage", new List<Group>() } });
		_mockBoardRepository.Setup(b => b.GetStages("bA")).Returns(new List<Stage>());
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pA")).ReturnsAsync(new Project { Id = "pA", Groups = new List<Group>() });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uA" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
			.Callback<Ticket, Ticket>((o, u) => updated = u);

		// Act
		_controller.MoveTicket("tA", "newA", "bA", "pA");

		// Assert
		Assert.Equal("uA", updated.AssignedTo);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_RelevantGroupsEmpty_RetainsAssignedAndSucceeds()
	{
		// Arrange
		var ticket = new Ticket { Id = "tC", BoardId = "bC", AssignedTo = "uC", Stage = "oldC" };
		_mockTicketRepository.Setup(r => r.Get("tC")).Returns(ticket);

		var grp = new Group { Id = "gC1", GroupName = "G", ManagerId = "mgr", Members = new List<TicketAppUser>() };
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bC"))
							.Returns(new Dictionary<string, List<Group>> { { "newC", new List<Group> { grp } } });
		_mockBoardRepository.Setup(b => b.GetStages("bC")).Returns(new List<Stage>());

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pC"))
							 .ReturnsAsync(new Project { Id = "pC", Groups = new List<Group> { new Group { Id = "gC2" } } });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uC" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
							.Callback<Ticket, Ticket>((o, u) => updated = u);

		// Act
		_controller.MoveTicket("tC", "newC", "bC", "pC");

		// Assert
		Assert.Null(updated.AssignedTo);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_ProjectGroupsNull_RetainsAssignedAndSucceeds()
	{
		// Arrange
		var ticket = new Ticket { Id = "tB", BoardId = "bB", AssignedTo = "uB", Stage = "oldB" };
		_mockTicketRepository.Setup(r => r.Get("tB")).Returns(ticket);

		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bB"))
							.Returns(new Dictionary<string, List<Group>> { { "newB", new List<Group>() } });
		_mockBoardRepository.Setup(b => b.GetStages("bB")).Returns(new List<Stage>());

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pB"))
							 .ReturnsAsync(new Project { Id = "pB", Groups = null! });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "uB" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
							.Callback<Ticket, Ticket>((o, u) => updated = u);

		// Act
		_controller.MoveTicket("tB", "newB", "bB", "pB");

		// Assert
		Assert.Null(updated.AssignedTo);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_GroupMembersNull_PopulatesThenClearsAssignedTo()
	{
		// Arrange
		var ticket = new Ticket { Id = "tD", BoardId = "bD", AssignedTo = "uD", Stage = "oldD" };
		_mockTicketRepository.Setup(r => r.Get("tD")).Returns(ticket);

		var grp = new Group { Id = "gD", ManagerId = "mgrD", Members = null! };
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bD"))
							.Returns(new Dictionary<string, List<Group>> { { "newD", new List<Group> { grp } } });
		_mockBoardRepository.Setup(b => b.GetStages("bD")).Returns(new List<Stage>());

		// project.Groups contains the same group
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pD"))
							 .ReturnsAsync(new Project { Id = "pD", Groups = new List<Group> { grp } });

		// userRepo returns empty list → userD not in it → should clear
		_mockUserRepository.Setup(u => u.GetUsersByGroupId("gD")).Returns(new List<TicketAppUser>());

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "mgrD" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
							.Callback<Ticket, Ticket>((o, u) => updated = u);

		// Act
		_controller.MoveTicket("tD", "newD", "bD", "pD");

		// Assert
		Assert.Null(updated.AssignedTo); 
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_GroupMembersNonEmptyButNoMatch_ClearsAssignedTo()
	{
		// Arrange
		var ticket = new Ticket { Id = "tE", BoardId = "bE", AssignedTo = "uE", Stage = "oldE" };
		_mockTicketRepository.Setup(r => r.Get("tE")).Returns(ticket);

		var member1 = new TicketAppUser { Id = "otherUser" };
		var grp = new Group { Id = "gE", ManagerId = "mgrE", Members = new List<TicketAppUser> { member1 } };
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("bE"))
							.Returns(new Dictionary<string, List<Group>> { { "newE", new List<Group> { grp } } });
		_mockBoardRepository.Setup(b => b.GetStages("bE")).Returns(new List<Stage>());
		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("pE"))
							 .ReturnsAsync(new Project { Id = "pE", Groups = new List<Group> { grp } });

		// because Members is non-empty, userRepo.GetUsersByGroupId is not called
		_mockUserRepository.Setup(u => u.GetUsersByGroupId(It.IsAny<string>()))
						   .Throws(new Exception("Should not be called"));

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "mgrE" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");
		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
							.Callback<Ticket, Ticket>((o, u) => updated = u);

		// Act
		_controller.MoveTicket("tE", "newE", "bE", "pE");

		// Assert
		Assert.Null(updated.AssignedTo);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}


	[Fact]
	public void MoveTicket_AssignedToNull_SetsSuccessMessageAndRedirects()
	{
		// Arrange
		var ticket = new Ticket { Id = "t0", BoardId = "b0", AssignedTo = null, Stage = "old" };
		_mockTicketRepository.Setup(r => r.Get("t0")).Returns(ticket);
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "u0", FirstName = "A", LastName = "B" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");

		// stub out GetStages so LogChangeIfNeeded doesn’t NRE
		_mockBoardRepository.Setup(b => b.GetStages("b0")).Returns(new List<Stage>());
		// boardStageGroups is never consulted because AssignedTo is null
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("b0")).Returns(new Dictionary<string, List<Group>>());

		SetupTempData(_controller);

		// Act
		var result = _controller.MoveTicket("t0", "new", "b0", "p0") as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_BoardStageGroupsNull_SetsSuccessMessageAndRedirects()
	{
		// Arrange
		var ticket = new Ticket { Id = "t1", BoardId = "b1", AssignedTo = "u1", Stage = "old" };
		_mockTicketRepository.Setup(r => r.Get("t1")).Returns(ticket);

		// boardStageGroups is null
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("b1"))
							.Returns((Dictionary<string, List<Group>>)null!);
		// stub out GetStages so LogChangeIfNeeded doesn’t NRE
		_mockBoardRepository.Setup(b => b.GetStages("b1")).Returns(new List<Stage>());

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "u1", FirstName = "A", LastName = "B" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");

		SetupTempData(_controller);

		// Act
		var result = _controller.MoveTicket("t1", "new", "b1", "p1") as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void MoveTicket_UserInGroup_RetainsAssignedToAndRedirects()
	{
		// Arrange
		var ticket = new Ticket { Id = "t2", BoardId = "b2", AssignedTo = "u2", Stage = "old" };
		_mockTicketRepository.Setup(r => r.Get("t2")).Returns(ticket);

		var group = new Group
		{
			Id = "g2",
			ManagerId = "mgr",
			Members = new List<TicketAppUser> { new TicketAppUser { Id = "u2" } }
		};
		_mockBoardRepository.Setup(b => b.GetBoardStageGroups("b2"))
							.Returns(new Dictionary<string, List<Group>> { { "new2", new List<Group> { group } } });

		// stub out GetStages so LogChangeIfNeeded doesn’t NRE
		_mockBoardRepository.Setup(b => b.GetStages("b2")).Returns(new List<Stage>());

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("p2"))
							 .ReturnsAsync(new Project { Id = "p2", Groups = new List<Group> { group } });
		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "mgr", FirstName = "A", LastName = "B" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");

		SetupTempData(_controller);

		Ticket updated = null!;
		_mockTicketRepository.Setup(r => r.UpdateTicket(It.IsAny<Ticket>(), It.IsAny<Ticket>()))
							 .Callback<Ticket, Ticket>((orig, upd) => updated = upd);

		// Act
		_controller.MoveTicket("t2", "new2", "b2", "p2");

		// Assert
		Assert.NotNull(updated);
		Assert.Equal("u2", updated.AssignedTo);
		Assert.Equal("Ticket moved successfully.", _controller.TempData["SuccessMessage"]);
	}

	[Fact]
	public void Details_TicketNotFound_ReturnsNotFound()
	{
		// Arrange
		_mockTicketRepository.Setup(r => r.GetTicketWithHistory("nope"))
							 .Returns((Ticket?)null!);

		// Act
		var result = _controller.Details("nope", "proj1");

		// Assert
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public void Details_ValidTicket_ReturnsViewWithCorrectModel()
	{
		// Arrange
		var histOlder = new TicketHistory { Id = "h1", ChangeDate = DateTime.UtcNow.AddMinutes(-10), ChangeDescription = "Old" };
		var histNewer = new TicketHistory { Id = "h2", ChangeDate = DateTime.UtcNow, ChangeDescription = "New" };
		var ticket = new Ticket
		{
			Id = "t1",
			Stage = "s1",
			History = new List<TicketHistory> { histOlder, histNewer }
		};

		_mockTicketRepository.Setup(r => r.GetTicketWithHistory("t1"))
							 .Returns(ticket);

		var project = new Project { Id = "proj1" };
		var board = new Board { Id = "b1" };
		var stage = new Stage { Id = "s1", Name = "StageOne" };

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("proj1"))
							  .ReturnsAsync(project);
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("proj1"))
							.ReturnsAsync(board);
		_mockBoardRepository.Setup(b => b.GetStages("b1"))
							.Returns(new List<Stage> { stage });

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "u1" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("User");

		// Act
		var result = _controller.Details("t1", "proj1") as ViewResult;
		var vm = Assert.IsType<TicketDetailsViewModel>(result!.Model);

		// Assert
		Assert.Equal("StageOne", vm.StageName);
		Assert.Equal(project, vm.Project);
		Assert.Equal(board, vm.Board);
		Assert.Same(ticket, vm.Ticket);
		Assert.Collection(vm.History,
			first => Assert.Equal(histNewer, first),
			second => Assert.Equal(histOlder, second)
		);
		Assert.NotNull(vm.CurrentUser);
		Assert.Equal("User", vm.CurrentUserRole);
	}

	[Fact]
	public void Details_StageNotFound_UsesUnknownName()
	{
		// Arrange
		var ticket = new Ticket
		{
			Id = "t2",
			Stage = "noSuchStage",
			History = new List<TicketHistory>()
		};

		_mockTicketRepository.Setup(r => r.GetTicketWithHistory("t2"))
							 .Returns(ticket);

		_mockProjectRepository.Setup(p => p.GetProjectByIdAsync("proj2"))
							  .ReturnsAsync(new Project { Id = "proj2" });
		_mockBoardRepository.Setup(b => b.GetBoardByProjectIdAsync("proj2"))
							.ReturnsAsync(new Board { Id = "b2" });
		_mockBoardRepository.Setup(b => b.GetStages("b2"))
							.Returns(new List<Stage>());

		_mockSingletonService.Setup(s => s.CurrentUser).Returns(new TicketAppUser { Id = "u2" });
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");

		// Act
		var result = _controller.Details("t2", "proj2") as ViewResult;
		var vm = Assert.IsType<TicketDetailsViewModel>(result!.Model);

		// Assert
		Assert.Equal("Unknown", vm.StageName);
		Assert.Empty(vm.History);
	}

	[Fact]
	public void DeleteTicket_RepositoryThrows_SetsErrorAndRedirects()
	{
		// Arrange
		var vm = new BoardViewModel { SelectedTicketId = "x", Project = new Project { Id = "pX" } };
		_mockTicketRepository.Setup(r => r.DeleteTicket("x")).Throws(new Exception());
		SetupTempData(_controller);

		// Act
		var result = _controller.DeleteTicket(vm) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Sorry, deleting ticket failed.", _controller.TempData["ErrorMessage"]);
		Assert.Equal("Index", result!.ActionName);
		Assert.Equal("Board", result.ControllerName);
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