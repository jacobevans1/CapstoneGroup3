using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels;

/// <summary>
/// The view model for the Project controller.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectViewModel
{
	/// <summary>
	/// The currently logged in user
	/// </summary>
	[ValidateNever]
	public TicketAppUser CurrentUser { get; set; }

	/// <summary>
	/// The currently logged in user's role.
	/// </summary>
	[ValidateNever]
	public string? CurrentUserRole { get; set; }

	/// <summary>
	/// Gets or sets the project lead identifier.
	/// </summary>
	[Required(ErrorMessage = "Please assign a project lead")]
	public string? ProjectLeadId { get; set; }

	/// <summary>
	/// Gets or sets the name of the project.
	/// </summary>
	[Required(ErrorMessage = "Please enter a project name")]
	public string? ProjectName { get; set; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the selected group ids.
	/// </summary>
	[Required(ErrorMessage = "Please assign at least one group")]
	public List<string> SelectedGroupIds { get; set; } = new List<string>();

	/// <summary>
	/// Gets or sets the assigned groups.
	/// </summary>
	public List<Group> AssignedGroups { get; set; } = new List<Group>();

	/// <summary>
	/// Gets or sets the available groups.
	/// </summary>
	public List<Group> AvailableGroups { get; set; } = new List<Group>();

	/// <summary>
	/// Gets or sets the available group leads.
	/// </summary>
	public List<TicketAppUser> AvailableGroupLeads { get; set; } = new List<TicketAppUser>();

	/// <summary>
	/// Gets or sets the projects.
	/// </summary>
	public IEnumerable<Project> Projects { get; set; } = new List<Project>();

	/// <summary>
	/// Gets or sets the name of the search project.
	/// </summary>
	public string? SearchProjectName { get; set; }

	/// <summary>
	/// Gets or sets the search project lead.
	/// </summary>
	public string? SearchProjectLead { get; set; }

	/// <summary>
	/// Gets the filtered projects.
	/// </summary>
	public IEnumerable<Project> FilteredProjects => Projects
		.Where(p => string.IsNullOrEmpty(SearchProjectName) || p.ProjectName!.Contains(SearchProjectName, StringComparison.OrdinalIgnoreCase))
		.Where(p => string.IsNullOrEmpty(SearchProjectLead) || (p.Lead != null && p.Lead.FullName.Contains(SearchProjectLead, StringComparison.OrdinalIgnoreCase)))
		.ToList();


	/// <summary>
	/// Gets or sets the project groups.
	/// </summary>
	public Dictionary<Project, List<Group>> ProjectGroups { get; set; } = new Dictionary<Project, List<Group>>();

	/// <summary>
	/// Gets or sets the current route.
	/// </summary>
	public ProjectGridData CurrentRoute { get; set; } = new ProjectGridData();

	/// <summary>
	/// Gets or sets the total pages.
	/// </summary>
	public int TotalPages { get; set; }

	/// <summary>
	/// The page sizes
	/// </summary>
	public readonly int[] PageSizes = { 5, 10, 20, 50 };

	/// <summary>
	/// Gets or sets the size of the selected page.
	/// </summary>
	public int SelectedPageSize { get; set; } = 10;

	/// <summary>
	/// Checks if the current user is the project lead for the project.
	/// </summary>
	/// <param name="project"></param>
	public bool IsCurrentUserProjectLeadForProject(Project project)
	{
		return project.LeadId == CurrentUser.Id;
	}

	/// <summary>
	/// Checks if the current user is a group manager in the project.
	/// </summary>
	/// <param name="project"></param>
	public bool IsCurrentUserAGroupManagerInProject(Project project)
	{
		foreach (var group in ProjectGroups[project])
		{
			if (group.ManagerId == CurrentUser.Id)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the current user is the group manager for any group.
	/// </summary>
	public bool IsCurrentUserLeadForAnyGroup()
	{
		foreach (var group in AvailableGroups)
		{
			if (group.ManagerId == CurrentUser.Id)
			{
				return true;
			}
		}

		return false;
	}
}
