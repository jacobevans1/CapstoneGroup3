using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels;

/// <summary>
/// The view model for the Project controller.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectViewModel
{
	/// <summary>
	/// The project object to store the project details.
	/// </summary>
	public Project Project { get; set; } = new();

	/// <summary>
	/// The names of the groups assigned to the project.
	/// </summary>
	public IEnumerable<Group> AssignedGroups { get; set; } = new List<Group>();

	/// <summary>
	/// The selected group IDs.
	/// </summary>
	public string?[] SelectedGroupIds { get; set; } = Array.Empty<string>();

	/// <summary>
	/// The available groups based on the selected projects.
	/// </summary>
	public IEnumerable<Group> AvailableGroups { get; set; } = new List<Group>();

	/// <summary>
	/// Stores the available group leads based on the selected projects.
	/// </summary>
	public IEnumerable<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

	/// <summary>
	/// Stores the selected group lead ID.
	/// </summary>
	public string? ProjectLeadId { get; set; }

	/// <summary>
	/// The list of projects from the database.
	/// </summary>
	public IEnumerable<Project> Projects { get; set; } = new List<Project>();

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
}
