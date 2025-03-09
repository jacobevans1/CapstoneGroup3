using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels
{
	/// <summary>
	/// The view model for the Group controller.
	/// </summary>
	public class GroupViewModel
	{
		/// <summary>
		/// The currently logged in user
		/// </summary>
		public TicketAppUser CurrentUser { get; set; }

		/// <summary>
		/// The currently logged in user's role.
		/// </summary>
		public string? CurrentUserRole { get; set; }

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		public Group Group { get; set; } = new();

		/// <summary>
		/// Gets or sets the assigned users.
		/// </summary>
		public IEnumerable<TicketAppUser> AssignedUsers { get; set; } = new List<TicketAppUser>();

		/// <summary>
		/// Gets or sets the selected user IDs.
		/// </summary>
		public string?[] SelectedUserIds { get; set; } = Array.Empty<string>();

		/// <summary>
		/// Gets or sets the available users.
		/// </summary>
		public IEnumerable<TicketAppUser> AvailableUsers { get; set; } = new List<TicketAppUser>();

		/// <summary>
		/// Gets or sets the available group managers.
		/// </summary>
		public IEnumerable<TicketAppUser> AvailableGroupManagers { get; set; } = new List<TicketAppUser>();

		/// <summary>
		///	Gets or sets the group manager's ID.
		/// </summary>
		public string? GroupManagerId { get; set; }

		/// <summary>
		/// Gets or sets the group manager's name.
		/// </summary>
		public string? GroupManagerName { get; set; }

        public string? SearchGroupName { get; set; }
        public string? SearchGroupLead { get; set; }

        // Gets or sets the groups.
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();

        public IEnumerable<Group> FilteredGroups => Groups
        .Where(g => string.IsNullOrEmpty(SearchGroupName) || g.GroupName.Contains(SearchGroupName, StringComparison.OrdinalIgnoreCase))
        .Where(g => string.IsNullOrEmpty(SearchGroupLead) || (g.Manager != null && g.Manager.FullName.Contains(SearchGroupLead, StringComparison.OrdinalIgnoreCase)))
        .ToList();
    

    // Gets or sets the current route (contains filtering/sorting information).
    public GroupGridData CurrentRoute { get; set; } = new GroupGridData();

		/// <summary>
		/// Gets or sets the total number of pages.
		/// </summary>
		public int TotalPages { get; set; }

		/// <summary>
		/// The page sizes.
		/// </summary>
		public readonly int[] PageSizes = { 5, 10, 20, 50 };

		/// <summary>
		/// Gets or sets the size of the selected page.
		/// </summary>
		public int SelectedPageSize { get; set; } = 10;

		/// <summary>
		/// Gets or sets the search term.
		/// </summary>
		public string? SearchTerm { get; set; }

		/// <summary>
		/// Checks if the current user is the group manager for the group.
		/// </summary>
		/// <param name="group"></param>
		public bool IsCurrentUserGroupManagerForGroup(Group group)
		{
			return group.ManagerId == CurrentUser.Id;
		}
	}
}
