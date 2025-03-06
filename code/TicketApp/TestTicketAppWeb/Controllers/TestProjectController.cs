/* using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Security.Claims;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers;

public class TestProjectController
{
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<IRepository<TicketAppUser>> _usersRepoMock;
    private readonly Mock<IRepository<Group>> _groupsRepoMock;
    private readonly ProjectController _controller;

    public TestProjectController()
    {
        _projectRepoMock = new Mock<IProjectRepository>();
        _usersRepoMock = new Mock<IRepository<TicketAppUser>>();
        _groupsRepoMock = new Mock<IRepository<Group>>();
        _controller = new ProjectController(
            _projectRepoMock.Object,
            _usersRepoMock.Object,
            _groupsRepoMock.Object
        );
        var tempData = new Mock<ITempDataDictionary>();
        _controller.TempData = tempData.Object;
    }

    [Fact]
    public void Index_ReturnsViewResult_WithViewModel()
    {
        // Arrange / Act
        var result = _controller.Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProjectViewModel>(viewResult.ViewData.Model);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Index_PostRedirectsToSelectGroups()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now },
            ProjectLeadId = "leadId"
        };

        // Act
        var result = _controller.Index(projectViewModel);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        // Assert
        Assert.Equal("SelectGroups", redirectResult.ActionName);
    }

    [Fact]
    public void Index_Post_HandlesNullSelectedGroupIds()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now },
            ProjectLeadId = "leadId",
            SelectedGroupIds = null!
        };

        // Act
        var result = _controller.Index(projectViewModel);

        // Assert
        Assert.Empty(projectViewModel.SelectedGroupIds);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("SelectGroups", redirectResult.ActionName);
    }

    [Fact]
    public async Task Add_Project_WithValidModel_RedirectsToIndex()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project
            {
                Id = "1",
                ProjectName = "Test Project",
                LeadId = "leadId",
                CreatedById = "userId",
                CreatedAt = DateTime.Now
            },
            ProjectLeadId = "leadId"
        };

        _projectRepoMock.Setup(repo => repo.Insert(It.IsAny<Project>())).Verifiable();
        _projectRepoMock.Setup(repo => repo.Save()).Verifiable();

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        _controller.TempData = tempData;

        // Act
        var result = await _controller.Add(projectViewModel);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Project", redirectResult.ControllerName);

        Assert.True(_controller.TempData.ContainsKey("message"));
        Assert.Equal($"Project {projectViewModel.Project.ProjectName} added successfully.", _controller.TempData["message"]);

        _projectRepoMock.Verify(repo => repo.Insert(It.IsAny<Project>()), Times.Once);
        _projectRepoMock.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Add_Project_WithUserIdentityNull_SetsCreatedByIdToNull()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project
            {
                Id = "1",
                ProjectName = "Test Project",
                LeadId = "leadId",
                CreatedById = null,
                CreatedAt = DateTime.Now
            }
        };

        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(ctx => ctx.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        _projectRepoMock.Setup(repo => repo.Insert(It.IsAny<Project>())).Verifiable();
        _projectRepoMock.Setup(repo => repo.Save()).Verifiable();

        // Act
        var result = await _controller.Add(projectViewModel);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Project", redirectResult.ControllerName);

        Assert.Null(projectViewModel.Project.CreatedById);

        _projectRepoMock.Verify(repo => repo.Insert(It.IsAny<Project>()), Times.Once);
        _projectRepoMock.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Add_Project_WithInvalidModel_ReturnsViewWithValidationErrors()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project { Id = "1", ProjectName = "", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now }
        };

        _controller.ModelState.AddModelError("Project.ProjectName", "Project name is required");

        // Act
        var result = await _controller.Add(projectViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Add", viewResult.ViewName);
        Assert.True(_controller.ModelState.ContainsKey("Project.ProjectName"));

        _projectRepoMock.Verify(repo => repo.Insert(It.IsAny<Project>()), Times.Never);
        _projectRepoMock.Verify(repo => repo.Save(), Times.Never);
    }

    [Fact]
    public void Edit_ReturnsViewResult_WithProject()
    {
        // Arrange
        var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now };
        _projectRepoMock.Setup(repo => repo.Get("1")).Returns(project);

        // Act
        var result = _controller.Edit("1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProjectViewModel>(viewResult.ViewData.Model);
        Assert.Equal(project, model.Project);
    }

    [Fact]
    public void Edit_PostRedirectsToIndex()
    {
        // Arrange 
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now }
        };

        _projectRepoMock.Setup(repo => repo.Update(It.IsAny<Project>())).Verifiable();
        _projectRepoMock.Setup(repo => repo.Save()).Verifiable();

        // Act
        var result = _controller.Edit(projectViewModel);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _projectRepoMock.Verify();
    }

    [Fact]
    public void Edit_Post_ReturnsViewWithInvalidModel()
    {
        // Arrange
        var projectViewModel = new ProjectViewModel
        {
            Project = new Project { Id = "1", ProjectName = "", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now }
        };

        _controller.ModelState.AddModelError("Project.ProjectName", "Project name is required");

        // Act
        var result = _controller.Edit(projectViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);

        // Assert
        Assert.True(_controller.ModelState.ContainsKey("Project.ProjectName"));
        Assert.Equal("Edit", viewResult.ViewName);

        _projectRepoMock.Verify(repo => repo.Update(It.IsAny<Project>()), Times.Never);
        _projectRepoMock.Verify(repo => repo.Save(), Times.Never);
    }


    [Fact]
    public void Edit_Get_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _projectRepoMock.Setup(repo => repo.Get(It.IsAny<string>())).Returns((Project?)null);

        // Act
        var result = _controller.Edit("invalid-id");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ReturnsViewResult_WithProject()
    {
        // Arrange
        var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now };
        _projectRepoMock.Setup(repo => repo.Get("1")).Returns(project);

        // Act
        var result = _controller.Delete("1");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Project>(viewResult.ViewData.Model);

        // Assert
        Assert.Equal(project, model);
    }

    [Fact]
    public void Delete_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepoMock.Setup(repo => repo.Get("invalid-id")).Returns((Project?)null);

        // Act
        var result = _controller.Delete("invalid-id");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteConfirmed_ReturnsRedirectToIndex()
    {
        // Arrange
        var project = new Project { Id = "1", ProjectName = "Test Project", LeadId = "leadId", CreatedById = "userId", CreatedAt = DateTime.Now };
        _projectRepoMock.Setup(repo => repo.Get("1")).Returns(project);
        _projectRepoMock.Setup(repo => repo.Delete(It.IsAny<Project>())).Verifiable();
        _projectRepoMock.Setup(repo => repo.Save()).Verifiable();

        // Act
        var result = _controller.DeleteConfirmed("1");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        // Assert
        Assert.Equal("Index", redirectResult.ActionName);

        _projectRepoMock.Verify();
    }

    [Fact]
    public void DeleteConfirmed_NonExistingProject_DoesNotThrow()
    {
        // Arrange
        _projectRepoMock.Setup(repo => repo.Get(It.IsAny<string>())).Returns((Project?)null);

        // Act
        var result = _controller.DeleteConfirmed("invalid-id");

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public void SelectGroups_LoadsGroups_AndFiltersAvailableLeads()
    {
        // Arrange
        var group1 = new Group { Id = "G1", ManagerId = "M1", Members = new List<TicketAppUser> { new TicketAppUser { Id = "M1" } } };
        var group2 = new Group { Id = "G2", ManagerId = "M2", Members = new List<TicketAppUser> { new TicketAppUser { Id = "M2" } } };

        var selectedGroupIds = new[] { "G1", "G2" };

        _groupsRepoMock.Setup(repo => repo.Get("G1")).Returns(group1);
        _groupsRepoMock.Setup(repo => repo.Get("G2")).Returns(group2);

        // Act
        var result = _controller.SelectGroups(selectedGroupIds);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProjectViewModel>(viewResult.Model);

        Assert.Equal(2, model.AvailableGroupLeads.Count());
        Assert.Contains(model.AvailableGroupLeads, lead => lead.Id == "M1");
        Assert.Contains(model.AvailableGroupLeads, lead => lead.Id == "M2");
    }

    [Fact]
    public void SelectGroups_ReturnsView_WhenNoGroupsAreSelected()
    {
        // Arrange
        var selectedGroupIds = new string[] { };
        _groupsRepoMock.Setup(repo => repo.List(It.IsAny<QueryOptions<Group>>())).Returns(new List<Group>());

        // Act
        var result = _controller.SelectGroups(selectedGroupIds);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProjectViewModel>(viewResult.Model);
        Assert.Empty(model.AvailableGroupLeads);
    }

    [Fact]
    public void List_Get_Returns_Projects_With_Options()
    {
        // Arrange
        var gridData = new ProjectGridData { SortDirection = "asc", PageNumber = 1, PageSize = 10 };
        _projectRepoMock.Setup(repo => repo.List(It.IsAny<QueryOptions<Project>>()))
            .Returns(new List<Project> { new Project { ProjectName = "Test Project" } });

        // Act
        var result = _controller.List(gridData);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProjectViewModel>(viewResult.Model);
        Assert.NotEmpty(model.Projects);
    }

    [Fact]

    public void List_SortsByProjectLead_WhenIsSortByProjectLeadIsTrue()
    {
        // Arrange
        var gridData = new ProjectGridData
        {
            SortField = nameof(TicketAppUser),

            SortDirection = "asc",

            PageNumber = 1,

            PageSize = 10
        };

        var projects = new List<Project>

        {
            new Project { ProjectName = "Project X", Lead = new TicketAppUser { FirstName = "Charlie", LastName = "Brown" } },

            new Project { ProjectName = "Project Y", Lead = new TicketAppUser { FirstName = "Alice", LastName = "Smith" } },

            new Project { ProjectName = "Project Z", Lead = new TicketAppUser { FirstName = "Bob", LastName = "Johnson" } }
        };

        _projectRepoMock.Setup(repo => repo.List(It.IsAny<QueryOptions<Project>>()))

            .Returns(projects.OrderBy(p => p.Lead?.FullName).ToList());

        // Act

        var result = _controller.List(gridData);

        // Assert

        var viewResult = Assert.IsType<ViewResult>(result);

        var model = Assert.IsType<ProjectViewModel>(viewResult.Model, exactMatch: false);

        Assert.Equal("Alice Smith", model.Projects.First()?.Lead?.FullName);

        Assert.Equal("Charlie Brown", model.Projects.Last()?.Lead?.FullName);

    }

    [Fact]
    public void PageSizes_RedirectsToIndex_WithCorrectRouteParameters()
    {
        // Arrange
        var currentRoute = new ProjectGridData
        {
            SortDirection = "asc",
            PageNumber = 1,
            PageSize = 10,
            SortField = nameof(TicketAppUser),
        };

        // Act
        var result = _controller.PageSizes(currentRoute);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        var routeValues = Assert.IsType<RouteValueDictionary>(redirectResult.RouteValues);

        Assert.Equal("asc", routeValues["SortDirection"]);
        Assert.Equal(1, Convert.ToInt32(routeValues["PageNumber"])); 
        Assert.Equal(10, Convert.ToInt32(routeValues["PageSize"]));
    }
}*/
