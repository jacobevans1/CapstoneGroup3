using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ExtensionMethods;

namespace TicketAppWeb.Models.Grid
{
	public class UserGridData : GridData
	{
		// set initial sort field in constructor
		public UserGridData() => SortField = nameof(TicketAppUser.LastName);

		// sort flags
		public bool IsSortByFirstName =>
			SortField.EqualsNoCase(nameof(TicketAppUser.FirstName));
	}
}
