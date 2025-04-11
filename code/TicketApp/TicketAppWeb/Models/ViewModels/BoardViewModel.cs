using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.ViewModels
{
	/// <summary>
	/// Represents the view model for a board, containing the board and its associated project.
	/// </summary>
	public class BoardViewModel
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
		/// Gets or sets the board associated with the view model.
		/// </summary>
		[ValidateNever]
		public Board Board { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the view model.
		/// </summary>
		[ValidateNever]
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the list of stages associated with the board.
		/// </summary>
		[ValidateNever]
		public ICollection<Stage> Stages { get; set; }

		/// <summary>
		/// Gets or sets the selected group ids.
		/// </summary>
		[Required(ErrorMessage = "Please assign at least one group")]
		public List<string> SelectedGroupIds { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets the list of groups associated with the board.
		/// </summary>
		[ValidateNever]
		public Dictionary<string, List<Group>> AssignedGroups { get; set; }

		/// <summary>
		/// Gets or sets the list of groups available from the project.
		/// </summary>
		[ValidateNever]
		public ICollection<Group> AvailableGroups => Project.Groups
			.Where(g => !AssignedGroups.SelectMany(kvp => kvp.Value).Any(ag => ag.Id == g.Id))
			.ToList();

		/// <summary>
		/// Gets or sets the list of tickets associated with the board.
		/// </summary>
		[ValidateNever]
		public Dictionary<string, List<Ticket>> AssignedTickets { get; set; }

		/// <summary>
		/// Gets or sets the name of the new stage to be added.
		/// </summary>
		[Required(ErrorMessage = "Please enter a stage name")]
		public string NewStageName { get; set; }

		/// <summary>
		/// Gets or sets the selected stage identifier.
		/// </summary>
		public string SelectedStageId { get; set; }

		/// <summary>
		/// Gets or sets the selected group identifier.
		/// </summary>
		public string SelectedGroupId { get; set; }

		/// <summary>
		/// Gets or sets the selected ticket identifier.
		/// </summary>
		public string SelectedTicketId { get; set; }

		/// <summary>
		/// Gets or sets the direction to move the stage (e.g., "left" or "right").
		/// </summary>
		public string SelectedDirection { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BoardViewModel"/> class.
		/// </summary>
		public BoardViewModel()
		{
			Board = new Board();
			Project = new Project();
			Stages = new List<Stage>();
			AssignedGroups = new Dictionary<string, List<Group>>();
			AssignedTickets = new Dictionary<string, List<Ticket>>();
		}

		/// <summary>
		/// Checks if the current user is the project lead for the project.
		/// </summary>
		public bool IsCurrentUserProjectLeadForProject()
		{
			return Project.LeadId == CurrentUser.Id;
		}

		/// <summary>
		/// Checks if the current user is a group manager in the project.
		/// </summary>
		public bool IsCurrentUserAGroupManagerInProject()
		{
			foreach (var group in Project.Groups)
			{
				if (group.ManagerId == CurrentUser.Id)
				{
					return true;
				}
			}

			return false;
		}
	}
}
