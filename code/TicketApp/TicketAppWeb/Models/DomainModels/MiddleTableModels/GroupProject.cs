// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	/// <summary>
	/// Represents the association between a group and a project.
	/// </summary>
	public class GroupProject
	{
		/// <summary>
		/// Gets or sets the project identifier.
		/// </summary>
		public string ProjectsId { get; set; }

		/// <summary>
		/// Gets or sets the group identifier.
		/// </summary>
		public string GroupsId { get; set; }

		/// <summary>
		/// Gets or sets the project associated with the group.
		/// </summary>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the group associated with the project.
		/// </summary>
		public Group Group { get; set; }
	}
}
