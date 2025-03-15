// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels
{
	/// <summary>
	/// Represents a project board that tracks stages of tickets
	/// </summary>
	public class Board
	{
		/// <summary>
		/// Gets or sets the board identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the board.
		/// </summary>
		public string BoardName { get; set; }

		/// <summary>
		/// Gets or sets the description of the board.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project identifier.
		/// </summary>
		public List<Ticket> Tickets { get; set; }

		/// <summary>
		/// Gets or sets the list of statuses for tickets on the board.
		/// </summary>
		public List<string> StatusList { get; set; }

		/// <summary>
		/// Gets or sets the project identifier.
		/// </summary>
		public string ProjectId { get; set; }
	}
}
