using System.ComponentModel.DataAnnotations;

namespace TicketAppDesktop.Models;

public class Stage
{
	/// <summary>
	/// Gets or sets the stage identifier.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the stage.
	/// </summary>
	public string? Name { get; set; }
}