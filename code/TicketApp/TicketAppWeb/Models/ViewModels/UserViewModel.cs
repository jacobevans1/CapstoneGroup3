using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.ViewModels
{
	public class UserViewModel
	{
		public IEnumerable<TicketAppUser> Users { get; set; } = new List<TicketAppUser>();
	}

}
