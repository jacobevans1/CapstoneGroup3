using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestBoardStage
	{
		[Fact]
		public void CanInitializeBoardStage_WithProperties()
		{
			// Arrange
			var boardId = Guid.NewGuid().ToString();
			var stageId = Guid.NewGuid().ToString();
			var stageOrder = 1;

			var board = new Board
			{
				Id = boardId,
				BoardName = "Dev Board",
				Description = "Development Tasks"
			};

			var stage = new Stage
			{
				Id = stageId,
				Name = "To Do"
			};

			// Act
			var boardStage = new BoardStage
			{
				BoardId = boardId,
				StageId = stageId,
				StageOrder = stageOrder,
				Board = board,
				Stage = stage
			};

			// Assert
			Assert.Equal(boardId, boardStage.BoardId);
			Assert.Equal(stageId, boardStage.StageId);
			Assert.Equal(stageOrder, boardStage.StageOrder);
			Assert.Equal(board, boardStage.Board);
			Assert.Equal(stage, boardStage.Stage);
		}

		[Fact]
		public void CanSetBoardStage_NullableNavigationProperties()
		{
			// Arrange & Act
			var boardStage = new BoardStage
			{
				BoardId = "board1",
				StageId = "stage1",
				StageOrder = 1,
				Board = null,
				Stage = null
			};

			// Assert
			Assert.Null(boardStage.Board);
			Assert.Null(boardStage.Stage);
		}

		[Fact]
		public void CanChangeStageOrder()
		{
			// Arrange
			var boardId = Guid.NewGuid().ToString();
			var stageId = Guid.NewGuid().ToString();
			var board = new Board
			{
				Id = boardId,
				BoardName = "Dev Board",
				Description = "Development Tasks"
			};

			var stage = new Stage
			{
				Id = stageId,
				Name = "In Progress"
			};

			var boardStage = new BoardStage
			{
				BoardId = boardId,
				StageId = stageId,
				StageOrder = 1,
				Board = board,
				Stage = stage
			};

			// Act
			boardStage.StageOrder = 2;

			// Assert
			Assert.Equal(2, boardStage.StageOrder);
		}
	}
}
