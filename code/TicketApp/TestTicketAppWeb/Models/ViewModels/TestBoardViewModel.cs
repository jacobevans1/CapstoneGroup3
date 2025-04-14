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

		[Fact]
		public void BoardViewModel_AvailableGroups_ShouldReturnCorrectGroups()
		{
			// Arrange
			var group1 = new Group { Id = "group1" };
			var group2 = new Group { Id = "group2" };
			var group3 = new Group { Id = "group3" };

			var project = new Project
			{
				Groups = new List<Group> { group1, group2, group3 }
			};

			var assignedGroups = new Dictionary<string, List<Group>>
		{
			{ "stage1", new List<Group> { group1 } }
		};

			var viewModel = new BoardViewModel
			{
				Project = project,
				AssignedGroups = assignedGroups
			};

			// Act
			var availableGroups = viewModel.AvailableGroups;

			// Assert
			Assert.Contains(group2, availableGroups);
			Assert.Contains(group3, availableGroups);
			Assert.DoesNotContain(group1, availableGroups);
		}

		[Fact]
		public void BoardViewModel_NewDescription_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new BoardViewModel();
			var description = "New stage description";

			// Act
			viewModel.NewDescription = description;

			// Assert
			Assert.Equal(description, viewModel.NewDescription);
		}

		[Fact]
		public void BoardViewModel_SelectedStageId_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new BoardViewModel();
			var selectedStageId = "stage1";

			// Act
			viewModel.SelectedStageId = selectedStageId;

			// Assert
			Assert.Equal(selectedStageId, viewModel.SelectedStageId);
		}

		[Fact]
		public void BoardViewModel_SelectedGroupId_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new BoardViewModel();
			var selectedGroupId = "group1";

			// Act
			viewModel.SelectedGroupId = selectedGroupId;

			// Assert
			Assert.Equal(selectedGroupId, viewModel.SelectedGroupId);
		}

		[Fact]
		public void BoardViewModel_SelectedTicketId_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new BoardViewModel();
			var selectedTicketId = "ticket1";

			// Act
			viewModel.SelectedTicketId = selectedTicketId;

			// Assert
			Assert.Equal(selectedTicketId, viewModel.SelectedTicketId);
		}

		[Fact]
		public void BoardViewModel_SelectedDirection_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new BoardViewModel();
			var direction = "right";

			// Act
			viewModel.SelectedDirection = direction;

			// Assert
			Assert.Equal(direction, viewModel.SelectedDirection);
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserAGroupManagerForStage_ShouldReturnTrue_WhenUserIsManagerForStage()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user1" };
			var group1 = new Group { ManagerId = "user1" };
			var group2 = new Group { ManagerId = "user2" };

			var assignedGroups = new Dictionary<string, List<Group>>
		{
			{ "stage1", new List<Group> { group1, group2 } }
		};

			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				AssignedGroups = assignedGroups
			};

			// Act
			var result = viewModel.IsCurrentUserAGroupManagerForStage("stage1");

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void BoardViewModel_IsCurrentUserAGroupManagerForStage_ShouldReturnFalse_WhenUserIsNotManagerForStage()
		{
			// Arrange
			var currentUser = new TicketAppUser { Id = "user3" };
			var group1 = new Group { ManagerId = "user1" };
			var group2 = new Group { ManagerId = "user2" };

			var assignedGroups = new Dictionary<string, List<Group>>
		{
			{ "stage1", new List<Group> { group1, group2 } }
		};

			var viewModel = new BoardViewModel
			{
				CurrentUser = currentUser,
				AssignedGroups = assignedGroups
			};

			// Act
			var result = viewModel.IsCurrentUserAGroupManagerForStage("stage1");

			// Assert
			Assert.False(result);
		}
	}
}
