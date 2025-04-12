// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	/// <summary>
	/// Represents the association between a board and a stage.
	/// </summary>
	public class BoardStage
	{
		/// <summary>
		/// Gets or sets the board identifier.
		/// </summary>
		public string BoardId { get; set; }

		/// <summary>
		/// Gets or sets the stage identifier.
		/// </summary>
		public string StageId { get; set; }

		/// <summary>
		/// Gets or sets the order of the stage in the board.
		/// </summary>
		public int StageOrder { get; set; }

		/// <summary>
		/// Gets or sets the board associated with this stage.
		/// </summary>
		public Board Board { get; set; }

		/// <summary>
		/// Gets or sets the stage associated with this board.
		/// </summary>
		public Stage Stage { get; set; }
	}
}
