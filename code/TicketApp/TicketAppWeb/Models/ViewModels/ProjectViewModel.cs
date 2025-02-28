using System.ComponentModel.DataAnnotations;
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
	[Required(ErrorMessage ="Please assign a project lead")]
	public string? ProjectLeadId { get; set; }

	[Required(ErrorMessage ="Please provide a project name")]
	public string? ProjectName { get; set; }

	public string? Description { get; set; }

    [Required(ErrorMessage = "Please assign at least one group")]
    public List<string> SelectedGroupIds { get; set; } = new List<string>();

    //public Project Project { get; set; } = new();

    public List<Group> AssignedGroups { get; set; } = new List<Group>();

    public List<Group> AvailableGroups { get; set; } = new List<Group>();

    public List<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

    public IEnumerable<Project> Projects { get; set; } = new List<Project>();

	public Dictionary<Project, List<Group>> ProjectGroups { get; set; } = new Dictionary<Project, List<Group>>();

	public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

	public int TotalPages { get; set; }

	public readonly int[] PageSizes = { 5, 10, 20, 50 };

	public int SelectedPageSize { get; set; } = 10;
}
