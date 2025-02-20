using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.Models.ViewModels;

/// <summary>
/// The ProjectListViewModel class represents the data for the project list view.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectListViewModel
{
    // Gets or sets the projects.
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();

    // Gets or sets the current route (contains filtering/sorting information).
    public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

    // Gets or sets the current page.
    public int CurrentPage { get; set; } = 1;

    // Gets or sets the total pages.
    public int TotalPages { get; set; }

    // Gets or sets the available page sizes.
    public readonly int[] PageSizes = { 5, 10, 20, 50 };

    // Gets or sets the selected page size.
    public int SelectedPageSize { get; set; } = 10;

    // Gets or sets the search query (if filtering is applied).
    public string? SearchTerm { get; set; }
}
