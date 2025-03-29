// Capstone Group 3
// Spring 2025
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

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
		/// Gets or sets the project identifier associated with the board's project.
		/// </summary>
		public string ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the board's project.
		/// </summary>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the list of board statuses associated with the board.
		/// </summary>
		public ICollection<BoardStatus> BoardStatuses { get; set; }

		/// <summary>
		/// Gets or sets the list of statuses associated with the board.
		/// </summary>
		public ICollection<Status> Statuses { get; set; }

		/// <summary>
		/// Gets or sets the list of tickets associated with the board.
		/// </summary>
		public ICollection<Ticket> Tickets { get; set; }

		public Board()
		{
			Statuses = new List<Status>();
			Tickets = new List<Ticket>();
		}
	}
}
