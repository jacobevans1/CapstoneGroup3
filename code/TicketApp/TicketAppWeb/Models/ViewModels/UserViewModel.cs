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
		///	The user object to store the user details.
		/// </summary>
		public TicketAppUser User { get; set; } = new();

		/// <summary>
		/// The name of the role selected in the dropdown.
		/// </summary>
		public string? SelectedRoleName { get; set; }

		/// <summary>
		/// The list of users from the database.
		/// </summary>
		public IEnumerable<TicketAppUser> Users { get; set; } = new List<TicketAppUser>();

		/// <summary>
		/// The dictionary of users and their roles.
		/// </summary>
		public Dictionary<TicketAppUser, string> UserRoles { get; set; } = new Dictionary<TicketAppUser, string>();

		/// <summary>
		/// The list of roles from the database.
		/// </summary>
		public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();

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
