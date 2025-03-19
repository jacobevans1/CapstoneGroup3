using Microsoft.AspNetCore.Identity;
namespace TicketAppWeb.Models.DomainModels;

/// <summary>
/// The TicketAppUser class represents a user of the TicketApp application.
/// </summary>
public class TicketAppUser : IdentityUser
{

	/// <summary>
	/// Initializes a new instance of the Ticket App User class.
	/// </summary>
	/// <remarks>
	/// The Id property is initialized to form a new GUID string value.
	/// </remarks>
	public TicketAppUser()
    {
        Groups = new HashSet<Group>();
    }

	/// <summary>
	/// Gets or sets the first name.
	/// </summary>
	public string FirstName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the last name.
	/// </summary>
	public string LastName { get; set; } = string.Empty;

	/// <summary>
	/// Gets the full name.
	/// </summary>
	public string FullName => $"{FirstName} {LastName}";

	/// <summary>
	/// Gets or sets the groups.
	/// </summary>
	public virtual ICollection<Group> Groups { get; set; }
}