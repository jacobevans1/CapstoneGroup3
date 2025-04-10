//using Microsoft.EntityFrameworkCore;
//using Moq;
//using TicketAppWeb.Models.DataLayer;
//using TicketAppWeb.Models.DataLayer.Repositories;
//using TicketAppWeb.Models.DomainModels;
//using TicketAppWeb.Models.DomainModels.MiddleTableModels;

//namespace TestTicketAppWeb.Models.DataLayer
//{
//	public class TestBoardRepository
//	{
//		private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> elements) where T : class
//		{
//			var queryable = elements.AsQueryable();
//			var mockSet = new Mock<DbSet<T>>();
//			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
//			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
//			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
//			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
//			return mockSet;
//		}

//		private TicketAppContext CreateMockContext<T>(List<T> data, string dbName) where T : class
//		{
//			var options = new DbContextOptionsBuilder<TicketAppContext>()
//				.UseInMemoryDatabase(databaseName: dbName)
//				.Options;
//			var context = new TicketAppContext(options);
//			context.Set<T>().AddRange(data);
//			context.SaveChanges();
//			return context;
//		}

//		[Fact]
//		public async Task GetBoardByProjectIdAsync_ReturnsBoard()
//		{
//			var project = new Project { Id = "project1", ProjectName = "Test Project", LeadId = "Test" };
//			var board = new Board { Id = "board1", BoardName = "Test Board", ProjectId = "project1", Description = "Test Board Description" };

//			var context = CreateMockContext(new List<Project> { project }, "GetBoardTest");
//			context.Boards.Add(board);
//			context.SaveChanges();

//			var repository = new BoardRepository(context);
//			var result = await repository.GetBoardByProjectIdAsync("project1");

//			Assert.NotNull(result);
//			Assert.Equal("Test Board", result.BoardName);
//		}

//		[Fact]
//		public void AddBoard_CreatesNewBoard()
//		{
//			var project = new Project
//			{
//				Id = "project2",
//				ProjectName = "New Project",
//				LeadId = "lead1",
//				Groups = new List<Group> { new Group { Id = "group1", ManagerId = "lead1" } }
//			};
//			var context = CreateMockContext(new List<Project> { project }, "AddBoardTest");
//			var repository = new BoardRepository(context);

//			repository.AddBoard(project);

//			var addedBoard = context.Boards.FirstOrDefault(b => b.ProjectId == "project2");
//			Assert.NotNull(addedBoard);
//			Assert.Equal("New Project Board", addedBoard.BoardName);
//			Assert.False(string.IsNullOrEmpty(addedBoard.Description));
//		}

//		[Fact]
//		public void AddStage_AddsNewStage()
//		{
//			var board = new Board { Id = "board1", BoardName = "Test Board", Description = "Test", ProjectId = "project1" };
//			var context = CreateMockContext(new List<Board> { board }, "AddStageTest");
//			var repository = new BoardRepository(context);

//			repository.AddStage("board1", "Review", "group1");

//			var addedStage = context.Stages.FirstOrDefault(s => s.Name == "Review");
//			Assert.NotNull(addedStage);
//		}

//		[Fact]
//		public void DeleteStage_RemovesStage()
//		{
//			var stage = new Stage { Id = "stage1", Name = "Review" };
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "group1", StageOrder = 1 };
//			var context = CreateMockContext(new List<Stage> { stage }, "DeleteStageTest");
//			context.BoardStages.Add(boardStage);
//			context.SaveChanges();
//			var repository = new BoardRepository(context);

//			repository.DeleteStage("board1", "stage1");

//			Assert.Null(context.Stages.FirstOrDefault(s => s.Id == "stage1"));
//			Assert.Null(context.BoardStages.FirstOrDefault(bs => bs.StageId == "stage1"));
//		}

//		[Fact]
//		public void RenameStage_ChangesStageName()
//		{
//			var stage = new Stage { Id = "stage1", Name = "Old Name" };
//			var context = CreateMockContext(new List<Stage> { stage }, "RenameStageTest");
//			var repository = new BoardRepository(context);

//			repository.RenameStage("stage1", "New Name");

//			var updatedStage = context.Stages.FirstOrDefault(s => s.Id == "stage1");
//			Assert.NotNull(updatedStage);
//			Assert.Equal("New Name", updatedStage.Name);
//		}

//		[Fact]
//		public void AssignGroupToStage_AssignsGroup()
//		{
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "oldGroup", StageOrder = 1 };
//			var context = CreateMockContext(new List<BoardStage> { boardStage }, "AssignGroupTest");
//			var repository = new BoardRepository(context);

//			repository.AssignGroupToStage("board1", "stage1", "newGroup");

//			var updatedBoardStage = context.BoardStages.FirstOrDefault(bs => bs.StageId == "stage1");
//			Assert.NotNull(updatedBoardStage);
//			Assert.Equal("newGroup", updatedBoardStage.GroupId);
//		}

//		[Fact]
//		public void SaveBoardStages_UpdatesStages()
//		{
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "group1", StageOrder = 1 };
//			var context = CreateMockContext(new List<BoardStage> { boardStage }, "SaveBoardStagesTest");
//			var repository = new BoardRepository(context);

//			boardStage.StageOrder = 2;
//			repository.SaveBoardStages(new List<BoardStage> { boardStage });

//			var updatedStage = context.BoardStages.FirstOrDefault(bs => bs.StageId == "stage1");
//			Assert.NotNull(updatedStage);
//			Assert.Equal(2, updatedStage.StageOrder);
//		}

//		[Fact]
//		public void GetStages_ReturnsStages()
//		{
//			var board = new Board { Id = "board1", BoardName = "Test Board", Description = "Test", ProjectId = "project1" };
//			var stage = new Stage { Id = "stage1", Name = "Development" };
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "oldGroup", StageOrder = 1 };

//			var context = CreateMockContext(new List<Board> { board }, "GetStagesTest");
//			context.Stages.Add(stage);
//			context.BoardStages.Add(boardStage);
//			context.SaveChanges();

//			var repository = new BoardRepository(context);
//			var stages = context.Stages
//				.Where(s => context.BoardStages.Any(bs => bs.BoardId == "board1" && bs.StageId == s.Id))
//				.OrderBy(s => context.BoardStages.First(bs => bs.StageId == s.Id).StageOrder)
//				.ToList();

//			Assert.Single(stages);
//			Assert.Equal("Development", stages.First().Name);
//		}

//		[Fact]
//		public void GetBoardStages_ReturnsBoardStages()
//		{
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "oldGroup", StageOrder = 1 };
//			var context = CreateMockContext(new List<BoardStage> { boardStage }, "GetBoardStagesTest");

//			var repository = new BoardRepository(context);
//			var result = context.BoardStages.Where(bs => bs.BoardId == "board1").OrderBy(bs => bs.StageOrder).ToList();

//			Assert.Single(result);
//			Assert.Equal("stage1", result.First().StageId);
//		}

//		[Fact]
//		public void GetAllAssignedGroupsForStages_ReturnsAssignedGroups()
//		{
//			var group = new Group { Id = "group1", GroupName = "Developers", ManagerId = "manager1" };
//			var boardStage = new BoardStage { BoardId = "board1", StageId = "stage1", GroupId = "group1" };

//			var context = CreateMockContext(new List<Group> { group }, "GetAssignedGroupsTest");
//			context.BoardStages.Add(boardStage);
//			context.SaveChanges();

//			var repository = new BoardRepository(context);
//			var assignedGroups = context.BoardStages.Where(bs => bs.BoardId == "board1")
//				.Join(context.Groups, bs => bs.GroupId, g => g.Id, (bs, g) => new { bs.StageId, g.GroupName })
//				.ToDictionary(bg => bg.StageId, bg => bg.GroupName);

//			Assert.Single(assignedGroups);
//			Assert.Equal("Developers", assignedGroups["stage1"]);
//		}
//	}
//}
