using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.ViewModels
{
    public class AddGroupViewModel
    {
        [Required(ErrorMessage = "Please enter a group name.")]
        public string GroupName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a group description.")]
        public string Description { get; set; } = string.Empty;

        public List<TicketAppUser> AllUsers { get; set; } = new List<TicketAppUser>();

        [Required(ErrorMessage = "Please select at least one member.")]
        public List<string> SelectedUserIds { get; set; } = new List<string>();

        [Required(ErrorMessage = "A group lead is required.")]
        public string GroupLeadId { get; set; }
    }
}
