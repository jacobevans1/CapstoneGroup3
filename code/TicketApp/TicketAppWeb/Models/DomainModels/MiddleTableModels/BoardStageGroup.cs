// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	/// <summary>
	/// Represents the association between a group and a board stage.
	/// </summary>
	public class BoardStageGroup
	{
		/// <summary>
		/// Gets or sets the unique identifier for the association.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the board identifier.
		/// </summary>
		public string BoardId { get; set; }

		/// <summary>
		/// Gets or sets the stage identifier.
		/// </summary>
		public string StageId { get; set; }

		/// <summary>
		/// Gets or sets the group identifier.
		/// </summary>
		public string GroupId { get; set; }

		/// <summary>
		/// Gets or sets the board stage associated with this group.
		/// </summary>
		public BoardStage BoardStage { get; set; }

		/// <summary>
		/// Gets or sets the group associated with this board stage.
		/// </summary>
		public Group Group { get; set; }

	}
}
