using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels
{
	/// <summary>
	/// Represents a stage in a board.
	/// </summary>
	public class Stage
	{
		/// <summary>
		/// Gets or sets the stage identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the stage.
		/// </summary>
		[Required(ErrorMessage = "Please provide a stage name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the stage.
		/// </summary>
		public ICollection<BoardStage> BoardStages { get; set; } = new List<BoardStage>();
	}
}
