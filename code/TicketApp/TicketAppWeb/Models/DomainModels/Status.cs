using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TicketAppWeb.Models.DomainModels
{
	public class Status
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public ICollection<BoardStatus> BoardStatuses { get; set; }
	}
}
