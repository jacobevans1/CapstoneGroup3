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
                throw new ArgumentNullException(nameof(ticket));

            var now = DateTime.Now;
            var user = context.Users.FirstOrDefault(u => u.Id == ticket.CreatedBy);
            var assignee = context.Users.FirstOrDefault(u => u.Id == ticket.AssignedTo);
            var stageName = context.Stages.FirstOrDefault(s => s.Id == ticket.Stage)?.Name ?? "Unspecified";

            string historyEntry = $"{user?.FullName ?? "Unknown User"} created \"{ticket.Title}\" at {now:g}.";
            if (assignee != null)
                historyEntry += $" Initial Assignee: {assignee.FullName}.";
            if (!string.IsNullOrEmpty(stageName))
                historyEntry += $" Initial Stage: {stageName}.";

            ticket.History.Add(new TicketHistory
            {
                Id = Guid.NewGuid().ToString(),
                PropertyChanged = "Created",
                OldValue = null,
                NewValue = ticket.Title,
                ChangedByUserId = ticket.CreatedBy,
                ChangeDate = now,
                ChangeDescription = historyEntry
            });

            Insert(ticket);
            Save();
        }


        /// <summary>
        /// Updates a ticket in the database.
        /// </summary>
        /// <param name="ticket"></param>
        public void UpdateTicket(Ticket original, Ticket updated)
        {
            Console.WriteLine($"Comparing title: existing = '{original.Title}', incoming = '{updated.Title}'");

            if (original.Title?.Trim() != updated.Title?.Trim())
            {
                Console.WriteLine($"📝 Title changed from '{original.Title}' to '{updated.Title}'");
            }

            // Apply changes
            original.Title = updated.Title;
            original.Description = updated.Description;
            original.AssignedTo = updated.AssignedTo;
            original.Stage = updated.Stage;

            context.SaveChanges();
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
