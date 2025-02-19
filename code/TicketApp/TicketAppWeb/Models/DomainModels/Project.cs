using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models.DomainModels;

/// <summary>
/// Represent a project
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class Project
{
    // Construct a new project object
    public Project()
    {
        groups = new HashSet<Group>();
    }

    // Gets or sets the project identifier.
    public string? ProjectId { get; set; }

    // Gets or sets the name of the project.
    [Required(ErrorMessage = "Please enter a project name")]
    public string? ProjectName { get; set; }

    // Gets or sets the project description.
    [Required(ErrorMessage = "Please enter a project description")]
    public string? ProjectDescription { get; set; } = string.Empty;

    // Gets or sets the project lead identifier.
    [Required(ErrorMessage = "Please select a project lead")]
    public string? ProjectLeadId { get; set; }

    // Gets or sets the project lead.
    [ValidateNever]
    public TicketAppUser? ProjectLead { get; set; }

    // Gets or sets the created byid.
    public string? CreatedByid { get; set; }

    // Gets or sets the created by.
    [ValidateNever]
    public TicketAppUser? CreatedBy { get; set; }

    // Gets or sets the date the project was created on.
    public DateTime CreatedOnDate { get; set; } = DateTime.Now;

    // Gets or sets the list of groups on the project.
    public ICollection<Group> groups { get; set; }
}
