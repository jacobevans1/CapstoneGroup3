using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TicketAppWeb.Models.ViewModels;

public class PendingApprovalsViewModel
{
    public List<GroupApprovalRequest> PendingRequests { get; set; } = new List<GroupApprovalRequest>();

}
