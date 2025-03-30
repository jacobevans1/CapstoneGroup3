// Capstone Group 3
// Spring 2025

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

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
			Insert(board);
			AddDefaultBoardStages(board);
			Save();
		}

		/// <summary>
		/// Adds a new status to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageName"></param>
		/// <param name="groupId"></param>
		public void AddStatus(string boardId, string stageName, string groupId)
		{
			var status = CreateStage(stageName);
			var boardStatus = CreateBoardStage(boardId, status.Id, groupId);

			context.Stages.Add(status);
			context.SaveChanges();

			context.BoardStages.Add(boardStatus);
			context.SaveChanges();
		}

		/// <summary>
		/// Deletes a status for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageId"></param>
		public void DeleteStage(string boardId, string stageId)
		{
			var boardStatus = context.BoardStages.FirstOrDefault(bs => bs.BoardId == boardId && bs.StageId == stageId);
			if (boardStatus != null)
			{
				context.BoardStages.Remove(boardStatus);
				Save();
			}

			var status = context.Stages.FirstOrDefault(s => s.Id == stageId);
			if (status != null)
			{
				context.Stages.Remove(status);
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
			var status = context.Stages.FirstOrDefault(s => s.Id == stageId);
			if (status != null)
			{
				status.Name = newStageName;
				context.Stages.Update(status);
				Save();
			}
		}

		/// <summary>
		/// Assigns a group to a status.
		/// </summary>
		/// <param name="boardStageId"></param>
		/// <param name="stageId"></param>
		/// <param name="groupId"></param>
		public void AssignGroupToStage(string boardStageId, string stageId, string groupId)
		{
			var boardStatus = context.BoardStages.FirstOrDefault(bs => bs.BoardId == boardStageId && bs.StageId == stageId);
			if (boardStatus != null)
			{
				boardStatus.GroupId = groupId;
				context.BoardStages.Update(boardStatus);
				Save();
			}
		}

		/// <summary>
		/// Gets the statuses for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		public ICollection<Stage> GetBoardStages(string boardId)
		{
			var query = @"
		    SELECT bs.BoardId, bs.StageId, s.*
		    FROM BoardStages bs
		    JOIN Stages s ON bs.StageId = s.Id
		    WHERE bs.BoardId = @BoardId";

			var statuses = context.Stages
				.FromSqlRaw(query, new SqlParameter("@BoardId", boardId))
				.ToList();

			return statuses;
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

		private Board CreateBoard(Project project)
		{
			var board = new Board();

			board.Id = Guid.NewGuid().ToString();
			board.BoardName = $"{project.ProjectName} Board";
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

		private BoardStage CreateBoardStage(string boardId, string stageId, string groupId)
		{
			var boardStatus = new BoardStage
			{
				BoardId = boardId,
				StageId = stageId,
				GroupId = groupId
			};
			return boardStatus;
		}

		private void AddDefaultBoardStages(Board board)
		{
			foreach (var status in _defaultStages)
			{
				var boardStatus = new BoardStage
				{
					BoardId = board.Id,
					StageId = status.Id,
				};
				context.BoardStages.Add(boardStatus);
			}

			context.SaveChanges();
		}
	}
}