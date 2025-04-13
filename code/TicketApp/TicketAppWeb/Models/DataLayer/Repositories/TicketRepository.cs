using Microsoft.EntityFrameworkCore;
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

            // Add creation history record
            ticket.History.Add(new TicketHistory
            {
                Id = Guid.NewGuid().ToString(),
                PropertyChanged = "Created",
                OldValue = null,
                NewValue = ticket.Title,
                ChangedByUserId = ticket.CreatedBy,
                ChangeDate = DateTime.Now,
                ChangeDescription = $"Ticket '{ticket.Title}' was created"
            });

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

            var existingTicket = Get(ticket.Id);
            if (existingTicket == null)
            {
                throw new ArgumentException("Ticket not found", nameof(ticket));
            }

            // Track title changes
            if (existingTicket.Title != ticket.Title)
            {
                ticket.History.Add(new TicketHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    PropertyChanged = "Title",
                    OldValue = existingTicket.Title,
                    NewValue = ticket.Title,
                    ChangedByUserId = ticket.CreatedBy,
                    ChangeDate = DateTime.Now,
                    ChangeDescription = $"Title changed from '{existingTicket.Title}' to '{ticket.Title}'"
                });
            }

            // Track description changes
            if (existingTicket.Description != ticket.Description)
            {
                ticket.History.Add(new TicketHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    PropertyChanged = "Description",
                    OldValue = existingTicket.Description,
                    NewValue = ticket.Description,
                    ChangedByUserId = ticket.CreatedBy,
                    ChangeDate = DateTime.Now,
                    ChangeDescription = "Description was updated"
                });
            }

            // Track assignment changes
            if (existingTicket.AssignedTo != ticket.AssignedTo)
            {
                ticket.History.Add(new TicketHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    PropertyChanged = "AssignedTo",
                    OldValue = existingTicket.AssignedTo,
                    NewValue = ticket.AssignedTo,
                    ChangedByUserId = ticket.CreatedBy,
                    ChangeDate = DateTime.Now,
                    ChangeDescription = $"Assignment changed from user {existingTicket.AssignedTo} to {ticket.AssignedTo}"
                });
            }

            // Track stage changes
            if (existingTicket.Stage != ticket.Stage)
            {
                ticket.History.Add(new TicketHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    PropertyChanged = "Stage",
                    OldValue = existingTicket.Stage,
                    NewValue = ticket.Stage,
                    ChangedByUserId = ticket.CreatedBy,
                    ChangeDate = DateTime.Now,
                    ChangeDescription = $"Status changed from {existingTicket.Stage} to {ticket.Stage}"
                });
            }

            // Track completion changes
            if (existingTicket.IsComplete != ticket.IsComplete)
            {
                ticket.History.Add(new TicketHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    PropertyChanged = "IsComplete",
                    OldValue = existingTicket.IsComplete.ToString(),
                    NewValue = ticket.IsComplete.ToString(),
                    ChangedByUserId = ticket.CreatedBy,
                    ChangeDate = DateTime.Now,
                    ChangeDescription = ticket.IsComplete
                        ? "Ticket marked as complete"
                        : "Ticket reopened"
                });
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

        public Ticket GetTicketWithHistory(string ticketId)
        {
            var ticket = context.Tickets
                .Include(t => t.History)
                    .ThenInclude(h => h.ChangedByUser)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket != null && !string.IsNullOrEmpty(ticket.AssignedTo))
            {
                ticket.AssignedToUser = context.Users
                    .FirstOrDefault(u => u.Id == ticket.AssignedTo);
            }

            return ticket;
        }

    }

}
