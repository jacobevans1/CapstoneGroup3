using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.Models.ViewModels;

/// <summary>
/// The view model for the Project controller.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectViewModel
{
    public Project Project { get; set; } = new();

    public IEnumerable<Group> AssignedGroups { get; set; } = new List<Group>();

    public List<string> SelectedGroupIds { get; set; } = new List<string>();

    public IEnumerable<Group> AvailableGroups { get; set; } = new List<Group>();

    // Now dynamically updating group leads based on selected groups
    public IEnumerable<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

    public string? ProjectLeadId { get; set; }

    public IEnumerable<Project> Projects { get; set; } = new List<Project>();

    public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

    public int TotalPages { get; set; }

    public readonly int[] PageSizes = { 5, 10, 20, 50 };

    public int SelectedPageSize { get; set; } = 10;
}
