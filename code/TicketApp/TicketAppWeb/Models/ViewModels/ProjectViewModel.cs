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

    /// Gets or sets the groups.
    public IEnumerable<Group> Groups { get; set; } = new List<Group>();

    // Gets or sets the selected activities.
    public int[] SelectedGroups { get; set; } = Array.Empty<int>();

    // Gets or sets the the groups leads.
    public IEnumerable<TicketAppUser> GroupLeads { get; set; } = new List<TicketAppUser>();


    // Gets or sets the name of the project lead.
    public string? ProjectLeadName { get; set; }
}