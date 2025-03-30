namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	public class BoardStage
	{
		public string BoardId { get; set; }
		public string StageId { get; set; }
		public string GroupId { get; set; }

		public int StageOrder { get; set; }

		public Board Board { get; set; }
		public Stage Stage { get; set; }
		public Group Group { get; set; }
	}
}
