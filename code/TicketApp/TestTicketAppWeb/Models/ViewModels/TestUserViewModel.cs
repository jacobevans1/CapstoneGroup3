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
			Assert.Equal(0, viewModel.TotalPages);
			Assert.Equal(new int[] { 5, 10, 20, 50 }, viewModel.PageSizes);
			Assert.Equal(10, viewModel.SelectedPageSize);
			Assert.Null(viewModel.SearchTerm);
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
			viewModel.TotalPages = 5;
			viewModel.SelectedPageSize = 20;
			viewModel.SearchTerm = "test";

			// Assert
			Assert.Equal("Admin", viewModel.CurrentUserRole);
			Assert.Equal(user, viewModel.User);
			Assert.Single(viewModel.Users);
			Assert.Single(viewModel.UserRoles);
			Assert.Single(viewModel.AvailableRoles);
			Assert.Equal("Admin", viewModel.SelectedRoleName);
			Assert.Equal(route, viewModel.CurrentRoute);
			Assert.Equal(5, viewModel.TotalPages);
			Assert.Equal(20, viewModel.SelectedPageSize);
			Assert.Equal("test", viewModel.SearchTerm);
		}
	}
}
