using TicketAppWeb.Models.DomainModels.MiddleTableModels;

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
		/// Gets or sets the project identifier associated with the board's project.
		/// </summary>
		public string ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the board's project.
		/// </summary>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the list of board stages associated with the board.
		/// </summary>
		public ICollection<BoardStage> BoardStages { get; set; }

		/// <summary>
		/// Gets or sets the list of tickets associated with the board.
		/// </summary>
		public ICollection<Ticket> Tickets { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Board"/> class.
		/// </summary>
		public Board()
		{
			BoardStages = new List<BoardStage>();
			Tickets = new List<Ticket>();
		}
	}
}
