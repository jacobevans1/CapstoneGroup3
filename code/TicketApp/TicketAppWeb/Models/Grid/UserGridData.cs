using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ExtensionMethods;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.Grid
{
	/// <summary>
	/// The UserGridData class represents a data structure for paging and sorting of users.
	/// </summary>
	public class UserGridData : GridData
	{
		/// <summary>
		/// The default constructor initializes the sort field to the last name.
		/// </summary>
		public UserGridData() => SortField = nameof(TicketAppUser.LastName);

		/// <summary>
		///	Indicates whether the grid is sorted by the first name.
		/// </summary>
		public bool IsSortByFirstName =>
			SortField.EqualsNoCase(nameof(TicketAppUser.FirstName));
	}
}
