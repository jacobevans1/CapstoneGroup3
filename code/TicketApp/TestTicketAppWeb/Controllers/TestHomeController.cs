using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers;

/// <summary>
/// Test the current implementation of the home controller
/// Jabesi Abwe
/// 03/08/2025
/// </summary>
public class TestHomeController
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<IGroupRepository> _mockGroupRepository;
    private readonly HomeController _controller;

    public TestHomeController()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockGroupRepository = new Mock<IGroupRepository>();
        _controller = new HomeController(_mockProjectRepository.Object, _mockGroupRepository.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithPendingApprovals()
    {
        // Arrange
        var pendingRequests = new List<GroupApprovalRequest>
    {
        new GroupApprovalRequest { ProjectId = "projectId1", GroupId = "groupId1" }
    };
        _mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync("testUserId"))
            .ReturnsAsync(pendingRequests);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PendingApprovalsViewModel>(viewResult.Model);
        Assert.Equal(pendingRequests, model.PendingRequests);
    }

    [Fact]
    public async Task GetPendingApprovalsViewModel_UserIdIsNull_ShouldReturnEmptyPendingRequests()
    {
        // Arrange
        var controller = new HomeController(_mockProjectRepository.Object, _mockGroupRepository.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        _mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<GroupApprovalRequest>());

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PendingApprovalsViewModel>(viewResult.Model);
        Assert.Empty(model.PendingRequests);
    }

    [Fact]
    public async Task GetPendingApprovals_ShouldReturnPartialViewWithPendingApprovals()
    {
        // Arrange
        var pendingRequests = new List<GroupApprovalRequest>
    {
        new GroupApprovalRequest { ProjectId = "projectId1", GroupId = "groupId1" }
    };
        _mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync("testUserId"))
            .ReturnsAsync(pendingRequests);

        // Act
        var result = await _controller.GetPendingApprovals();

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_PendingApprovalsPartial", partialViewResult.ViewName);
        var model = Assert.IsType<PendingApprovalsViewModel>(partialViewResult.Model);
        Assert.Equal(pendingRequests, model.PendingRequests);
    }

    [Fact]
    public async Task ApproveGroupForProject_ShouldReturnRedirectToActionResultOnSuccess()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task ApproveGroupForProject_ShouldReturnRedirectToActionResultOnFailure()
    {
        // Arrange
        _controller.TempData = new Mock<ITempDataDictionary>().Object;
        _mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
            .ThrowsAsync(new Exception("Approval failed"));

        // Act
        var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(null!, _controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task RejectGroupForProject_ShouldReturnRedirectToActionResultOnSuccess()
    {
        // Arrange
        var project = new Project { Id = "projectId1", LeadId = "leadId1" };
        var group = new Group { Id = "groupId1", ManagerId = "managerId1" };
        _mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("projectId1")).ReturnsAsync(project);
        _mockGroupRepository.Setup(repo => repo.GetAsync("groupId1")).ReturnsAsync(group);
        _mockProjectRepository.Setup(repo => repo.RejectGroupForProjectAsync("projectId1", "groupId1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task RejectGroupForProject_ShouldReturnRedirectToActionResultOnFailure()
    {
        // Arrange
        _controller.TempData = new Mock<ITempDataDictionary>().Object;
        var project = new Project { Id = "projectId1", LeadId = "leadId1" };
        var group = new Group { Id = "groupId1", ManagerId = "managerId1" };
        _mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("projectId1")).ReturnsAsync(project);
        _mockGroupRepository.Setup(repo => repo.GetAsync("groupId1")).ReturnsAsync(group);
        _mockProjectRepository.Setup(repo => repo.RejectGroupForProjectAsync("projectId1", "groupId1"))
            .ThrowsAsync(new Exception("Rejection failed"));

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(null!, _controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task RejectGroupForProject_ProjectIsNull_ShouldReturnRedirectToActionResult()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("projectId1")).ReturnsAsync((Project)null!);

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task RejectGroupForProject_GroupIsNull_ShouldReturnRedirectToActionResult()
    {
        // Arrange
        var project = new Project { Id = "projectId1", LeadId = "leadId1" };
        _mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("projectId1")).ReturnsAsync(project);
        _mockGroupRepository.Setup(repo => repo.GetAsync("groupId1")).ReturnsAsync((Group)null!);

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task RejectGroupForProject_LeadChangeRequired_ShouldReturnRedirectToActionResult()
    {
        // Arrange
        var project = new Project { Id = "projectId1", LeadId = "managerId1" };
        var group = new Group { Id = "groupId1", ManagerId = "managerId1" };
        _mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("projectId1")).ReturnsAsync(project);
        _mockGroupRepository.Setup(repo => repo.GetAsync("groupId1")).ReturnsAsync(group);

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("EditProject", redirectResult.ActionName);
        Assert.Equal("Project", redirectResult.ControllerName);
        Assert.True((bool?)redirectResult?.RouteValues!["leadChangeRequired"]);
    }
}



