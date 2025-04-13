using System.ComponentModel.DataAnnotations.Schema;

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
		public string? Id { get; set; }

		/// <summary>
		/// Gets or sets the title of the ticket.
		/// </summary>
		public string? Title { get; set; }

		/// <summary>
		/// Gets or sets the description of the ticket.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets or sets the creation date of the ticket.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the user who created the ticket.
		/// </summary>
		public string? CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the user to whom the ticket is assigned.
		/// </summary>
		public string? AssignedTo { get; set; }

		/// <summary>
		/// Gets or sets the user to whom the ticket is assigned.
		/// </summary>
		[NotMapped]
		public TicketAppUser? AssignedToUser { get; set; }

		/// <summary>
		/// Gets or sets the stage of the ticket.
		/// </summary>
		public string? Stage { get; set; }

		/// <summary>
		/// Gets or sets the completion status of the ticket.
		/// </summary>
		public bool IsComplete { get; set; }

		/// <summary>
		/// Gets or sets the board identifier.
		/// </summary>
		public string? BoardId { get; set; }

		/// <summary>
		/// Gets or sets the history of changes made to the ticket.
		/// </summary>
		public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
	}
}