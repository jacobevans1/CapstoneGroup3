namespace TicketAppDesktop.Models;

public class Project
{
    /// <summary>
	/// Initializes a new instance of the <see cref="Project"/> class.
	/// </summary>
	public Project()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        Groups = new HashSet<Group>();
    }

    /// <summary>
    /// Gets or sets the project identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the lead identifier.
    /// </summary>
    public string? LeadId { get; set; }

    /// <summary>
    /// Gets or sets the lead.
    /// </summary>
    public TicketAppUser? Lead { get; set; }

    /// <summary>
    /// Gets or sets the identifier for who created the project
    /// </summary>
    public string? CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the user who created the project.
    /// </summary>
    public TicketAppUser? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time the project was created at.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the groups assigned to this project.
    /// </summary>
    public ICollection<Group> Groups { get; set; }

}
