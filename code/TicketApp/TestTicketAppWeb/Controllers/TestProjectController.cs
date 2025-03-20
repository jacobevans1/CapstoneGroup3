using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

/// <summary>
/// Tests the project controller
/// Jabesi Abwe
/// 03/08/2025
/// </summary>
public class ProjectControllerTests
{
	private readonly Mock<IProjectRepository> _mockRepo;
	private readonly ProjectController _controller;
	private readonly ClaimsPrincipal _user;
	private readonly SingletonService _ton;

	public ProjectControllerTests()
	{
		_mockRepo = new Mock<IProjectRepository>();
		_ton = new SingletonService();
		_controller = new ProjectController(_ton, _mockRepo.Object);

		// Simulate a logged-in user
		var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test-user") };
		var identity = new ClaimsIdentity(claims, "TestAuth");
		_user = new ClaimsPrincipal(identity);
		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext { User = _user }
		};
	}

	[Fact]
	public void Index_ReturnsViewResult_WithEmptyProjects()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetFilteredProjectsAndGroups(It.IsAny<string>(), It.IsAny<string>()))
				 .ReturnsAsync(new Dictionary<Project, List<Group>>());

		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(new List<Group>());

		// Act
		var result = _controller.Index(null, null) as ViewResult;

		// Assert
		Assert.NotNull(result);
		var model = Assert.IsType<ProjectViewModel>(result.Model);
		Assert.Empty(model.Projects);
	}


	[Fact]
	public void Index_ReturnsViewResult_WithViewModel()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetFilteredProjectsAndGroups(It.IsAny<string>(), It.IsAny<string>()))
				 .ReturnsAsync(new Dictionary<Project, List<Group>>());

		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(new List<Group>());

		// Act
		var result = _controller.Index(null, null) as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.IsType<ProjectViewModel>(result.Model);
	}

	[Fact]
	public async Task AddProject_ReturnsViewResult_WithAvailableGroups()
	{
		// Arrange
		var availableGroups = new List<Group> { new Group { Id = "1", GroupName = "Group A" } };
		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(availableGroups);

		// Act
		var result = await _controller.AddProject() as ViewResult;

		// Assert
		Assert.NotNull(result);
		var model = Assert.IsType<ProjectViewModel>(result.Model);
		Assert.Single(model.AvailableGroups);
		Assert.Equal("Group A", model.AvailableGroups[0].GroupName);
	}

	[Fact]
	public async Task CreateProject_DatabaseFailure_ReturnsViewWithErrorMessage()
	{
		// Arrange
		var model = new ProjectViewModel
		{
			ProjectName = "New Project",
			Description = "Test Description",
			ProjectLeadId = "lead-id",
			SelectedGroupIds = new List<string> { "group1" }
		};

		_mockRepo.Setup(r => r.AddProjectAsync(It.IsAny<Project>(), It.IsAny<List<string>>(), false))
				 .ThrowsAsync(new Exception("Database error"));

		// Act
		var result = await _controller.CreateProject(model) as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("AddProject", result.ViewName);
	}

	[Fact]
	public async Task CreateProject_InvalidModel_ReturnsViewWithSameModel()
	{
		// Arrange
		_controller.ModelState.AddModelError("ProjectName", "Required");

		var model = new ProjectViewModel();
		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(new List<Group>());

		// Act
		var result = await _controller.CreateProject(model) as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("AddProject", result.ViewName);
	}

	[Fact]
	public async Task CreateProject_ValidModel_CallsAddProjectAsync_AndRedirectsToIndex()
	{
		// Arrange
		var model = new ProjectViewModel
		{
			ProjectName = "New Project",
			Description = "Test Description",
			ProjectLeadId = "lead-id",
			SelectedGroupIds = new List<string> { "group1" }
		};

		_mockRepo.Setup(r => r.AddProjectAsync(It.IsAny<Project>(), It.IsAny<List<string>>(), false))
				 .Returns(Task.CompletedTask);

		var controller = new ProjectController(_ton, _mockRepo.Object)
		{
			TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
		};

		// Mock User Claims
		var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
		{
		new Claim(ClaimTypes.NameIdentifier, "user-id"),
		new Claim(ClaimTypes.Role, "User")
	}, "mock"));

		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext { User = user }
		};

		// Act
		var result = await controller.CreateProject(model) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result.ActionName);

		_mockRepo.Verify(r => r.AddProjectAsync(It.IsAny<Project>(), model.SelectedGroupIds, false), Times.Once);
	}

	[Fact]
	public async Task EditProject_Get_InvalidId_ReturnsNotFound()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetProjectByIdAsync("invalid-id")).ReturnsAsync((Project)null!);

		// Act
		var result = await _controller.EditProject("invalid-id") as NotFoundResult;

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task EditProject_Post_ProjectDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetProjectByIdAsync("1")).ReturnsAsync((Project)null!);

		var model = new ProjectViewModel { ProjectName = "Updated Project" };

		// Act
		var result = await _controller.EditProject(model, "1") as NotFoundResult;

		// Assert
		Assert.NotNull(result);
	}

    [Fact]
    public async Task EditProject_Post_DatabaseFailure_ReturnsViewWithErrorMessage()
    {
        // Arrange
        _controller.TempData = new Mock<ITempDataDictionary>().Object;
        var project = new Project
        {
            Id = "1",
            ProjectName = "Test Project"
        };

        _mockRepo.Setup(r => r.GetProjectByIdAsync("1")).ReturnsAsync(project);
        _mockRepo.Setup(r => r.UpdateProjectAsync(It.IsAny<Project>(), It.IsAny<List<string>>(), false))
                 .ThrowsAsync(new Exception("Database error"));

        var model = new ProjectViewModel
        {
            ProjectName = "Updated Project",
            SelectedGroupIds = new List<string> { "group1" }
        };

        // Act
        var result = await _controller.EditProject(model, "1") as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(null!, result.ViewName);
        Assert.Equal(null!, _controller.TempData["ErrorMessage"]);
    }

    [Fact]
	public async Task EditProject_Get_ReturnsViewWithModel_WhenProjectExists()
	{
		// Arrange
		var project = new Project
		{
			Id = "1",
			ProjectName = "Test Project",
			Description = "Description",
			LeadId = "lead-id",
			Groups = new List<Group> { new Group { Id = "group1", GroupName = "Group A" } }
		};

		_mockRepo.Setup(r => r.GetProjectByIdAsync("1")).ReturnsAsync(project);
		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(new List<Group>());

		// Act
		var result = await _controller.EditProject("1") as ViewResult;

		// Assert
		Assert.NotNull(result);
		var model = Assert.IsType<ProjectViewModel>(result.Model);
		Assert.Equal("Test Project", model.ProjectName);
		Assert.Single(model.SelectedGroupIds);
	}

	[Fact]
	public async Task EditProject_Post_InvalidModel_ReturnsView()
	{
		// Arrange
		var model = new ProjectViewModel();
		_controller.ModelState.AddModelError("ProjectName", "Required");

		_mockRepo.Setup(r => r.GetAvailableGroupsAsync()).ReturnsAsync(new List<Group>());

		// Act
		var result = await _controller.EditProject(model, "1") as ViewResult;

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task EditProject_Post_ValidModel_CallsUpdateProjectAsync_AndRedirectsToIndex()
	{
		// Arrange
		var project = new Project
		{
			Id = "1",
			ProjectName = "Test Project",
			Description = "Description",
			LeadId = "lead-id"
		};

		_mockRepo.Setup(r => r.GetProjectByIdAsync("1"))
				 .ReturnsAsync(project);

		_mockRepo.Setup(r => r.UpdateProjectAsync(It.IsAny<Project>(), It.IsAny<List<string>>(), false))
				 .Returns(Task.CompletedTask);

		var model = new ProjectViewModel
		{
			ProjectName = "Updated Project",
			Description = "Updated Description",
			ProjectLeadId = "lead-id",
			SelectedGroupIds = new List<string> { "group1" }
		};

		var controller = new ProjectController(_ton, _mockRepo.Object)
		{
			TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
		};

		// Mock User Claims
		var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
		{
		new Claim(ClaimTypes.NameIdentifier, "user-id"),
		new Claim(ClaimTypes.Role, "User")
	}, "mock"));

		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext { User = user }
		};

		// Act
		var result = await controller.EditProject(model, "1") as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result.ActionName);

		_mockRepo.Verify(r => r.UpdateProjectAsync(It.IsAny<Project>(), model.SelectedGroupIds, false), Times.Once);
	}

	[Fact]
	public async Task DeleteProject_InvalidId_ReturnsNotFound()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetProjectByIdAsync("invalid-id")).ReturnsAsync((Project)null!);

		// Act
		var result = await _controller.DeleteProject("invalid-id") as NotFoundResult;

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task ConfirmDelete_InvalidId_ReturnsNotFound()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetProjectByIdAsync("invalid-id")).ReturnsAsync((Project)null!);

		// Act
		var result = await _controller.ConfirmDelete("invalid-id") as NotFoundResult;

		// Assert
		Assert.NotNull(result);
	}


	[Fact]
	public async Task DeleteProject_ValidId_ReturnsView()
	{
		// Arrange
		var project = new Project { Id = "1", ProjectName = "Test Project" };
		_mockRepo.Setup(r => r.GetProjectByIdAsync("1")).ReturnsAsync(project);

		// Act
		var result = await _controller.DeleteProject("1") as ViewResult;

		// Assert
		Assert.NotNull(result);
		Assert.IsType<Project>(result.Model);
	}

	[Fact]
	public async Task ConfirmDelete_ValidId_CallsDeleteProjectAsync_AndRedirectsToIndex()
	{
		// Arrange
		var project = new Project { Id = "1", ProjectName = "Test Project" };

		_mockRepo.Setup(r => r.GetProjectByIdAsync("1"))
				 .ReturnsAsync(project);

		_mockRepo.Setup(r => r.DeleteProjectAsync(It.IsAny<Project>()))
				 .Returns(Task.CompletedTask);

		var controller = new ProjectController(_ton, _mockRepo.Object)
		{
			TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
		};

		// Act
		var result = await controller.ConfirmDelete("1") as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result.ActionName);

		_mockRepo.Verify(r => r.DeleteProjectAsync(project), Times.Once);
	}

	[Fact]
	public async Task GetGroupLeads_EmptyGroupIds_ReturnsEmptyList()
	{
		// Arrange
		var controller = new ProjectController(_ton, _mockRepo.Object);

		// Act
		var result = await controller.GetGroupLeads(null!) as JsonResult;

		// Assert
		Assert.NotNull(result);
		var jsonData = result.Value as IEnumerable<object>;
		Assert.Empty(jsonData!);  // Expected empty list
	}

	[Fact]
	public async Task GetGroupLeads_ValidGroupIds_ReturnsLeads()
	{
		// Arrange
		var groupIds = "1,2,3";
		var leads = new List<TicketAppUser>
	{
		new TicketAppUser { Id = "1", FirstName = "John", LastName = "Doe" },
		new TicketAppUser { Id = "2", FirstName = "Jane", LastName = "Doe" }
	};

		_mockRepo.Setup(r => r.GetGroupLeadsAsync(It.IsAny<List<string>>()))
				 .ReturnsAsync(leads);

		var controller = new ProjectController(_ton, _mockRepo.Object);

		// Act
		var result = await controller.GetGroupLeads(groupIds) as JsonResult;

		// Assert
		Assert.NotNull(result);
		var jsonData = result.Value as IEnumerable<object>;
		Assert.NotNull(jsonData);
		Assert.Equal(2, jsonData.Count());
	}

}
