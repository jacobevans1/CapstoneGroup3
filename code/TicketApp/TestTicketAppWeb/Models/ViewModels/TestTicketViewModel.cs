using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Models.ViewModels
{
	public class TestTicketViewModel
	{
		[Fact]
		public void TicketViewModel_Constructor_ShouldInitializeProperties()
		{
			// Arrange
			var viewModel = new TicketViewModel();

			// Act & Assert
			Assert.NotNull(viewModel.Ticket);
			Assert.NotNull(viewModel.NewTicket);
			Assert.NotNull(viewModel.Project);
			Assert.NotNull(viewModel.Board);
			Assert.NotNull(viewModel.AssignedGroups);
			Assert.NotNull(viewModel.EligibleAssignees);
		}

		[Fact]
		public void TicketViewModel_SelectedUserId_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new TicketViewModel();
			var userId = "user1";

			// Act
			viewModel.SelectedUserId = userId;

			// Assert
			Assert.Equal(userId, viewModel.SelectedUserId);
		}

		[Fact]
		public void TicketViewModel_SelectedStageId_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new TicketViewModel();
			var stageId = "stage1";

			// Act
			viewModel.SelectedStageId = stageId;

			// Assert
			Assert.Equal(stageId, viewModel.SelectedStageId);
		}

		[Fact]
		public void TicketViewModel_AssignedGroups_ShouldAddAndRemoveGroups()
		{
			// Arrange
			var viewModel = new TicketViewModel();
			var group1 = new Group { Id = "group1" };
			var group2 = new Group { Id = "group2" };

			// Act
			viewModel.AssignedGroups["stage1"] = new List<Group> { group1, group2 };

			// Assert
			Assert.Contains(group1, viewModel.AssignedGroups["stage1"]);
			Assert.Contains(group2, viewModel.AssignedGroups["stage1"]);
		}

		[Fact]
		public void TicketViewModel_EligibleAssignees_ShouldAddUsersCorrectly()
		{
			// Arrange
			var viewModel = new TicketViewModel();
			var user1 = new TicketAppUser { Id = "user1" };
			var user2 = new TicketAppUser { Id = "user2" };

			// Act
			viewModel.EligibleAssignees.Add(user1);
			viewModel.EligibleAssignees.Add(user2);

			// Assert
			Assert.Contains(user1, viewModel.EligibleAssignees);
			Assert.Contains(user2, viewModel.EligibleAssignees);
		}

		[Fact]
		public void TicketViewModel_CurrentUserRole_ShouldSetAndGetCorrectly()
		{
			// Arrange
			var viewModel = new TicketViewModel();
			var role = "Admin";

			// Act
			viewModel.CurrentUserRole = role;

			// Assert
			Assert.Equal(role, viewModel.CurrentUserRole);
		}
	}
}
