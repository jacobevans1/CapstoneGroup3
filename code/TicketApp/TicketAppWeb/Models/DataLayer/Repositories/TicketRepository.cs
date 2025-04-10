using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories
{
	/// <summary>
	/// The TicketRepository class implements the ITicketRepository interface.
	/// </summary>
	/// <param name="ctx"></param>
	public class TicketRepository(TicketAppContext ctx) : Repository<Ticket>(ctx), ITicketRepository
	{
		/// <summary>
		/// Adds a ticket to the database.
		/// </summary>
		/// <param name="ticket"></param>
		public void AddTicket(Ticket ticket)
		{
			if (ticket == null)
			{
				throw new ArgumentNullException(nameof(ticket));
			}
			Insert(ticket);
			Save();
		}

		/// <summary>
		/// Updates a ticket in the database.
		/// </summary>
		/// <param name="ticket"></param>
		public void UpdateTicket(Ticket ticket)
		{
			if (ticket == null)
			{
				throw new ArgumentNullException(nameof(ticket));
			}
			Update(ticket);
			Save();
		}

		/// <summary>
		/// Deletes a ticket from the database.
		/// </summary>
		/// <param name="ticketId"></param>
		public void DeleteTicket(string ticketId)
		{
			var ticket = Get(ticketId);
			if (ticket == null)
			{
				throw new ArgumentNullException(nameof(ticket));
			}
			Delete(ticket);
			Save();
		}
	}
}