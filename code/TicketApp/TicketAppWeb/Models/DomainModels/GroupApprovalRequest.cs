namespace TicketAppWeb.Models.DomainModels;

/// <summary>
/// The class represent groups that are pending to be added to a project needs menagers approval
/// Jabesi Abwe
/// 03/?/2025
/// </summary>
public class GroupApprovalRequest
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the project identifier.
    /// </summary>
    public string? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string? Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the project.
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    public Group? Group { get; set; }
}
