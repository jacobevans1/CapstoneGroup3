using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels;

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
	/// Gets or sets the name of the user search string.
	/// </summary>
	public string? UserNameSearchString { get; set; }

	/// <summary>
	/// Gets the filtered users based on the search string.
	/// </summary>
	public IEnumerable<TicketAppUser> FilteredUsers => Users
		.Where(u => string.IsNullOrEmpty(UserNameSearchString) || u.FullName!.Contains(UserNameSearchString, StringComparison.OrdinalIgnoreCase)).ToList();
}
