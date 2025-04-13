using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestBoardStageGroup
	{
		[Fact]
		public void CanInitializeBoardStageGroup_WithProperties()
		{
			// Arrange
			var boardId = Guid.NewGuid().ToString();
			var stageId = Guid.NewGuid().ToString();
			var groupId = Guid.NewGuid().ToString();
			var id = Guid.NewGuid().ToString();

			var board = new Board { Id = boardId, BoardName = "Dev Board", Description = "Development Tasks" };
			var stage = new Stage { Id = stageId, Name = "To Do" };

			var boardStage = new BoardStage
			{
				BoardId = boardId,
				StageId = stageId,
				StageOrder = 1,
				Board = board,
				Stage = stage
			};

			var manager = new TicketAppUser
			{
				Id = "manager123",
				FirstName = "John",
				LastName = "Doe"
			};

			var group = new Group
			{
				Id = groupId,
				GroupName = "Backend Team",
				Description = "Works on backend features",
				ManagerId = manager.Id,
				Manager = manager
			};

			// Act
			var boardStageGroup = new BoardStageGroup
			{
				Id = id,
				BoardId = boardId,
				StageId = stageId,
				GroupId = groupId,
				BoardStage = boardStage,
				Group = group
			};

			// Assert
			Assert.Equal(id, boardStageGroup.Id);
			Assert.Equal(boardId, boardStageGroup.BoardId);
			Assert.Equal(stageId, boardStageGroup.StageId);
			Assert.Equal(groupId, boardStageGroup.GroupId);
			Assert.Equal(boardStage, boardStageGroup.BoardStage);
			Assert.Equal(group, boardStageGroup.Group);
			Assert.Equal("Dev Board", boardStageGroup.BoardStage.Board.BoardName);
			Assert.Equal("Backend Team", boardStageGroup.Group.GroupName);
			Assert.Equal("John Doe", boardStageGroup.Group.Manager.FullName);
		}

		[Fact]
		public void CanSetBoardStageGroup_NullableNavigationProperties()
		{
			// Arrange & Act
			var boardStageGroup = new BoardStageGroup
			{
				Id = "test123",
				BoardId = "board1",
				StageId = "stage1",
				GroupId = "group1",
				BoardStage = null,
				Group = null
			};

			// Assert
			Assert.Null(boardStageGroup.BoardStage);
			Assert.Null(boardStageGroup.Group);
		}
	}
}
