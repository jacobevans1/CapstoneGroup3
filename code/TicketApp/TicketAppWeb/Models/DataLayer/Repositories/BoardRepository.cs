// Capstone Group 3
// Spring 2025

using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	/// <summary>
	/// The BoardRepository class implements IBoardRepository
	/// </summary>
	public class BoardRepository : Repository<Board>, IBoardRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoardRepository"/> class.
		/// </summary>
		/// <param name="ctx"></param>
		public BoardRepository(TicketAppContext ctx) : base(ctx)
		{
		}

		/// <summary>
		/// Adds a new board for the specified project.
		/// </summary>
		/// <param name="project"></param>
		public void AddBoard(Project project)
		{
			var board = CreateBoard(project);
			Insert(board);
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
	}
}
