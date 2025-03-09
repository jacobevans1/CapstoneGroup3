using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Models.ViewModels
{
	public class TestUserViewModel
	{
		[Fact]
		public void UserViewModel_InitializesWithDefaults()
		{
			// Arrange & Act
			var viewModel = new UserViewModel();

			// Assert
			Assert.Null(viewModel.CurrentUserRole);
			Assert.NotNull(viewModel.User);
			Assert.Empty(viewModel.Users);
			Assert.Empty(viewModel.UserRoles);
			Assert.Empty(viewModel.AvailableRoles);
			Assert.Null(viewModel.SelectedRoleName);
			Assert.NotNull(viewModel.CurrentRoute);
		}

		[Fact]
		public void UserViewModel_CanSetProperties()
		{
			// Arrange
			var viewModel = new UserViewModel();
			var user = new TicketAppUser { UserName = "testuser" };
			var users = new List<TicketAppUser> { user };
			var roles = new List<IdentityRole> { new IdentityRole("Admin") };
			var userRoles = new Dictionary<TicketAppUser, string> { { user, "Admin" } };
			var route = new UserGridData { PageNumber = 2 };

			// Act
			viewModel.CurrentUserRole = "Admin";
			viewModel.User = user;
			viewModel.Users = users;
			viewModel.UserRoles = userRoles;
			viewModel.AvailableRoles = roles;
			viewModel.SelectedRoleName = "Admin";
			viewModel.CurrentRoute = route;

			// Assert
			Assert.Equal("Admin", viewModel.CurrentUserRole);
			Assert.Equal(user, viewModel.User);
			Assert.Single(viewModel.Users);
			Assert.Single(viewModel.UserRoles);
			Assert.Single(viewModel.AvailableRoles);
			Assert.Equal("Admin", viewModel.SelectedRoleName);
			Assert.Equal(route, viewModel.CurrentRoute);
		}
	}
}
