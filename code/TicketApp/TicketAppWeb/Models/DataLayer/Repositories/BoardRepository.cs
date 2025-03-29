// Capstone Group 3
// Spring 2025

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
			newBoard.Project = result?.Project;
			newBoard.Statuses = await (from bs in context.BoardStatuses
									   join s in context.Statuses on bs.StatusId equals s.Id
									   where bs.BoardId == newBoard.Id
									   select s).ToListAsync();

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
			AddBoardStatuses(board);
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

		private Board CreateBoard(Project project)
		{
			var board = new Board();

			board.Id = Guid.NewGuid().ToString();
			board.BoardName = $"{project.ProjectName} Board";
			board.Project = project;

			return board;
		}

		private void AddBoardStatuses(Board board)
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
