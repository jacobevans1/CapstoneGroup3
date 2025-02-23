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
    // Gets or sets the project.
    public Project Project { get; set; } = new();

    // Gets or sets the list of assigned groups for the project.
    public IEnumerable<Group> AssignedGroups { get; set; } = new List<Group>();

    // Gets or sets the selected group IDs for project assignment.
    public string?[] SelectedGroupIds { get; set; } = Array.Empty<string>();

    // Gets or sets the list of available groupss
    public IEnumerable<Group> AvailableGroups { get; set; } = new List<Group>();

    // Gets or sets the list of available group leads.
    public IEnumerable<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

    // Gets or sets the project lead's ID.
    public string? ProjectLeadId { get; set; }

    // Gets or sets the project lead's name.
    public string? ProjectLeadName { get; set; }

    // Gets or sets the projects.
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();

    // Gets or sets the current route (contains filtering/sorting information).
    public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

    // Gets or sets the total pages.
    public int TotalPages { get; set; }

    // Gets or sets the available page sizes.
    public readonly int[] PageSizes = { 5, 10, 20, 50 };

    // Gets or sets the selected page size.
    public int SelectedPageSize { get; set; } = 10;

    // Gets or sets the search query (if filtering is applied).
    public string? SearchTerm { get; set; }
}
