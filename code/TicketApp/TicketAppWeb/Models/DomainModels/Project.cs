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
    public Project()
    {
        groups = new HashSet<Group>();
    }

    public string? ProjectId { get; set; }

    [Required(ErrorMessage = "Please enter a project name")]
    public string? ProjectName { get; set; }

    [Required(ErrorMessage = "Please enter a project description")]
    public string? ProjectDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a project lead")]
    public string? ProjectLeadId { get; set; }

    [ValidateNever]
    public TicketAppUser? ProjectLead { get; set; }

    public string? CreatedByid { get; set; }

    [ValidateNever]
    public TicketAppUser? CreatedBy { get; set; }

    public DateTime CreatedOnDate { get; set; } = DateTime.Now;

    public ICollection<Group> groups { get; set; }
}
