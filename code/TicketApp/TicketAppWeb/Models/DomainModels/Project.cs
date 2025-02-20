using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models.DomainModels;

/// <summary>
/// Represents a project
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class Project
{
    public Project()
    {
        Groups = new HashSet<Group>();
    }

    public string? Id { get; set; }

    [Required(ErrorMessage = "Please enter a project name")]
    public string? ProjectName { get; set; }

    [Required(ErrorMessage = "Please enter a project description")]
    public string? Description { get; set; } = string.Empty;

    public string? LeadId { get; set; }

    [ValidateNever]
    public TicketAppUser? Lead { get; set; }

    public string? CreatedById { get; set; }

    [ValidateNever]
    public TicketAppUser? CreatedBy { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Group> Groups { get; set; }
}
