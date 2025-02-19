using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models.DomainModels;

public class Group
{
    public Group()
    {
        members = new HashSet<TicketAppUser>();
    }

    public string? GroupId { get; set; }

    [Required(ErrorMessage = "Please enter a gorup name")]
    public string? GroupName { get; set; }

    [Required(ErrorMessage = "Please enter a gorup description")]
    public string? GroupDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a gorup lead")]
    public string? GroupLeadId { get; set; }

    [ValidateNever]
    public TicketAppUser? GroupLead { get; set; }

    public DateTime CreatedOnDate { get; set; } = DateTime.Now;

    public ICollection<TicketAppUser> members { get; set; }
}

