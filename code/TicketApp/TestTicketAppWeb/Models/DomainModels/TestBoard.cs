using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestBoard
	{
		[Fact]
		public void Board_Constructor_ShouldInitializeCollections()
		{
			// Arrange & Act
			var board = new Board();

			// Assert
			Assert.NotNull(board.BoardStages);
			Assert.Empty(board.BoardStages);

			Assert.NotNull(board.Tickets);
			Assert.Empty(board.Tickets);
		}

		[Fact]
		public void Board_Property_ShouldSetAndGetValues()
		{
			// Arrange
			var board = new Board
			{
				Id = "1",
				BoardName = "Test Board",
				Description = "Test Description",
				ProjectId = "P1",
				Project = new Project { Id = "P1", Description = "Test Project" }
			};

			// Act & Assert
			Assert.Equal("1", board.Id);
			Assert.Equal("Test Board", board.BoardName);
			Assert.Equal("Test Description", board.Description);
			Assert.Equal("P1", board.ProjectId);
			Assert.NotNull(board.Project);
			Assert.Equal("P1", board.Project.Id);
		}

		[Fact]
		public void Board_Collection_AddingItems_ShouldWork()
		{
			// Arrange
			var board = new Board();
			var boardStage = new BoardStage { BoardId = "1", StageId = "2" };
			var ticket = new Ticket { Id = "T1", Title = "Test Ticket" };

			// Act
			board.BoardStages.Add(boardStage);
			board.Tickets.Add(ticket);

			// Assert
			Assert.Contains(boardStage, board.BoardStages);
			Assert.Contains(ticket, board.Tickets);
		}
	}
}
