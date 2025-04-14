using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestTicket
	{
		[Fact]
		public void Ticket_Properties_SetAndGet_Correctly()
		{
			// Arrange
			var createdDate = DateTime.Now;
			var user = new TicketAppUser { Id = "user1", UserName = "User One" };
			var historyItem = new TicketHistory
			{
				TicketId = "T1",
				PropertyChanged = "Title",
				OldValue = "Old Title",
				NewValue = "New Title",
				ChangedByUserId = "user1",
				ChangedByUser = user,
				ChangeDate = createdDate,
				ChangeDescription = "Updated title"
			};

			var history = new List<TicketHistory> { historyItem };

			var ticket = new Ticket
			{
				Id = "T1",
				Title = "Fix Bug",
				Description = "Fix the login bug.",
				CreatedDate = createdDate,
				CreatedBy = "admin",
				AssignedTo = "dev1",
				AssignedToUser = user,
				Stage = "In Progress",
				IsComplete = true,
				BoardId = "B1",
				History = history
			};

			// Assert
			Assert.Equal("T1", ticket.Id);
			Assert.Equal("Fix Bug", ticket.Title);
			Assert.Equal("Fix the login bug.", ticket.Description);
			Assert.Equal(createdDate, ticket.CreatedDate);
			Assert.Equal("admin", ticket.CreatedBy);
			Assert.Equal("dev1", ticket.AssignedTo);
			Assert.Equal(user, ticket.AssignedToUser);
			Assert.Equal("In Progress", ticket.Stage);
			Assert.True(ticket.IsComplete);
			Assert.Equal("B1", ticket.BoardId);
			Assert.Single(ticket.History);
			Assert.Equal(historyItem, ticket.History.First());
		}

		[Fact]
		public void Ticket_Default_History_IsInitialized()
		{
			var ticket = new Ticket();

			Assert.NotNull(ticket.History);
			Assert.Empty(ticket.History);
		}
	}
}
