using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels
{
	/// <summary>
	/// The UserViewModel class represents a data structure for the user view.
	/// </summary>
	public class UserViewModel
	{
		/// <summary>
		/// The currently logged in user's role.
		/// </summary>
		public string? CurrentUserRole { get; set; }

		/// <summary>
		///	The user object to store the user details. Used for creating a new user or editing an existing user.
		/// </summary>
		public TicketAppUser User { get; set; } = new();

		/// <summary>
		/// The list of all the users from the database.
		/// </summary>
		public IEnumerable<TicketAppUser> Users { get; set; } = new List<TicketAppUser>();

		/// <summary>
		/// The dictionary of users and their roles. Used for displaying the user grid.
		/// </summary>
		public Dictionary<TicketAppUser, string> UserRoles { get; set; } = new Dictionary<TicketAppUser, string>();

		/// <summary>
		/// The list of available roles from the database. Used for the dropdown.
		/// </summary>
		public IEnumerable<IdentityRole> AvailableRoles { get; set; } = new List<IdentityRole>();

		/// <summary>
		/// The name of the role selected in the dropdown. Used for creating a new user or editing an existing user.
		/// </summary>
		public string? SelectedRoleName { get; set; }

		/// <summary>
		/// The current route for the user grid.
		/// </summary>
		public UserGridData CurrentRoute { get; set; } = new UserGridData();

		/// <summary>
		/// The total number of pages in the user grid.
		/// </summary>
		public int TotalPages { get; set; }

		/// <summary>
		/// The list of page sizes for the user grid.
		/// </summary>
		public readonly int[] PageSizes = { 5, 10, 20, 50 };

		/// <summary>
		/// The selected page size for the user grid.
		/// </summary>
		public int SelectedPageSize { get; set; } = 10;

		/// <summary>
		/// The search term for the user grid.
		/// </summary>
		public string? SearchTerm { get; set; }
	}

}
