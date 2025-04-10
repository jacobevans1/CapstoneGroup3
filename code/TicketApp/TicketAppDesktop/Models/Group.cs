namespace TicketAppDesktop.Models;

public class Group
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Group"/> class.
    /// </summary>
    public Group()
    {
        Id = Guid.NewGuid().ToString();
        Members = new HashSet<TicketAppUser>();
        Projects = new HashSet<Project>();
        CreatedAt = DateTime.UtcNow;
    }

    public Group(string id, string groupName, string description, string managerId, DateTime createdAt) : this()
    {
        Id = id;
        GroupName = groupName;
        Description = description;
        ManagerId = managerId;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the manager identifier.
    /// </summary>
    public string? ManagerId { get; set; }

    /// <summary>
    /// Gets or sets the manager.
    /// </summary>
    public virtual TicketAppUser? Manager { get; set; }

    /// <summary>
    /// Gets the date and time the group was created at.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the members of the group
    /// </summary>
    public virtual ICollection<TicketAppUser> Members { get; set; }

    /// <summary>
    /// Gets or sets the projects that the group is on
    /// </summary>
    public virtual ICollection<Project> Projects { get; set; }

}
