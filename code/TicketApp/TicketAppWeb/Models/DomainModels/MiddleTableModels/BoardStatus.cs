namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	public class BoardStatus
	{
		public string BoardId { get; set; }
		public string StatusId { get; set; }
		public string GroupId { get; set; }

		public Board Board { get; set; }
		public Status Status { get; set; }
		public Group Group { get; set; }
	}
}
