using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels
{
	public class TicketViewModel
	{
		/// <summary>
		/// The currently logged in user
		/// </summary>
		[ValidateNever]
		public TicketAppUser CurrentUser { get; set; }

		/// <summary>
		/// The currently logged in user's role.
		/// </summary>
		[ValidateNever]
		public string? CurrentUserRole { get; set; }

		/// <summary>
		/// Gets or sets the current ticket.
		/// </summary>
		[ValidateNever]
		public Ticket Ticket { get; set; }

		/// <summary>
		/// Gets or sets the new ticket to add.
		/// </summary>
		[ValidateNever]
		public Ticket NewTicket { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the ticket.
		/// </summary>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the board associated with the ticket.
		/// </summary>
		public Board Board { get; set; }

		/// <summary>
		/// Initializes a new instance of the TicketViewModel class.
		/// </summary>
		public TicketViewModel()
		{
			Ticket = new Ticket();
			NewTicket = new Ticket();
			Project = new Project();
			Board = new Board();
		}
	}
}
