using System;

namespace TicketAppWeb.Models.DomainModels
{
	public class TicketHistory
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string TicketId { get; set; }
		public string PropertyChanged { get; set; }
		public string? OldValue { get; set; } 
		public string? NewValue { get; set; }
		public string ChangedByUserId { get; set; }
		public TicketAppUser ChangedByUser { get; set; }
		public DateTime ChangeDate { get; set; } = DateTime.Now;
		public string ChangeDescription { get; set; }
	}
}
