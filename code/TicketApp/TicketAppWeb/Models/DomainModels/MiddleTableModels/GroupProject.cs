// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels.MiddleTableModels
{
	/// <summary>
	/// The GroupProject class represents the many-to-many relationship between groups and projects.
	/// </summary>
	public class GroupProject
	{
		public string ProjectsId { get; set; }
		public string GroupsId { get; set; }

		public Project Project { get; set; }
		public Group Group { get; set; }
	}
}
