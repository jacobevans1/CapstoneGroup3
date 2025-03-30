using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TicketAppWeb.Models.DomainModels
{
	public class Stage
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public ICollection<BoardStage> BoardStages { get; set; }
	}
}
