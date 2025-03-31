using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers
{
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

			_controller.TempData = new TempDataDictionary(
				new DefaultHttpContext(),
				Mock.Of<ITempDataProvider>()
			);
		}



		[Fact]
		public async Task Index_ShouldReturnViewWithPendingApprovals()
		{
			var pendingRequests = new List<GroupApprovalRequest>
			{
				new GroupApprovalRequest { ProjectId = "projectId1", GroupId = "groupId1" }
			};

			_mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync("testUserId"))
				.ReturnsAsync(pendingRequests);

			var result = await _controller.Index();

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<PendingApprovalsViewModel>(viewResult.Model);
			Assert.Equal(pendingRequests, model.PendingRequests);
		}

		[Fact]
		public async Task GetPendingApprovalsViewModel_UserIdIsNull_ShouldReturnEmptyPendingRequests()
		{
			var controller = new HomeController(_mockProjectRepository.Object, _mockGroupRepository.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
			};

			_mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync(It.IsAny<string>()))
				.ReturnsAsync(new List<GroupApprovalRequest>());

			var result = await controller.Index();

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<PendingApprovalsViewModel>(viewResult.Model);
			Assert.Empty(model.PendingRequests);
		}

		[Fact]
		public async Task GetPendingApprovals_ShouldReturnPartialViewWithPendingApprovals()
		{
			var pendingRequests = new List<GroupApprovalRequest>
			{
				new GroupApprovalRequest { ProjectId = "projectId1", GroupId = "groupId1" }
			};

			_mockProjectRepository.Setup(repo => repo.GetPendingGroupApprovalRequestsAsync("testUserId"))
				.ReturnsAsync(pendingRequests);

			var result = await _controller.GetPendingApprovals();

			var partialViewResult = Assert.IsType<PartialViewResult>(result);
			Assert.Equal("_PendingApprovalsPartial", partialViewResult.ViewName);
			var model = Assert.IsType<PendingApprovalsViewModel>(partialViewResult.Model);
			Assert.Equal(pendingRequests, model.PendingRequests);
		}

		[Fact]
		public async Task ApproveGroupForProject_ShouldRedirectToIndex_OnSuccess()
		{
			_mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
				.Returns(Task.CompletedTask);

			var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}

		[Fact]
		public async Task ApproveGroupForProject_ShouldRedirectToIndex_OnFailure()
		{
			_mockProjectRepository.Setup(repo => repo.ApproveGroupForProjectAsync("projectId1", "groupId1"))
				.ThrowsAsync(new Exception("Approval failed"));

			var result = await _controller.ApproveGroupForProject("projectId1", "groupId1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}

		[Fact]
		public async Task RejectGroupForProject_ShouldRedirectToIndex_WhenProjectIsNull()
		{
			_mockProjectRepository.Setup(repo => repo.GetProjectByIdAsync("p1"))
				.ReturnsAsync((Project)null);

			var result = await _controller.RejectGroupForProject("p1", "g1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}

		[Fact]
		public async Task RejectGroupForProject_ShouldRedirectToIndex_WhenGroupIsNull()
		{
			var project = new Project { Id = "p1", LeadId = "lead1" };
			_mockProjectRepository.Setup(r => r.GetProjectByIdAsync("p1")).ReturnsAsync(project);
			_mockGroupRepository.Setup(r => r.GetAsync("g1")).ReturnsAsync((Group)null);

			var result = await _controller.RejectGroupForProject("p1", "g1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}

		[Fact]
		public async Task RejectGroupForProject_ShouldRedirectToEditProject_WhenManagerIsLead()
		{
			var projectId = "p1";
			var groupId = "g1";
			var leadId = "lead123";

			var project = new Project { Id = projectId, LeadId = leadId };
			var group = new Group { Id = groupId, ManagerId = leadId };

			_mockProjectRepository.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
			_mockGroupRepository.Setup(r => r.GetAsync(groupId)).ReturnsAsync(group);

			var result = await _controller.RejectGroupForProject(projectId, groupId);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("EditProject", redirect.ActionName);
			Assert.Equal("Project", redirect.ControllerName);
			Assert.True((bool)redirect.RouteValues["leadChangeRequired"]);
		}

		[Fact]
		public async Task RejectGroupForProject_ShouldRedirectToIndex_WhenRejectionSuccessful()
		{
			var project = new Project { Id = "p1", LeadId = "lead1" };
			var group = new Group { Id = "g1", ManagerId = "notLead" };

			_mockProjectRepository.Setup(r => r.GetProjectByIdAsync("p1")).ReturnsAsync(project);
			_mockGroupRepository.Setup(r => r.GetAsync("g1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.RejectGroupForProjectAsync("p1", "g1")).Returns(Task.CompletedTask);

			var result = await _controller.RejectGroupForProject("p1", "g1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}

		[Fact]
		public async Task RejectGroupForProject_ShouldRedirectToIndex_WhenExceptionThrown()
		{
			var project = new Project { Id = "p1", LeadId = "lead1" };
			var group = new Group { Id = "g1", ManagerId = "notLead" };

			_mockProjectRepository.Setup(r => r.GetProjectByIdAsync("p1")).ReturnsAsync(project);
			_mockGroupRepository.Setup(r => r.GetAsync("g1")).ReturnsAsync(group);
			_mockProjectRepository.Setup(r => r.RejectGroupForProjectAsync("p1", "g1"))
				.ThrowsAsync(new Exception("Some error"));

			var result = await _controller.RejectGroupForProject("p1", "g1");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirect.ActionName);
			Assert.Equal("Home", redirect.ControllerName);
		}
	}
}
