using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;
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

			// Act
			viewModel.CurrentUserRole = "Admin";
			viewModel.User = user;
			viewModel.Users = users;
			viewModel.UserRoles = userRoles;
			viewModel.AvailableRoles = roles;
			viewModel.SelectedRoleName = "Admin";

			// Assert
			Assert.Equal("Admin", viewModel.CurrentUserRole);
			Assert.Equal(user, viewModel.User);
			Assert.Single(viewModel.Users);
			Assert.Single(viewModel.UserRoles);
			Assert.Single(viewModel.AvailableRoles);
			Assert.Equal("Admin", viewModel.SelectedRoleName);
		}

		[Fact]
		public void UserViewModel_FilteredUsers_ShouldFilterBasedOnUserNameSearchString()
		{
			// Arrange
			var viewModel = new UserViewModel();
			var users = new List<TicketAppUser>
			{
				new TicketAppUser { FirstName = "John", LastName = "Doe"},
				new TicketAppUser { FirstName = "Jane", LastName = "Smith"},
				new TicketAppUser { FirstName = "John", LastName = "Smith"}
			};

			viewModel.Users = users;

			// Act & Assert
			viewModel.UserNameSearchString = null;
			var filteredUsers = viewModel.FilteredUsers.ToList();
			Assert.Equal(3, filteredUsers.Count);

			// Act & Assert
			viewModel.UserNameSearchString = "John";
			filteredUsers = viewModel.FilteredUsers.ToList();
			Assert.Equal(2, filteredUsers.Count);

			// Act & Assert
			viewModel.UserNameSearchString = "Jane";
			filteredUsers = viewModel.FilteredUsers.ToList();
			Assert.Single(filteredUsers);

			// Act & Assert
			viewModel.UserNameSearchString = "Michael";
			filteredUsers = viewModel.FilteredUsers.ToList();
			Assert.Empty(filteredUsers);
		}
	}
}
