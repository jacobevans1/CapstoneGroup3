using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.ViewModels
{
    public class DeleteGroupViewModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public List<Project> AffectedProjects { get; set; } = new List<Project>();
        public string? NewLeadId { get; set; }
        public List<TicketAppUser> AvailableUsers { get; set; } = new List<TicketAppUser>();
    }

}
