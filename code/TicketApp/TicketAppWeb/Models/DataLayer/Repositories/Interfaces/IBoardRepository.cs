using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces
{
	/// <summary>
	/// The IBoardRepository interface defines the methods that must be implemented by all Board repository classes.
	/// </summary>
	public interface IBoardRepository : IRepository<Board>
	{
		/// <summary>
		/// Gets the board for the specified project.
		/// </summary>
		/// <param name="projectId"></param>
		Task<Board?> GetBoardByProjectIdAsync(string projectId);

		/// <summary>
		///	Adds a new board for the specified project.
		/// </summary>
		/// <param name="project"></param>
		void AddBoard(Project project);

		/// <summary>
		/// Deletes a board for the specified project.
		/// </summary>
		/// <param name="project"></param>
		void DeleteBoard(Project project);
	}
}
