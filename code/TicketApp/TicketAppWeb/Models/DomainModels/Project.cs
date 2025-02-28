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
    // Constructor
    public Project()
    {
        Groups = new HashSet<Group>();
    }

    // Project Id
    public string? Id { get; set; }

    // Project name
    [Required(ErrorMessage = "Please enter a project name")]
    public string? ProjectName { get; set; }

    // Project description
    public string? Description { get; set; } = string.Empty;

    // Project Lead Id
    [Required(ErrorMessage = "Please select a project lead")]
    public string? LeadId { get; set; }

    // Project lead
    [ValidateNever]
    public TicketAppUser? Lead { get; set; }

    // The Id of the user who created the project
    public string? CreatedById { get; set; }

    // The user who created the project
    [ValidateNever]
    public TicketAppUser? CreatedBy { get; set; } 

    // Date the project was created
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Please assign at least one group to the project")]
    // List of project groups
    public ICollection<Group> Groups { get; set; }
}
