using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

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
		/// <param name="isAdmin"></param>
		void AddBoard(Project project, bool isAdmin);

		/// <summary>
		/// Adds a new stage to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageName"></param>
		/// <param name="groupIds"></param>
		void AddStage(string boardId, string stageName, List<string> groupIds);

		/// <summary>
		/// Deletes a stage for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageId"></param>
		void DeleteStage(string boardId, string stageId);

		/// <summary>
		/// Renames a stage on the board.
		/// </summary>
		/// <param name="stageId"></param>
		/// <param name="newStageName"></param>
		void RenameStage(string stageId, string newStageName);

		/// <summary>
		/// Assigns a group to a stage.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageId"></param>
		/// <param name="groupId"></param>
		void AssignGroupToStage(string boardId, string stageId, List<string> groupId);

		/// <summary>
		/// Gets the stages for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		ICollection<Stage> GetStages(string boardId);

		/// <summary>
		/// Gets the board stages for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		ICollection<BoardStage> GetBoardStages(string boardId);

		/// <summary>
		/// Gets the groups assigned to each stage for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		Dictionary<string, List<Group>> GetBoardStageGroups(string boardId);

		/// <summary>
		/// Gets the tickets assigned to each stage for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		Dictionary<string, List<Ticket>> GetBoardStageTickets(string boardId);

		/// <summary>
		/// Saves the board stages to the database.
		/// </summary>
		/// <param name="boardStages"></param>
		void SaveBoardStages(List<BoardStage> boardStages);

		/// <summary>
		/// Deletes the board stage groups for the specified board and stage.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageId"></param>
		void DeleteBoardStageGroups(string boardId, string stageId);
	}
}
