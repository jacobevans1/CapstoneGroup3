using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestTicketAssignee
	{
		[Fact]
		public void TicketAssignee_CanSetAndGetProperties()
		{
			// Arrange
			var ticketAssignee = new TicketAssignee();
			var ticket = new Ticket { Id = "T123" };
			var user = new TicketAppUser { Id = "U456" };

			// Act
			ticketAssignee.TicketId = "T123";
			ticketAssignee.UserId = "U456";
			ticketAssignee.Ticket = ticket;
			ticketAssignee.User = user;

			// Assert
			Assert.Equal("T123", ticketAssignee.TicketId);
			Assert.Equal("U456", ticketAssignee.UserId);
			Assert.Equal(ticket, ticketAssignee.Ticket);
			Assert.Equal(user, ticketAssignee.User);
		}

		[Fact]
		public void TicketAssignee_Constructor_InitializesProperties()
		{
			// Arrange
			var ticket = new Ticket { Id = "T789" };
			var user = new TicketAppUser { Id = "U987" };

			// Act
			var ticketAssignee = new TicketAssignee
			{
				TicketId = "T789",
				UserId = "U987",
				Ticket = ticket,
				User = user
			};

			// Assert
			Assert.Equal("T789", ticketAssignee.TicketId);
			Assert.Equal("U987", ticketAssignee.UserId);
			Assert.Equal(ticket, ticketAssignee.Ticket);
			Assert.Equal(user, ticketAssignee.User);
		}
	}
}
