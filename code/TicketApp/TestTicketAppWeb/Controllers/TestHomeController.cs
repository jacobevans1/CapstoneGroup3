using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    private readonly HomeController _controller;

    public TestHomeController()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _controller = new HomeController(_mockProjectRepository.Object);

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
        var controller = new HomeController(_mockProjectRepository.Object);
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
    public async Task ApproveGroupForProject_ShouldReturnJsonResultOnSuccess()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var data = jsonResult.Value;
        Assert.True((bool)data!.GetType().GetProperty("success")!.GetValue(data, null)!);
    }

    [Fact]
    public async Task ApproveGroupForProject_ShouldReturnJsonResultOnFailure()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
            .ThrowsAsync(new Exception("Approval failed"));

        // Act
        var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var data = jsonResult.Value;
        Assert.False((bool)data!.GetType().GetProperty("success")!.GetValue(data, null)!);
        Assert.Equal("Approval failed", data!.GetType().GetProperty("message")!.GetValue(data, null));
    }

    [Fact]
    public async Task RejectGroupForProject_ShouldReturnJsonResultOnSuccess()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.RejectGroupForProjectAsync("projectId1", "groupId1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var data = jsonResult.Value;
        Assert.True((bool)data!.GetType().GetProperty("success")!.GetValue(data, null)!);
    }

    [Fact]
    public async Task RejectGroupForProject_ShouldReturnJsonResultOnFailure()
    {
        // Arrange
        _mockProjectRepository.Setup(repo => repo.RejectGroupForProjectAsync("projectId1", "groupId1"))
            .ThrowsAsync(new Exception("Rejection failed"));

        // Act
        var result = await _controller.RejectGroupForProject("projectId1", "groupId1");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var data = jsonResult.Value;
        Assert.False((bool)data!.GetType().GetProperty("success")!.GetValue(data, null)!);
        Assert.Equal("Rejection failed", data!.GetType().GetProperty("message")!.GetValue(data, null));
    }
}