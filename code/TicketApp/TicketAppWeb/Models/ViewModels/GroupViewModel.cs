using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.Models.ViewModels
{
	public class GroupViewModel
	{
		// Gets or sets the group.
		public Group Group { get; set; } = new();

		// Gets or sets the list of assigned users for the group.
		public IEnumerable<TicketAppUser> AssignedUsers { get; set; } = new List<TicketAppUser>();

		// Gets or sets the selected user IDs for group assignment.
		public string?[] SelectedUserIds { get; set; } = Array.Empty<string>();

		// Gets or sets the list of available users.
		public IEnumerable<TicketAppUser> AvailableUsers { get; set; } = new List<TicketAppUser>();

		// Gets or sets the list of available group managers.
		public IEnumerable<TicketAppUser> AvailableGroupManagers { get; set; } = new List<TicketAppUser>();

		// Gets or sets the group manager's ID.
		public string? GroupManagerId { get; set; }

		// Gets or sets the group manager's name.
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
