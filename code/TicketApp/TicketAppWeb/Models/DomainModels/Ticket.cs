// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels
{
	/// <summary>
	/// Represents a task or issue that needs to be tracked
	/// </summary>
	public class Ticket
	{
		/// <summary>
		/// Gets or sets the ticket identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the title of the ticket.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description of the ticket.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the status of the ticket.
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the list of assignees for the ticket.
		/// </summary>
		public List<TicketAppUser> Assignees { get; set; }

		/// <summary>
		/// Gets or sets the board identifier.
		/// </summary>
		public string BoardId { get; set; }
	}
}
