using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TicketAppWeb.Controllers;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Controllers;

public class TestUserController
{
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly Mock<SingletonService> _mockSingletonService;
	private readonly UserController _controller;

	public TestUserController()
	{
		_mockUserRepository = new Mock<IUserRepository>();
		_mockSingletonService = new Mock<SingletonService>();
		_controller = new UserController(_mockSingletonService.Object, _mockUserRepository.Object);

		_controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
	}

	// Test the Index method
	[Fact]
	public void Index_ShouldReturnViewResult()
	{
		// Arrange
		var vm = new UserViewModel();
		_mockSingletonService.Setup(s => s.CurrentUserRole).Returns("Admin");
		_mockUserRepository.Setup(r => r.List(It.IsAny<QueryOptions<TicketAppUser>>())).Returns(new List<TicketAppUser>());
		_mockUserRepository.Setup(r => r.GetRolesAsync())
			.ReturnsAsync(new List<IdentityRole>
			{
				new IdentityRole { Name = "Admin" },
				new IdentityRole { Name = "User" }
			}.AsEnumerable());

		_mockUserRepository.Setup(r => r.GetUserRolesAsync()).ReturnsAsync(new Dictionary<TicketAppUser, string>());

		// Act
		var result = _controller.Index(null);

		// Assert
		var viewResult = Assert.IsType<ViewResult>(result);
		var viewModel = Assert.IsAssignableFrom<UserViewModel>(viewResult.Model);
		Assert.Equal("Admin", viewModel.CurrentUserRole);
	}

	// Test CreateUser (GET)
	[Fact]
	public void CreateUser_ShouldReturnViewResult()
	{
		// Act
		var result = _controller.CreateUser();

		// Assert
		var viewResult = Assert.IsType<ViewResult>(result);
	}

	// Test CreateUser (POST)
	[Fact]
	public async Task CreateUser_ShouldRedirectToIndex_WhenModelStateIsValid()
	{
		// Arrange
		var userViewModel = new UserViewModel
		{
			User = new TicketAppUser { UserName = "newuser" },
			SelectedRoleName = "Admin"
		};

		_mockUserRepository.Setup(r => r.CreateUser(It.IsAny<TicketAppUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);
		_mockUserRepository.Setup(r => r.Save()).Verifiable();

		// Act
		var result = await _controller.CreateUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		_mockUserRepository.Verify(r => r.Save(), Times.Once);
	}

	[Fact]
	public async Task CreateUser_ShouldReturnErrorMessage_WhenModelStateIsInvalid()
	{
		// Arrange
		_controller.ModelState.AddModelError("UserName", "Required");
		var userViewModel = new UserViewModel();

		// Act
		var result = await _controller.CreateUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		Assert.Equal("Sorry, user creation failed.", _controller.TempData["ErrorMessage"]);
	}

	// Test EditUser (GET)
	[Fact]
	public void GetUserData_ShouldReturnJson_WhenUserExists()
	{
		// Arrange
		var userId = "123";
		var user = new TicketAppUser { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "123-456-7890" };

		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(user);
		_mockUserRepository.Setup(r => r.GetUserRolesAsync()).ReturnsAsync(new Dictionary<TicketAppUser, string> { { user, "Admin" } });

		// Act
		var result = _controller.GetUserData(userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		var value = jsonResult.Value;

		var properties = value?.GetType().GetProperties();

		Assert.Contains(properties!, p => p.Name == "firstName");
		Assert.Contains(properties!, p => p.Name == "lastName");
		Assert.Contains(properties!, p => p.Name == "email");
		Assert.Contains(properties!, p => p.Name == "phoneNumber");
		Assert.Contains(properties!, p => p.Name == "roleId");

		Assert.Equal(user.FirstName, properties?.Single(p => p.Name == "firstName").GetValue(value));
		Assert.Equal(user.LastName, properties?.Single(p => p.Name == "lastName").GetValue(value));
		Assert.Equal(user.Email, properties?.Single(p => p.Name == "email").GetValue(value));
		Assert.Equal(user.PhoneNumber, properties?.Single(p => p.Name == "phoneNumber").GetValue(value));
		Assert.Equal("Admin", properties?.Single(p => p.Name == "roleId").GetValue(value));
	}

	// Test DeleteUser (GET)
	[Fact]
	public void DeleteUser_ShouldReturnJson_WhenUserExists()
	{
		// Arrange
		var userId = "123";
		var user = new TicketAppUser { FirstName = "John", LastName = "Doe" };

		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(user);

		// Act
		var result = _controller.DeleteUser(userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		var value = jsonResult.Value;

		var properties = value!.GetType().GetProperties();
		Assert.Contains(properties, p => p.Name == "fullName");
		Assert.Equal(user.FullName, properties.Single(p => p.Name == "fullName").GetValue(value));
	}


	// Test DeleteConfirmed
	[Fact]
	public void DeleteConfirmed_ShouldRedirectToIndex_WhenUserDeleted()
	{
		// Arrange
		var userId = "123";
		var user = new TicketAppUser { FirstName = "John", LastName = "Doe" };
		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(user);
		_mockUserRepository.Setup(r => r.Delete(It.IsAny<TicketAppUser>())).Verifiable();
		_mockUserRepository.Setup(r => r.Save()).Verifiable();

		// Act
		var result = _controller.DeleteConfirmed(userId);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		_mockUserRepository.Verify(r => r.Save(), Times.Once);
	}

	// Test for handling invalid user in GetUserData
	[Fact]
	public void GetUserData_ShouldRedirectToIndex_WhenUserNotFound()
	{
		// Arrange
		var userId = "123";
		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns((TicketAppUser?)null);

		// Act
		var result = _controller.GetUserData(userId);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		_mockUserRepository.Verify(r => r.Get(It.IsAny<string>()), Times.Once);
	}

	// Test for handling null user in DeleteUser
	[Fact]
	public void DeleteUser_ShouldReturnBadRequest_WhenUserIdIsNull()
	{
		// Act
		var result = _controller.DeleteUser(null!);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("User ID is required.", badRequestResult.Value);
	}

	// Test for exception handling in CreateUser (POST)
	[Fact]
	public async Task CreateUser_ShouldReturnError_WhenExceptionIsThrown()
	{
		// Arrange
		var userViewModel = new UserViewModel { User = new TicketAppUser { FirstName = "John", LastName = "Doe" } };
		_mockUserRepository.Setup(r => r.CreateUser(It.IsAny<TicketAppUser>(), It.IsAny<string>())).Throws(new Exception("Database error"));

		// Act
		var result = await _controller.CreateUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);

		// Ensure that the TempData contains the expected error message
		var errorMessage = _controller.TempData["ErrorMessage"] as string;
		Assert.Equal("Sorry, user update failed.", errorMessage);
	}

	[Fact]
	public async Task EditUser_ShouldReturnError_WhenUserIsNull()
	{
		// Arrange
		var userViewModel = new UserViewModel { User = null! };

		// Act
		var result = await _controller.EditUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		Assert.Equal("Sorry, user update failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
	{
		// Arrange
		var userId = "invalidUserId";
		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns((TicketAppUser?)null);

		// Act
		var result = _controller.DeleteConfirmed(userId);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		Assert.Equal("User not found.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void DeleteUser_ShouldReturnBadRequest_WhenUserIdIsEmpty()
	{
		// Act
		var result = _controller.DeleteUser(string.Empty);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("User ID is required.", badRequestResult.Value);
	}


	[Fact]
	public void DeleteConfirmed_ShouldReturnError_WhenExceptionIsThrown()
	{
		// Arrange
		var userId = "123";
		var user = new TicketAppUser { FirstName = "John", LastName = "Doe" };
		_mockUserRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(user);
		_mockUserRepository.Setup(r => r.Delete(It.IsAny<TicketAppUser>())).Throws(new Exception("Database error"));

		// Act
		var result = _controller.DeleteConfirmed(userId);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		Assert.Equal("Sorry, deleting user failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public async Task CreateUser_ShouldAssignCorrectRole_WhenRoleIsProvided()
	{
		// Arrange
		var userViewModel = new UserViewModel
		{
			User = new TicketAppUser { UserName = "newuser" },
			SelectedRoleName = "Admin"
		};

		_mockUserRepository.Setup(r => r.CreateUser(It.IsAny<TicketAppUser>(), "Admin")).Returns(Task.CompletedTask);
		_mockUserRepository.Setup(r => r.Save()).Verifiable();

		// Act
		var result = await _controller.CreateUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		_mockUserRepository.Verify(r => r.Save(), Times.Once);
	}

	[Fact]
	public async Task EditUser_ShouldReturnError_WhenExceptionIsThrown()
	{
		// Arrange
		var userViewModel = new UserViewModel { User = new TicketAppUser { FirstName = "John", LastName = "Doe" } };
		_mockUserRepository.Setup(r => r.UpdateUser(It.IsAny<TicketAppUser>(), It.IsAny<string>())).Throws(new Exception("Database error"));

		// Act
		var result = await _controller.EditUser(userViewModel);

		// Assert
		var redirectResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectResult.ActionName);
		Assert.Equal("User", redirectResult.ControllerName);
		Assert.Equal("Sorry, user update failed.", _controller.TempData["ErrorMessage"]);
	}

	[Fact]
	public void DeleteConfirmed_ShouldRedirectToIndex_WhenUserIdIsNullOrEmpty()
	{
		// Act
		var result = _controller.DeleteConfirmed(string.Empty) as RedirectToActionResult;

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Index", result.ActionName);
		Assert.Equal("User", result.ControllerName);
		Assert.Equal("Invalid user ID.", _controller.TempData["ErrorMessage"]);
	}
}
