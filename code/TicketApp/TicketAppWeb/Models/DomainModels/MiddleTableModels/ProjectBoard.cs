namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	public class ProjectBoard
	{
		public string ProjectId { get; set; }
		public string BoardId { get; set; }

		public Project Project { get; set; }
		public Board Board { get; set; }
	}
}
