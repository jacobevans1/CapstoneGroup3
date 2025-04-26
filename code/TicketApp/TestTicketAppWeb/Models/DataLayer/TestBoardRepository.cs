using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DataLayer;

public class TestBoardRepository : IDisposable
{
	private TicketAppContext CreateMockContext<T>(List<T> seedData, string dbName) where T : class
	{
		var options = new DbContextOptionsBuilder<TicketAppContext>()
			.UseInMemoryDatabase(databaseName: dbName)
			.Options;
		var ctx = new TicketAppContext(options);
		ctx.Set<T>().AddRange(seedData);
		ctx.SaveChanges();
		return ctx;
	}

	private readonly TicketAppContext _context;
	private readonly BoardRepository _repo;

	public TestBoardRepository()
	{
		_context = CreateMockContext(new List<Group>(), "TestBoardRepo");
		_repo = new BoardRepository(_context);
	}

	public void Dispose() => _context.Dispose();

	[Fact]
	public void AddBoard_CreatesBoardDefaultStagesAndGroups()
	{
		var mgr = new Group { Id = "mgr", ManagerId = "mgr" };
		var project = new Project
		{
			Id = "p2",
			ProjectName = "P2",
			LeadId = "mgr",
			Groups = new List<Group> { mgr }
		};

		var ctx = CreateMockContext(new List<Group> { mgr }, "TBR_AddBoard");
		var repo = new BoardRepository(ctx);

		// Act
		repo.AddBoard(project);

		// Assert board exists
		var board = ctx.Boards.Single(b => b.ProjectId == "p2");
		Assert.Equal("P2 Board", board.BoardName);
		Assert.Equal("Default Description", board.Description);

		// Assert exactly 3 default stages created
		var stages = ctx.BoardStages.Where(bs => bs.BoardId == board.Id).ToList();
		Assert.Equal(3, stages.Count);

		// Assert each default stage has one stage-group for the manager
		var bsg = ctx.BoardStageGroups.Where(x => x.GroupId == "mgr").ToList();
		Assert.Equal(3, bsg.Count);
	}

	[Fact]
	public void AddStage_NoGroups_AddsOnlyStageAndBoardStage()
	{
		// Arrange seed a Board so we can add a stage on it
		var board = new Board
		{
			Id = "b1",
			ProjectId = "p1",
			BoardName = "B1",
			Description = "desc"
		};
		_context.Boards.Add(board);
		_context.SaveChanges();

		// Act
		_repo.AddStage("b1", "NewStage", new List<string>());

		// Assert
		var stage = _context.Stages.Single(s => s.Name == "NewStage");
		var boardStage = _context.BoardStages.Single(bs => bs.StageId == stage.Id && bs.BoardId == "b1");
		var groupsInBs = _context.BoardStageGroups.Where(bsg => bsg.StageId == stage.Id);
		Assert.NotNull(stage);
		Assert.NotNull(boardStage);
		Assert.Empty(groupsInBs);
	}

	[Fact]
	public void AddStage_WithGroups_AddsStageAndGroups()
	{
		// Arrange
		var board = new Board { Id = "b2", ProjectId = "p2", BoardName = "B2", Description = "d" };
		var grp = new Group { Id = "g2", ManagerId = "m2" };
		_context.Boards.Add(board);
		_context.Groups.Add(grp);
		_context.SaveChanges();

		// Act
		_repo.AddStage("b2", "StageX", new List<string> { "g2" });

		// Assert
		var stage = _context.Stages.Single(s => s.Name == "StageX");
		var assignment = _context.BoardStageGroups
								 .Single(bsg => bsg.StageId == stage.Id && bsg.GroupId == "g2");
		Assert.NotNull(stage);
		Assert.NotNull(assignment);
	}

	[Fact]
	public void DeleteStage_Existing_RemovesStageAndBoardStage()
	{
		// Arrange
		var board = new Board { Id = "b3", ProjectId = "p3", BoardName = "B3", Description = "d" };
		var stage = new Stage { Id = "s3", Name = "toDelete" };
		_context.Boards.Add(board);
		_context.Stages.Add(stage);
		_context.BoardStages.Add(new BoardStage { BoardId = "b3", StageId = "s3", StageOrder = 0 });
		_context.SaveChanges();

		// Act
		_repo.DeleteStage("b3", "s3");

		// Assert
		Assert.Null(_context.Stages.Find("s3"));
		Assert.Empty(_context.BoardStages.Where(bs => bs.StageId == "s3"));
	}

	[Fact]
	public void DeleteBoardStageGroups_RemovesAllMatching()
	{
		// Arrange
		_context.BoardStageGroups.AddRange(
			new BoardStageGroup { Id = Guid.NewGuid().ToString(), BoardId = "b7", StageId = "x", GroupId = "g1" },
			new BoardStageGroup { Id = Guid.NewGuid().ToString(), BoardId = "b7", StageId = "x", GroupId = "g2" }
		);
		_context.SaveChanges();

		// Act
		_repo.DeleteBoardStageGroups("b7", "x");

		// Assert
		Assert.Empty(_context.BoardStageGroups.Where(bsg => bsg.BoardId == "b7" && bsg.StageId == "x"));
	}

	[Fact]
	public void DeleteStage_NotExist_DoesNothing()
	{
		// should not throw
		_repo.DeleteStage("noBoard", "noStage");
	}

	[Fact]
	public void RenameStage_Existing_UpdatesName()
	{
		// Arrange
		var stage = new Stage { Id = "s4", Name = "old" };
		_context.Stages.Add(stage);
		_context.SaveChanges();

		// Act
		_repo.RenameStage("s4", "new");

		// Assert
		Assert.Equal("new", _context.Stages.Find("s4")!.Name);
	}

	[Fact]
	public void RenameStage_NotExist_DoesNothing()
	{
		// should not throw
		_repo.RenameStage("noStage", "whatever");
	}

	

	[Fact]
	public void SaveBoardStages_UpdatesOrder()
	{
		// Arrange
		_context.BoardStages.Add(new BoardStage { BoardId = "b6", StageId = "s6", StageOrder = 1 });
		_context.SaveChanges();

		// Act
		_repo.SaveBoardStages(new List<BoardStage> { new BoardStage { StageId = "s6", StageOrder = 42 } });

		// Assert
		Assert.Equal(42, _context.BoardStages.Single(bs => bs.StageId == "s6").StageOrder);
	}

	[Fact]
	public void AssignGroupToStage_RemovesOldAddsNew()
	{
		// Arrange
		var board = new Board
		{
			Id = "b5",
			ProjectId = "p5",
			BoardName = "B5",
			Description = "desc"
		};
		var stage = new Stage { Id = "s5", Name = "st5" };

		_context.Boards.Add(board);
		_context.Stages.Add(stage);
		_context.BoardStages.Add(new BoardStage
		{
			BoardId = "b5",
			StageId = "s5",
			StageOrder = 0
		});

		// Two groups exist, but only gA is initially assigned:
		_context.Groups.AddRange(
			new Group { Id = "gA", ManagerId = "m" },
			new Group { Id = "gB", ManagerId = "m" }
		);
		_context.BoardStageGroups.Add(new BoardStageGroup
		{
			Id = Guid.NewGuid().ToString(),
			BoardId = "b5",
			StageId = "s5",
			GroupId = "gA"
		});

		_context.SaveChanges();

		// Act switch assignment to only gB
		_repo.AssignGroupToStage("b5", "s5", new List<string> { "gB" });

		// Assert direct in-memory inspection
		var assigned = _context.BoardStageGroups
			.Where(bsg => bsg.BoardId == "b5" && bsg.StageId == "s5")
			.Select(bsg => bsg.GroupId)
			.ToList();

		Assert.Single(assigned);
		Assert.Contains("gB", assigned);
	}

	[Fact]
	public async Task GetBoardByProjectIdAsync_NoBoard_ReturnsNull()
	{
		var ctx = CreateMockContext(new List<Group>(), "TBR_NoBoard");
		var repo = new BoardRepository(ctx);

		var result = await repo.GetBoardByProjectIdAsync("doesNotExist");
		Assert.Null(result);
	}

	[Fact]
	public async Task GetBoardByProjectIdAsync_WithBoard_ReturnsBoardAndGroups()
	{
		var group = new Group { Id = "g1", GroupName = "G1", ManagerId = "lead1" };
		var project = new Project
		{
			Id = "p1",
			ProjectName = "Proj1",
			LeadId = "lead1",
			Groups = new List<Group> { group }
		};
		var board = new Board
		{
			Id = "b1",
			ProjectId = "p1",
			BoardName = "Proj1 Board",
			Description = "desc"
		};

		var ctx = CreateMockContext(new List<Group> { group }, "TBR_WithBoard");
		ctx.Projects.Add(project);
		ctx.Boards.Add(board);
		ctx.SaveChanges();

		var repo = new BoardRepository(ctx);
		var result = await repo.GetBoardByProjectIdAsync("p1");

		Assert.NotNull(result);
		Assert.Equal("b1", result!.Id);
		Assert.Equal("Proj1", result.Project.ProjectName);
		Assert.Single(result.Project.Groups);
		Assert.Equal("g1", result.Project.Groups.First().Id);
	}

	[Fact]
	public void GetBoardStages_InMemory_ReturnsOrderedBoardStages()
	{
		var seed = new List<BoardStage>
	{
		new BoardStage { BoardId = "B", StageId = "s2", StageOrder = 1 },
		new BoardStage { BoardId = "B", StageId = "s1", StageOrder = 0 }
	};
		var ctx = CreateMockContext(seed, "TBR_GetBoardStages");
		var repo = new BoardRepository(ctx);

		var list = repo.GetBoardStages("B").ToList();
		Assert.Equal(2, list.Count);
		Assert.Equal("s1", list[0].StageId);
		Assert.Equal("s2", list[1].StageId);
	}

	[Fact]
	public void GetStages_InMemory_ReturnsOrderedStages()
	{
		var stages = new List<Stage>
		{
			new Stage { Id = "s1", Name = "First" },
			new Stage { Id = "s2", Name = "Second" }
		};

		var boardStages = new List<BoardStage>
		{
			new BoardStage { BoardId = "B", StageId = "s2", StageOrder = 1 },
			new BoardStage { BoardId = "B", StageId = "s1", StageOrder = 0 }
		};

		var ctx = CreateMockContext(stages, "TBR_Stages");
		ctx.Set<BoardStage>().AddRange(boardStages);
		ctx.SaveChanges();

		var repo = new BoardRepository(ctx);
		var result = repo.GetStages("B").ToList();

		Assert.Equal(2, result.Count);
		Assert.Equal("First", result[0].Name);
		Assert.Equal("Second", result[1].Name);
	}

	[Fact]
	public void GetBoardStageTickets_InMemory_ReturnsTicketsGroupedByStage()
	{
		var boardStages = new List<BoardStage>
		{
			new BoardStage { BoardId = "b1", StageId = "s1", StageOrder = 0 },
			new BoardStage { BoardId = "b1", StageId = "s2", StageOrder = 1 }
		};

		var tickets = new List<Ticket>
		{
			new Ticket { Id = "t1", BoardId = "b1", Stage = "s1", Title = "T1" },
			new Ticket { Id = "t2", BoardId = "b1", Stage = null!, Title = "T2" },
			new Ticket { Id = "t3", BoardId = "b1", Stage = "s1", Title = "T3" },
			new Ticket { Id = "t4", BoardId = "b1", Stage = "s2", Title = "T4" }
		};

		var ctx = CreateMockContext(boardStages, "TBR_StageTickets_Stages");
		ctx.Set<Ticket>().AddRange(tickets);
		ctx.SaveChanges();

		var repo = new BoardRepository(ctx);

		// Act
		var dict = repo.GetBoardStageTickets("b1");

		// Assert should have exactly the two stage keys
		Assert.Equal(2, dict.Keys.Count);
		Assert.Contains("s1", dict.Keys);
		Assert.Contains("s2", dict.Keys);

		// s1 got t1 and t3
		var s1List = dict["s1"];
		Assert.Equal(2, s1List.Count);
		Assert.Contains(s1List, t => t.Id == "t1");
		Assert.Contains(s1List, t => t.Id == "t3");

		// s2 got only t4
		var s2List = dict["s2"];
		Assert.Single(s2List);
		Assert.Equal("t4", s2List[0].Id);
	}
}
