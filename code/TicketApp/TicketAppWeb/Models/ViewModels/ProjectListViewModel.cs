using TicketAppWeb.Models.Grid;
using Microsoft.CodeAnalysis;

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

    // Gets or sets the current route.
    public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

    // Gets or sets the total pages.
    public int TotalPages { get; set; }

    public readonly int[] PageSizes = { 1, 2, 3, 4, 5};

}