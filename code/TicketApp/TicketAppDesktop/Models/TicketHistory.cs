namespace TicketAppDesktop.Models;

public class TicketHistory
{
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public string? Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	/// Gets or sets the ticket identifier.
	/// </summary>
	/// <value>
	/// The ticket identifier.
	/// </value>
	public string? TicketId { get; set; }

	/// <summary>
	/// Gets or sets the property changed.
	/// </summary>
	/// <value>
	/// The property changed.
	/// </value>
	public string? PropertyChanged { get; set; }

	/// <summary>
	/// Gets or sets the old value.
	/// </summary>
	/// <value>
	/// The old value.
	/// </value>
	public string? OldValue { get; set; }

	/// <summary>
	/// Creates new value.
	/// </summary>
	/// <value>
	/// The new value.
	/// </value>
	public string? NewValue { get; set; }

	/// <summary>
	/// Gets or sets the changed by user identifier.
	/// </summary>
	/// <value>
	/// The changed by user identifier.
	/// </value>
	public string? ChangedByUserId { get; set; }

	/// <summary>
	/// Gets or sets the changed by user.
	/// </summary>
	/// <value>
	/// The changed by user.
	/// </value>
	public TicketAppUser? ChangedByUser { get; set; }

	/// <summary>
	/// Gets or sets the change date.
	/// </summary>
	/// <value>
	/// The change date.
	/// </value>
	public DateTime ChangeDate { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the change description.
	/// </summary>
	/// <value>
	/// The change description.
	/// </value>
	public string? ChangeDescription { get; set; }
}