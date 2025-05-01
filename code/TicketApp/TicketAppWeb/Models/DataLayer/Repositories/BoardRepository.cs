using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories;

/// <summary>
/// The BoardRepository class implements IBoardRepository
/// </summary>
public class BoardRepository : Repository<Board>, IBoardRepository
{
	private readonly List<Stage> _defaultStages;

	/// <summary>
	/// Initializes a new instance of the <see cref="BoardRepository"/> class.
	/// </summary>
	/// <param name="ctx"></param>
	public BoardRepository(TicketAppContext ctx) : base(ctx)
	{
		_defaultStages = new List<Stage>
		{
			new Stage { Id = "5de8ea48-a734-4b97-bc39-18070e4e25a9", Name = "Todo" },
			new Stage { Id = "8409c38a-5fab-4283-8957-1c5888d7716d", Name = "In Progress" },
			new Stage { Id = "bd0ccc84-e3bd-4417-85ae-15ac2457e390", Name = "Done" }
		};
	}

	/// <summary>
	/// Gets the board for the specified project.
	/// </summary>
	/// <param name="projectId"></param>
	public async Task<Board?> GetBoardByProjectIdAsync(string projectId)
	{
		var result = await (from p in context.Projects
							join b in context.Boards on p.Id equals b.ProjectId into boardGroup
							from board in boardGroup.DefaultIfEmpty()
							where p.Id == projectId
							select new { Project = p, Board = board })
					  .FirstOrDefaultAsync();

		if (result == null) return null;

		var newBoard = result.Board!;
		var project = result.Project!;

		project.Groups = context.Projects
								 .Include(p => p.Groups)
								 .First(p => p.Id == projectId)
								 .Groups;
		newBoard.Project = project;
		return newBoard;
	}

	/// <summary>
	/// Adds a new board for the specified project.
	/// </summary>
	/// <param name="project"></param>
	/// <param name="isAdmin"></param>
	public void AddBoard(Project project, bool isAdmin)
	{
		var board = CreateBoard(project);
		var managerGroup = new Group();

		managerGroup = isAdmin ? project.Groups.FirstOrDefault() : project.Groups.FirstOrDefault(g => g.ManagerId == project.CreatedById);

		Insert(board);
		var boardStages = AddDefaultBoardStages(board);
		AddDefaultBoardStageGroups(boardStages, managerGroup!.Id);
		Save();
	}

	/// <summary>
	/// Adds a new status to the board.
	/// </summary>
	/// <param name="boardId"></param>
	/// <param name="stageName"></param>
	/// <param name="groupIds"></param>
	public void AddStage(string boardId, string stageName, List<string> groupIds)
	{
		var stage = CreateStage(stageName);
		var boardStages = context.BoardStages.Where(bs => bs.BoardId == boardId).ToList();
		var boardStage = CreateBoardStage(boardId, stage.Id, boardStages.Count);

		context.Stages.Add(stage);
		Save();

		context.BoardStages.Add(boardStage);
		Save();

		foreach (var groupId in groupIds)
		{
			var boardStageGroup = CreateBoardStageGroup(boardId, stage.Id, groupId);
			context.BoardStageGroups.Add(boardStageGroup);
		}
		Save();
	}

	/// <summary>
	/// Deletes a status for the specified board.
	/// </summary>
	/// <param name="boardId"></param>
	/// <param name="stageId"></param>
	public void DeleteStage(string boardId, string stageId)
	{
		var boardStage = context.BoardStages.FirstOrDefault(bs => bs.BoardId == boardId && bs.StageId == stageId);
		if (boardStage != null)
		{
			context.BoardStages.Remove(boardStage);
			Save();
		}

		var stage = context.Stages.FirstOrDefault(s => s.Id == stageId);
		if (stage != null)
		{
			context.Stages.Remove(stage);
			Save();
		}
	}


	/// <summary>
	/// Renames a status on the board.
	/// </summary>
	/// <param name="stageId"></param>
	/// <param name="newStageName"></param>
	public void RenameStage(string stageId, string newStageName)
	{
		var stage = context.Stages.FirstOrDefault(s => s.Id == stageId);
		if (stage != null)
		{
			stage.Name = newStageName;
			context.Stages.Update(stage);
			Save();
		}
	}

	/// <summary>
	/// Assigns a group to a status.
	/// </summary>
	/// <param name="boardId"></param>
	/// <param name="stageId"></param>
	/// <param name="groupIds"></param>
	public void AssignGroupToStage(string boardId, string stageId, List<string> newGroupIds)
	{
		var boardStage = context.BoardStages.FirstOrDefault(bs => bs.BoardId == boardId && bs.StageId == stageId);
		if (boardStage == null)
		{
			throw new Exception("Board stage not found.");
		}

		var existingBoardStageGroups = GetBoardStageGroups(boardId)[stageId];

		foreach (var group in existingBoardStageGroups)
		{
			if (!newGroupIds.Contains(group.Id))
			{
				var boardStageGroup = context.BoardStageGroups
					.FirstOrDefault(bsg => bsg.BoardId == boardId && bsg.StageId == stageId && bsg.GroupId == group.Id);
				context.BoardStageGroups.Remove(boardStageGroup!);
			}
		}
		Save();

		foreach (var newGroupId in newGroupIds)
		{
			if (!IsGroupAssignedToStage(boardId, stageId, newGroupId))
			{
				var boardStageGroup = CreateBoardStageGroup(boardId, stageId, newGroupId);
				context.BoardStageGroups.Add(boardStageGroup);
			}
		}
		Save();
	}

	/// <summary>
	/// Gets the stages for the specified board.
	/// </summary>
	/// <param name="boardId"></param>
	public ICollection<Stage> GetStages(string boardId)
	{
		return (from bs in context.BoardStages
				join s in context.Stages on bs.StageId equals s.Id
				where bs.BoardId == boardId
				orderby bs.StageOrder
				select s)
			   .ToList();
	}

	/// <summary>
	/// Gets the board stages for the specified board.
	/// </summary>
	/// <param name="boardId"></param>
	public ICollection<BoardStage> GetBoardStages(string boardId)
	{

		return context.BoardStages
			  .Where(bs => bs.BoardId == boardId)
			  .OrderBy(bs => bs.StageOrder)
			  .ToList();
	}

	/// <summary>
	/// Gets the groups assigned to each stage for the specified board.
	/// </summary>
	/// <param name="boardId"></param>
	public Dictionary<string, List<Group>> GetBoardStageGroups(string boardId)
	{
		return context.BoardStageGroups
		   .Include(bsg => bsg.Group)
		   .Where(bsg => bsg.BoardId == boardId)
		   .AsEnumerable()
		   .GroupBy(bsg => bsg.StageId)
		   .ToDictionary(g => g.Key, g => g.Select(x => x.Group).ToList());
	}



	/// <summary>
	/// Gets the tickets assigned to each stage for the specified board.
	/// </summary>
	/// <param name="boardId"></param>
	public Dictionary<string, List<Ticket>> GetBoardStageTickets(string boardId)
	{

		var allStages = context.BoardStages
			.Where(bs => bs.BoardId == boardId)
			.Select(bs => bs.StageId)
			.ToList();

		var tickets = context.Tickets
			.Where(t => t.BoardId == boardId)
			.ToList();

		var result = allStages.ToDictionary(id => id, id => new List<Ticket>());

		foreach (var ticket in tickets)
			if (ticket.Stage != null && result.ContainsKey(ticket.Stage))
				result[ticket.Stage]!.Add(ticket);

		return result;
	}


	/// <summary>
	/// Saves the board stages to the database.
	/// </summary>
	/// <param name="boardStages"></param>
	public void SaveBoardStages(List<BoardStage> boardStages)
	{
		foreach (var boardStage in boardStages)
		{
			var existingStage = context.BoardStages.FirstOrDefault(bs => bs.StageId == boardStage.StageId);
			if (existingStage != null)
			{
				existingStage.StageOrder = boardStage.StageOrder;
				context.BoardStages.Update(existingStage);
				Save();
			}
		}
	}

	/// <summary>
	/// Deletes the board stage groups for the specified board and stage.
	/// </summary>
	/// <param name="boardId"></param>
	/// <param name="stageId"></param>
	public void DeleteBoardStageGroups(string boardId, string stageId)
	{
		foreach (var boardStageGroup in context.BoardStageGroups
			.Where(bsg => bsg.BoardId == boardId && bsg.StageId == stageId))
		{
			context.BoardStageGroups.Remove(boardStageGroup);
			Save();
		}
	}


	private Board CreateBoard(Project project)
	{
		var board = new Board();

		board.Id = Guid.NewGuid().ToString();
		board.BoardName = $"{project.ProjectName} Board";
		board.Description = "Default Description";
		board.Project = project;

		return board;
	}

	private Stage CreateStage(string stageName)
	{
		var stage = new Stage
		{
			Id = Guid.NewGuid().ToString(),
			Name = stageName
		};
		return stage;
	}

	private BoardStage CreateBoardStage(string boardId, string stageId, int stageOrder)
	{
		var boardStage = new BoardStage
		{
			BoardId = boardId,
			StageId = stageId,
			StageOrder = stageOrder + 1
		};
		return boardStage;
	}

	private BoardStageGroup CreateBoardStageGroup(string boardId, string stageId, string groupId)
	{
		var boardStageGroup = new BoardStageGroup
		{
			Id = Guid.NewGuid().ToString(),
			BoardId = boardId,
			StageId = stageId,
			GroupId = groupId
		};
		return boardStageGroup;
	}

	private List<BoardStage> AddDefaultBoardStages(Board board)
	{
		foreach (var stage in _defaultStages)
		{
			var boardStage = new BoardStage
			{
				BoardId = board.Id,
				StageId = stage.Id,
				StageOrder = _defaultStages.IndexOf(stage)
			};
			context.BoardStages.Add(boardStage);
		}
		Save();
		return context.BoardStages.ToList();
	}

	private void AddDefaultBoardStageGroups(List<BoardStage> boardStages, string groupId)
	{
		foreach (var boardStage in boardStages)
		{
			var boardStageGroup = new BoardStageGroup
			{
				Id = Guid.NewGuid().ToString(),
				BoardId = boardStage.BoardId,
				StageId = boardStage.StageId,
				GroupId = groupId
			};
			context.BoardStageGroups.Add(boardStageGroup);
		}
		Save();
	}

	private bool IsGroupAssignedToStage(string boardId, string stageId, string groupId)
	{
		var boardStageGroup = context.BoardStageGroups
			.FirstOrDefault(bsg => bsg.BoardId == boardId && bsg.StageId == stageId && bsg.GroupId == groupId);

		return boardStageGroup != null;
	}
}