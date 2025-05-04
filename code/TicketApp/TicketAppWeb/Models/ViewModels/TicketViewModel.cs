using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels;

public class TicketViewModel
{
	/// <summary>
	/// The currently logged in user
	/// </summary>
	[ValidateNever]
	public TicketAppUser CurrentUser { get; set; } = new();

	/// <summary>
	/// The currently logged in user's role.
	/// </summary>
	[ValidateNever]
	public string? CurrentUserRole { get; set; }

	/// <summary>
	/// Gets or sets the current ticket.
	/// </summary>
	[ValidateNever]
	public Ticket Ticket { get; set; } = new();

	/// <summary>
	/// Gets or sets the new ticket to add.
	/// </summary>
	[ValidateNever]
	public Ticket NewTicket { get; set; } = new();

	/// <summary>
	/// Gets or sets the project associated with the ticket.
	/// </summary>
	[ValidateNever]
	public Project Project { get; set; } = new();

	/// <summary>
	/// Gets or sets the board associated with the ticket.
	/// </summary>
	[ValidateNever]
	public Board Board { get; set; } = new();

	/// <summary>
	/// Gets or sets the selected user ID.
	/// </summary>
	public string SelectedUserId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the selected stage ID.
	/// </summary>
	public string SelectedStageId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the assigned groups.
	/// </summary>
	[ValidateNever]
	public Dictionary<string, List<Group>> AssignedGroups { get; set; } = new();

	/// <summary>
	/// Gets or sets the eligible assignees.
	/// </summary>
	[ValidateNever]
	public List<TicketAppUser> EligibleAssignees { get; set; } = new();

	/// <summary>
	/// Gets or sets the selected group ID.
	/// </summary>
	[ValidateNever]
	public bool CanAssignUser => IsCurrentUserRoleAdmin() || IsCurrentUserProjectLeadForProject() || IsCurrentUserAGroupManagerForStage(SelectedStageId) ||
								 IsCurrentUserAssignedToTicket() || IsTicketUnassigned();


	/// <summary>
	/// Initializes a new instance of the TicketViewModel class.
	/// </summary>
	public TicketViewModel()
	{
		Ticket = new Ticket();
		NewTicket = new Ticket();
		Project = new Project();
		Board = new Board();
	}

	/// <summary>
	/// Checks if the current user is the project lead for the project.
	/// </summary>
	public bool IsCurrentUserProjectLeadForProject()
	{
		return Project.LeadId == CurrentUser.Id;
	}

	/// <summary>
	/// Checks if the current user is a group manager in the stage.
	/// </summary>
	public bool IsCurrentUserAGroupManagerForStage(string stageId)
	{
		foreach (var group in AssignedGroups[stageId])
		{
			if (group.ManagerId == CurrentUser.Id)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the current user is in a group is assigned to the ticket.
	/// </summary>
	public bool IsCurrentUserAssignedToTicket()
	{
		if (Ticket.AssignedTo == CurrentUser.Id)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Checks if the current ticket is unassigned.
	/// </summary>
	public bool IsTicketUnassigned()
	{
		return Ticket.AssignedTo == "Unassigned";
	}

	/// <summary>
	/// Checks if the current user is an admin.
	/// </summary>
	public bool IsCurrentUserRoleAdmin()
	{
		return CurrentUserRole == "Admin";
	}
}
