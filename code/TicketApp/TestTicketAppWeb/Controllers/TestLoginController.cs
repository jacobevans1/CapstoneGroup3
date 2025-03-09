using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Controllers
{
	public class TestLoginController
	{
		private readonly Mock<UserManager<TicketAppUser>> _mockUserManager;
		private readonly Mock<SignInManager<TicketAppUser>> _mockSignInManager;
		private readonly Mock<SingletonService> _mockSingletonService;
		private readonly LoginController _controller;

		public TestLoginController()
		{
			_mockUserManager = new Mock<UserManager<TicketAppUser>>(
				Mock.Of<IUserStore<TicketAppUser>>(),
				null, null, null, null, null, null, null, null);

			_mockSignInManager = new Mock<SignInManager<TicketAppUser>>(
				_mockUserManager.Object,
				Mock.Of<IHttpContextAccessor>(),
				Mock.Of<IUserClaimsPrincipalFactory<TicketAppUser>>(),
				null, null, null, null);

			_mockSingletonService = new Mock<SingletonService>();

			_controller = new LoginController(_mockSingletonService.Object, _mockSignInManager.Object, _mockUserManager.Object);
		}

		[Fact]
		public async Task Index_ShouldReturnView_WhenUsernameAndPasswordAreInvalid()
		{
			// Arrange
			var username = "invalidUser";
			var password = "wrongPassword";

			_mockUserManager.Setup(u => u.FindByNameAsync(username)).ReturnsAsync((TicketAppUser)null);

			// Act
			var result = await _controller.Index(username, password) as ViewResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ViewName);
			Assert.Equal("Invalid username or password.", result.ViewData["Error"]);
		}

		[Fact]
		public async Task Index_ShouldReturnView_WhenLoginFails()
		{
			// Arrange
			var username = "existingUser";
			var password = "wrongPassword";
			var user = new TicketAppUser { UserName = username };

			_mockUserManager.Setup(u => u.FindByNameAsync(username)).ReturnsAsync(user);
			_mockSignInManager.Setup(s => s.PasswordSignInAsync(user, password, false, false))
							  .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

			// Act
			var result = await _controller.Index(username, password) as ViewResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ViewName);
			Assert.Equal("Invalid username or password.", result.ViewData["Error"]);
		}

		[Fact]
		public async Task Index_ShouldRedirectToHome_WhenLoginSucceeds()
		{
			// Arrange
			var username = "existingUser";
			var password = "correctPassword";
			var user = new TicketAppUser { UserName = username };

			_mockUserManager.Setup(u => u.FindByNameAsync(username)).ReturnsAsync(user);
			_mockSignInManager.Setup(s => s.PasswordSignInAsync(user, password, false, false))
							  .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new System.Collections.Generic.List<string> { "User" });

			// Act
			var result = await _controller.Index(username, password) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Home", result.ControllerName);
		}

		[Fact]
		public async Task Logout_ShouldRedirectToLogin_WhenLoggedOutSuccessfully()
		{
			// Arrange
			_mockSignInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Logout() as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ActionName);
			Assert.Equal("Login", result.ControllerName);
		}

		[Fact]
		public async Task Index_ShouldReturnView_WhenUsernameOrPasswordAreEmpty()
		{
			// Arrange
			var emptyUsername = "";
			var emptyPassword = "";

			// Act
			var resultWithEmptyUsername = await _controller.Index(emptyUsername, "somePassword") as ViewResult;

			// Assert
			Assert.NotNull(resultWithEmptyUsername);
			Assert.Equal("Index", resultWithEmptyUsername.ViewName);
			Assert.Equal("Username and password are required.", resultWithEmptyUsername.ViewData["Error"]);

			// Act
			var resultWithEmptyPassword = await _controller.Index("someUsername", emptyPassword) as ViewResult;

			// Assert
			Assert.NotNull(resultWithEmptyPassword);
			Assert.Equal("Index", resultWithEmptyPassword.ViewName);
			Assert.Equal("Username and password are required.", resultWithEmptyPassword.ViewData["Error"]);

			// Act
			var resultWithBothEmpty = await _controller.Index(emptyUsername, emptyPassword) as ViewResult;

			// Assert
			Assert.NotNull(resultWithBothEmpty);
			Assert.Equal("Index", resultWithBothEmpty.ViewName);
			Assert.Equal("Username and password are required.", resultWithBothEmpty.ViewData["Error"]);
		}

		[Fact]
		public void Index_ShouldReturnView_WhenCalled()
		{
			// Act
			var result = _controller.Index() as ViewResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Index", result.ViewName);
		}

	}
}
