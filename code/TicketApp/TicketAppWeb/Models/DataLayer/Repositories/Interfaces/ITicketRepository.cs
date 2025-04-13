using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces
{
	/// <summary>
	/// The ITicketRepository interface defines the methods that must be implemented by all Ticket repository classes.
	/// </summary>
	public interface ITicketRepository : IRepository<Ticket>
	{
		void AddTicket(Ticket ticket);

		void UpdateTicket(Ticket ticket);

		void DeleteTicket(string ticketId);

        Ticket GetTicketWithHistory(string ticketId);


        //Ticket GetTicketById(string ticketId);

        //List<Ticket> GetTicketsByBoardId(string boardId);

        //List<Ticket> GetTicketsByStageId(string boardId, string stageId);

        //List<Ticket> GetTicketsByUserId(string userId);
    }
}
