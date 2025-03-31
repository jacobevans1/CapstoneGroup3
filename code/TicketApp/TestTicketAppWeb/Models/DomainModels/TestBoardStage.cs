using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestBoardStage
	{
		[Fact]
		public void BoardStage_CanSetAndGetProperties()
		{
			// Arrange
			var board = new Board { Id = "B1" };
			var stage = new Stage { Id = "S1" };
			var group = new Group { Id = "G1" };

			var boardStage = new BoardStage
			{
				BoardId = "B1",
				StageId = "S1",
				GroupId = "G1",
				StageOrder = 2,
				Board = board,
				Stage = stage,
				Group = group
			};

			// Assert
			Assert.Equal("B1", boardStage.BoardId);
			Assert.Equal("S1", boardStage.StageId);
			Assert.Equal("G1", boardStage.GroupId);
			Assert.Equal(2, boardStage.StageOrder);
			Assert.Equal(board, boardStage.Board);
			Assert.Equal(stage, boardStage.Stage);
			Assert.Equal(group, boardStage.Group);
		}

		[Fact]
		public void BoardStage_DefaultConstructor_CreatesInstance()
		{
			// Act
			var boardStage = new BoardStage();

			// Assert
			Assert.NotNull(boardStage);
		}
	}
}
