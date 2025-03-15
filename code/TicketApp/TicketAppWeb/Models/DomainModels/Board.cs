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
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the board.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the board.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project identifier.
		/// </summary>
		public string ProjectId { get; set; }

		//public List<Ticket> Tickets { get; set; }
	}
}
