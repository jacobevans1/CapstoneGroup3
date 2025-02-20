using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models.DomainModels;

public class Group
{
    public Group()
    {
        Members = new HashSet<TicketAppUser>();
        Projects = new HashSet<Project>();
    }

    public string? Id { get; set; }

    [Required(ErrorMessage = "Please enter a group name")]
    public string? GroupName { get; set; }

    [Required(ErrorMessage = "Please enter a group description")]
    public string? Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a group lead")]
    public string? ManagerId { get; set; }

    [ValidateNever]
    public TicketAppUser? Manager { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<TicketAppUser> Members { get; set; } 

    public ICollection<Project> Projects { get; set; }
}
