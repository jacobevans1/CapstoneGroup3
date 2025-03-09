using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.ViewModels;

public class PendingApprovalsViewModel
{
    public List<GroupApprovalRequest> PendingRequests { get; set; } = new List<GroupApprovalRequest>();

}
