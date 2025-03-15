using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels;

public class Group
{
	// Constructor
	public Group()
	{
		Id = Guid.NewGuid().ToString(); // Auto-generate ID if not provided
		Members = new HashSet<TicketAppUser>();
		Projects = new HashSet<Project>();
		CreatedAt = DateTime.UtcNow; // Set creation time on object initialization
	}

	// Group Id
	[Key]
	public string Id { get; set; }

	// Group name
	[Required(ErrorMessage = "Please enter a group name")]
	[StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
	public string GroupName { get; set; } = string.Empty;

	// Group description
	[Required(ErrorMessage = "Please enter a group description")]
	[StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
	public string Description { get; set; } = string.Empty;

	// Group manager ID
	[Required(ErrorMessage = "Please select a group manager")]
	public string ManagerId { get; set; }

	// Navigation property for the manager (linked to `AspNetUsers`)
	[ForeignKey("ManagerId")]
	[ValidateNever]
	public virtual TicketAppUser? Manager { get; set; }

	// Date when the group was created
	public DateTime CreatedAt { get; private set; }

	// Members of the group
	public virtual ICollection<TicketAppUser> Members { get; set; }

	// Projects linked to the group
	public virtual ICollection<Project> Projects { get; set; }
}
