using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestTicket
	{
		[Fact]
		public void Ticket_Constructor_ShouldSetPropertiesCorrectly()
		{
			// Arrange
			var ticket = new Ticket
			{
				Id = "T1",
				Title = "Bug Fix",
				Description = "Fixing a bug in the system",
				Stage = "Open",
				BoardId = "B1"
			};

			// Act & Assert
			Assert.Equal("T1", ticket.Id);
			Assert.Equal("Bug Fix", ticket.Title);
			Assert.Equal("Fixing a bug in the system", ticket.Description);
			Assert.Equal("Open", ticket.Stage);
			Assert.Equal("B1", ticket.BoardId);
		}

		[Fact]
		public void Ticket_Id_ShouldBeSetAndGetCorrectly()
		{
			// Arrange
			var ticket = new Ticket();
			ticket.Id = "T1";

			// Act & Assert
			Assert.Equal("T1", ticket.Id);
		}

		[Fact]
		public void Ticket_Title_ShouldBeSetAndGetCorrectly()
		{
			// Arrange
			var ticket = new Ticket();
			ticket.Title = "Feature Request";

			// Act & Assert
			Assert.Equal("Feature Request", ticket.Title);
		}

		[Fact]
		public void Ticket_Description_ShouldBeSetAndGetCorrectly()
		{
			// Arrange
			var ticket = new Ticket();
			ticket.Description = "Request to add a new feature";

			// Act & Assert
			Assert.Equal("Request to add a new feature", ticket.Description);
		}

		[Fact]
		public void Ticket_Status_ShouldBeSetAndGetCorrectly()
		{
			// Arrange
			var ticket = new Ticket();
			ticket.Stage = "Closed";

			// Act & Assert
			Assert.Equal("Closed", ticket.Stage);
		}

		[Fact]
		public void Ticket_BoardId_ShouldBeSetAndGetCorrectly()
		{
			// Arrange
			var ticket = new Ticket();
			ticket.BoardId = "B2";

			// Act & Assert
			Assert.Equal("B2", ticket.BoardId);
		}
	}
}
