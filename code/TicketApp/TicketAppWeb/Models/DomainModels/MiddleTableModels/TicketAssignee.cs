namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	public class TicketAssignee
	{
		public string TicketId { get; set; }
		public string UserId { get; set; }

		public Ticket Ticket { get; set; }
		public TicketAppUser User { get; set; }
	}
}
