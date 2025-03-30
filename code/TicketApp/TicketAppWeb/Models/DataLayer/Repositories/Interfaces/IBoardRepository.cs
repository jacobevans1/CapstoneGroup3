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
		/// Adds a new stage to the board.
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="stageName"></param>
		/// <param name="groupId"></param>
		void AddStatus(string boardId, string stageName, string groupId);

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
		/// <param name="boardStageId"></param>
		/// <param name="stageId"></param>
		/// <param name="groupId"></param>
		void AssignGroupToStage(string boardStageId, string stageId, string groupId);

		/// <summary>
		/// Gets the stages for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		ICollection<Stage> GetBoardStages(string boardId);

		/// <summary>
		/// Gets the groups assigned to each stage for the specified board.
		/// </summary>
		/// <param name="boardId"></param>
		Dictionary<string, string> GetAllAssignedGroupsForStages(string boardId);
	}
}
