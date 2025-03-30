using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
		public Board Board { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the view model.
		/// </summary>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the list of statuses associated with the board.
		/// </summary>
		public ICollection<Status> Statuses { get; set; }

		/// <summary>
		/// Gets or sets the list of groups associated with the board.
		/// </summary>
		public Dictionary<string, string> AssignedGroups { get; set; }

		/// <summary>
		/// Gets or sets the list of groups associated with the view model.
		/// </summary>
		public string NewStatusName { get; set; }

		/// <summary>
		/// Gets or sets the selected status identifier.
		/// </summary>
		public string SelectedStatusId { get; set; }

		/// <summary>
		/// Gets or sets the selected group identifier.
		/// </summary>
		public string SelectedGroupId { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BoardViewModel"/> class.
		/// </summary>
		public BoardViewModel()
		{
			Board = new Board();
			Project = new Project();
		}
	}
}
