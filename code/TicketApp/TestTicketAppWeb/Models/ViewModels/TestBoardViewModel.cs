using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Models.ViewModels
{
	public class TestBoardViewModel
	{
		[Fact]
		public void BoardViewModel_Constructor_ShouldInitializeProperties()
		{
			// Arrange
			var viewModel = new BoardViewModel();

			// Act & Assert
			Assert.NotNull(viewModel.Board);
			Assert.NotNull(viewModel.Project);
		}

		[Fact]
		public void BoardViewModel_NewStageName_ShouldBeRequired()
		{
			// Arrange
			var viewModel = new BoardViewModel();

			// Act
			viewModel.NewStageName = null;

			// Validate the view model
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(viewModel);
			var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

			// Assert
			Assert.False(isValid);
			Assert.Contains(validationResults, v => v.ErrorMessage == "Please enter a stage name");
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserProjectLeadForProject_ShouldReturnTrue_WhenUserIsLead()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user1" };
			var project = new Project { LeadId = "user1" };
			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				Project = project
			};

			// Act
			var result = viewModel.IsCurrentUserProjectLeadForProject();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserProjectLeadForProject_ShouldReturnFalse_WhenUserIsNotLead()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user1" };
			var currentUserRole = "Admin";
			var project = new Project { LeadId = "user2" };
			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				CurrentUserRole = currentUserRole,
				Project = project
			};

			// Act
			var result = viewModel.IsCurrentUserProjectLeadForProject();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserAGroupManagerInProject_ShouldReturnTrue_WhenUserIsManager()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user1" };
			var group1 = new Group { ManagerId = "user1" };
			var group2 = new Group { ManagerId = "user2" };
			var project = new Project
			{
				Groups = new List<Group> { group1, group2 }
			};

			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				Project = project
			};

			// Act
			var result = viewModel.IsCurrentUserAGroupManagerInProject();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserAGroupManagerInProject_ShouldReturnFalse_WhenUserIsNotManager()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user3" };
			var group1 = new Group { ManagerId = "user1" };
			var group2 = new Group { ManagerId = "user2" };
			var project = new Project
			{
				Groups = new List<Group> { group1, group2 }
			};

			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				Project = project
			};

			// Act
			var result = viewModel.IsCurrentUserAGroupManagerInProject();

			// Assert
			Assert.False(result);
		}
	}
}
