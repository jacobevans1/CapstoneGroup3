using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAppWeb.Models.DomainModels;

/// <summary>
/// The class represent a group entity
/// Emma
/// 02/?/2025
/// </summary>
public class Group
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Group"/> class.
	/// </summary>
	public Group()
    {
        Id = Guid.NewGuid().ToString();
        Members = new HashSet<TicketAppUser>();
        Projects = new HashSet<Project>();
        CreatedAt = DateTime.UtcNow;
    }

	/// <summary>
	/// Gets or sets the group identifier.
	/// </summary>
	[Key]
    public string Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the group.
	/// </summary>
	[Required(ErrorMessage = "Please enter a group name")]
    [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
    public string GroupName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	[Required(ErrorMessage = "Please enter a group description")]
    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the manager identifier.
	/// </summary>
	[Required(ErrorMessage = "Please select a group manager")]
    public string? ManagerId { get; set; }

	/// <summary>
	/// Gets or sets the manager.
	/// </summary>
	[ForeignKey("ManagerId")]
    [ValidateNever]
    public virtual TicketAppUser? Manager { get; set; }

	/// <summary>
	/// Gets the date and time the group was created at.
	/// </summary>
	public DateTime CreatedAt { get; private set; }

	/// <summary>
	/// Gets or sets the members of the group
	/// </summary>
	public virtual ICollection<TicketAppUser> Members { get; set; }

	/// <summary>
	/// Gets or sets the projects that the group is on
	/// </summary>
	public virtual ICollection<Project> Projects { get; set; }
}
