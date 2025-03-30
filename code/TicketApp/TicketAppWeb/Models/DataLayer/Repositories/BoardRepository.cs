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
		private readonly List<Status> _defaultStatuses;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoardRepository"/> class.
		/// </summary>
		/// <param name="ctx"></param>
		public BoardRepository(TicketAppContext ctx) : base(ctx)
		{
			_defaultStatuses = new List<Status>
			{
				new Status { Id = "5de8ea48-a734-4b97-bc39-18070e4e25a9", Name = "Todo" },
				new Status { Id = "8409c38a-5fab-4283-8957-1c5888d7716d", Name = "In Progress" },
				new Status { Id = "bd0ccc84-e3bd-4417-85ae-15ac2457e390", Name = "Done" }
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
			AddDefaultBoardStatuses(board);
			Save();
		}

		/// <summary>
		/// Deletes a board for the specified project.
		/// </summary>
		/// <param name="project"></param>
		public void DeleteBoard(Project project)
		{
			var board = context.Boards.FirstOrDefault(b => b.ProjectId == project.Id);
			Delete(board);
			Save();
		}

		/// <summary>
		/// Adds a new status to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="newStatusName"></param>
		/// <param name="groupId"></param>
		public void AddStatus(string boardId, string newStatusName, string groupId)
		{
			var status = CreateStatus(newStatusName);
			var boardStatus = CreateBoardStatus(boardId, status.Id, groupId);

			context.Statuses.Add(status);
			Console.WriteLine($"Status Entity State: {context.Entry(status).State}");
			var affectedRows = context.SaveChanges();
			Console.WriteLine($"Affected Rows: {affectedRows}");


			context.BoardStatuses.Add(boardStatus);
			Console.WriteLine($"BoardStatus Entity State: {context.Entry(boardStatus).State}");
			var affectedRows2 = context.SaveChanges();
			Console.WriteLine($"Affected Rows: {affectedRows2}");

		}

		/// <summary>
		/// Assigns a group to a status.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="statusId"></param>
		/// <param name="groupId"></param>
		public void AssignGroupToStatus(string boardId, string statusId, string groupId)
		{
			var boardStatus = context.BoardStatuses.FirstOrDefault(bs => bs.BoardId == boardId && bs.StatusId == statusId);
			if (boardStatus != null)
			{
				boardStatus.GroupId = groupId;
				context.BoardStatuses.Update(boardStatus);
				Save();
			}
		}

		/// <summary>
		/// Gets the statuses for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <returns></returns>
		public ICollection<Status> GetStatusesForBoard(string boardId)
		{
			var query = @"
		    SELECT bs.BoardId, bs.StatusId, s.*
		    FROM BoardStatuses bs
		    JOIN Statuses s ON bs.StatusId = s.Id
		    WHERE bs.BoardId = @BoardId";

			var statuses = context.Statuses
				.FromSqlRaw(query, new SqlParameter("@BoardId", boardId))
				.ToList();

			return statuses;
		}

		private Board CreateBoard(Project project)
		{
			var board = new Board();

			board.Id = Guid.NewGuid().ToString();
			board.BoardName = $"{project.ProjectName} Board";
			board.Project = project;

			return board;
		}

		private Status CreateStatus(string statusName)
		{
			var status = new Status
			{
				Id = Guid.NewGuid().ToString(),
				Name = statusName
			};
			return status;
		}

		private BoardStatus CreateBoardStatus(string boardId, string statusId, string groupId)
		{
			var boardStatus = new BoardStatus
			{
				BoardId = boardId,
				StatusId = statusId,
				GroupId = groupId
			};
			return boardStatus;
		}

		private void AddDefaultBoardStatuses(Board board)
		{
			foreach (var status in _defaultStatuses)
			{
				var boardStatus = new BoardStatus
				{
					BoardId = board.Id,
					StatusId = status.Id,
				};
				context.BoardStatuses.Add(boardStatus);
			}

			context.SaveChanges();
		}
	}
}