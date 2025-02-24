using System.Text.Json.Serialization;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ExtensionMethods;

namespace TicketAppWeb.Models.Grid;

/// <summary>
/// The GroupGridData class represents the data for the group grid.
/// Jacob Evans
/// 02/22/2025
/// </summary>
public class GroupGridData : GridData
{
	// set initial sort field in constructor
	public GroupGridData() => SortField = nameof(Group.GroupName);

	// sort flags
	[JsonIgnore]
	public bool IsSortByGroupManager =>
		SortField.EqualsNoCase(nameof(TicketAppUser));
}