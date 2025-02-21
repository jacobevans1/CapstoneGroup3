using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models.DomainModels;

public class Group
{
    // Constructor
    public Group()
    {
        Members = new HashSet<TicketAppUser>();
        Projects = new HashSet<Project>();
    }

    // Group Id
    public string? Id { get; set; }

    // Group name
    [Required(ErrorMessage = "Please enter a group name")]
    public string? GroupName { get; set; }

    // Group description
    [Required(ErrorMessage = "Please enter a group description")]
    public string? Description { get; set; } = string.Empty;

    // Group lead Id
    [Required(ErrorMessage = "Please select a group lead")]
    public string? ManagerId { get; set; }

    // Group lead
    [ValidateNever]
    public TicketAppUser? Manager { get; set; }

    // Date when the group was created
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Members of the group
    public ICollection<TicketAppUser> Members { get; set; } 

    // Projects of the goup
    public ICollection<Project> Projects { get; set; }
}
