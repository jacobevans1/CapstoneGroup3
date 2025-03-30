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
		/// Adds a new status to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="statusName"></param>
		/// <param name="groupId"></param>
		void AddStatus(string boardId, string statusName, string groupId);

		/// <summary>
		/// Deletes a status for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="statusId"></param>
		void DeleteStatus(string boardId, string statusId);

		/// <summary>
		/// Renames a status on the board.
		/// </summary>
		/// <param name="statusId"></param>
		/// <param name="newStatusName"></param>
		void RenameStatus(string statusId, string newStatusName);

		/// <summary>
		/// Assigns a group to a status.
		/// </summary>
		/// <param name="boardStatusId"></param>
		/// <param name="statusId"></param>
		/// <param name="groupId"></param>
		void AssignGroupToStatus(string boardStatusId, string statusId, string groupId);

		/// <summary>
		/// Gets the statuses for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		ICollection<Status> GetStatusesForBoard(string boardId);
	}
}
