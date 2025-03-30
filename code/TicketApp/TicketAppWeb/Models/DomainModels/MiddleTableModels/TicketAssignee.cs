// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	/// <summary>
	/// Represents the association between a ticket and a user assigned to it.
	/// </summary>
	public class TicketAssignee
	{
		/// <summary>
		/// Gets or sets the ticket identifier.
		/// </summary>
		public string TicketId { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the ticket associated with the user.
		/// </summary>
		public Ticket Ticket { get; set; }

		/// <summary>
		/// Gets or sets the user associated with the ticket.
		/// </summary>
		public TicketAppUser User { get; set; }
	}
}
