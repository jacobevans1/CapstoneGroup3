using TicketAppWeb.Models.DomainModels;

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

    // Gets or sets the list of available groups.
    public IEnumerable<Group> AvailableGroups { get; set; } = new List<Group>();

    // Gets or sets the list of available group leads.
    public IEnumerable<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

    // Gets or sets the project lead's ID.
    public string? ProjectLeadId { get; set; }

    // Gets or sets the project lead's name.
    public string? ProjectLeadName { get; set; }
}
