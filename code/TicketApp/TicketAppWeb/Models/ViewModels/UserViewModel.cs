using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.Models.ViewModels
{
	public class UserViewModel
	{
		// Gets or sets the user.
		public TicketAppUser User { get; set; } = new();

		// Gets or sets the selected user ID.
		public string? SelectedUserId { get; set; }

		// Gets or sets the users.
		public IEnumerable<TicketAppUser> Users { get; set; } = new List<TicketAppUser>();

		// Gets or sets the current route (contains filtering/sorting information).
		public UserGridData CurrentRoute { get; set; } = new UserGridData();

		// Gets or sets the total pages.
		public int TotalPages { get; set; }

		// Gets or sets the available page sizes.
		public readonly int[] PageSizes = { 5, 10, 20, 50 };

		// Gets or sets the selected page size.
		public int SelectedPageSize { get; set; } = 10;

		// Gets or sets the search query (if filtering is applied).
		public string? SearchTerm { get; set; }
	}

}
