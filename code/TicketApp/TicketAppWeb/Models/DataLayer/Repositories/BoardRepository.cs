using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories
{
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

			var newBoard = result?.Board;
			var project = result?.Project;

			project.Groups = context.Projects.Include(p => p.Groups).FirstOrDefault(p => p.Id == projectId).Groups;
			newBoard.Project = project;

			return newBoard;
		}

		/// <summary>
		/// Adds a new board for the specified project.
		/// </summary>
		/// <param name="project"></param>
		public void AddBoard(Project project)
		{
			var board = CreateBoard(project);
			var managerGroup = project.Groups.FirstOrDefault(g => g.ManagerId == project.LeadId);

			Insert(board);
			AddDefaultBoardStages(board, managerGroup.Id);
			Save();
		}

		/// <summary>
		/// Adds a new status to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageName"></param>
		/// <param name="groupId"></param>
		public void AddStage(string boardId, string stageName, string groupId)
		{
			var stage = CreateStage(stageName);
			var boardStages = context.BoardStages.Where(bs => bs.BoardId == boardId).ToList();
			var boardStage = CreateBoardStage(boardId, stage.Id, groupId, boardStages.Count);

			context.Stages.Add(stage);
			Save();

			context.BoardStages.Add(boardStage);
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
		/// <param name="groupId"></param>
		public void AssignGroupToStage(string boardId, string stageId, string groupId)
		{
			var boardStage = context.BoardStages.FirstOrDefault(bs => bs.BoardId == boardId && bs.StageId == stageId);
			if (boardStage != null)
			{
				boardStage.GroupId = groupId;
				context.BoardStages.Update(boardStage);
				Save();
			}
		}

		/// <summary>
		/// Gets the stages for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		public ICollection<Stage> GetStages(string boardId)
		{
			var query = @"
		    SELECT bs.BoardId, bs.StageId, s.*
		    FROM BoardStages bs
		    JOIN Stages s ON bs.StageId = s.Id
		    WHERE bs.BoardId = @BoardId
			ORDER BY bs.StageOrder";

			var stages = context.Stages
				.FromSqlRaw(query, new SqlParameter("@BoardId", boardId))
				.ToList();

			return stages;
		}

		/// <summary>
		/// Gets the board stages for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		public ICollection<BoardStage> GetBoardStages(string boardId)
		{
			var query = @"
		    SELECT *
		    FROM BoardStages bs
		    WHERE bs.BoardId = @BoardId
			ORDER BY bs.StageOrder";

			var boardStages = context.BoardStages
				.FromSqlRaw(query, new SqlParameter("@BoardId", boardId))
				.ToList();

			return boardStages;
		}

		/// <summary>
		/// Gets the groups assigned to each stage for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		public Dictionary<string, string> GetAllAssignedGroupsForStages(string boardId)
		{
			var query = @"
			SELECT bs.BoardId, bs.StageId, bs.GroupId, g.GroupName
			FROM BoardStages bs
			JOIN Groups g ON bs.GroupId = g.Id
			WHERE bs.BoardId = @BoardId";

			var assignedGroups = context.BoardStages
				.FromSqlRaw(query, new SqlParameter("@BoardId", boardId))
				.Select(bg => new { StatusId = bg.StageId, bg.Group.GroupName })
				.ToList();

			return assignedGroups.ToDictionary(bg => bg.StatusId, bg => bg.GroupName);
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
			var status = new Stage
			{
				Id = Guid.NewGuid().ToString(),
				Name = stageName
			};
			return status;
		}

		private BoardStage CreateBoardStage(string boardId, string stageId, string groupId, int stageOrder)
		{
			var boardStatus = new BoardStage
			{
				BoardId = boardId,
				StageId = stageId,
				GroupId = groupId,
				StageOrder = stageOrder + 1
			};
			return boardStatus;
		}

		private void AddDefaultBoardStages(Board board, string groupId)
		{
			foreach (var stage in _defaultStages)
			{
				var boardStatus = new BoardStage
				{
					BoardId = board.Id,
					StageId = stage.Id,
					GroupId = groupId,
					StageOrder = _defaultStages.IndexOf(stage)
				};
				context.BoardStages.Add(boardStatus);
			}
			Save();
		}
	}
}