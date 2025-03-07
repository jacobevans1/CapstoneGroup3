using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces;

/// <summary>
/// The IProjectRepository interface defines the methods that must be implemented by all Project repository classes.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    /// <summary>
    /// Gets the projects and groups.
    /// </summary>
    Task<Dictionary<Project, List<Group>>> GetFilteredProjectsAndGroups(string? projectName, string? projectLead);
    /// <summary>
    /// Gets all projects asynchronous.
    /// </summary>
    Task<List<Project>> GetAllProjectsAsync();

    /// <summary>
    /// Gets the available groups asynchronous.
    /// </summary>
    Task<List<Group>> GetAvailableGroupsAsync();

    /// <summary>
    /// Gets the project by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    Task<Project?> GetProjectByIdAsync(string id);

    /// <summary>
    /// Gets the group leads asynchronous.
    /// </summary>
    /// <param name="groupIds">The group ids.</param>
    Task<List<TicketAppUser>> GetGroupLeadsAsync(List<string> groupIds);

    /// <summary>
    /// Adds the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    Task AddProjectAsync(Project project, List<string> selectedGroupIds);

    /// <summary>
    /// Updates the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    Task UpdateProjectAsync(Project project, List<string> selectedGroupIds);

    /// <summary>
    /// Deletes the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    Task DeleteProjectAsync(Project project);

    /// <summary>
    /// Gets the project by name and lead asynchronous.
    /// </summary>
    /// <param name="projectName">Name of the project.</param>
    /// <param name="projectLeadId">The project lead identifier.</param>
    Task<Project?> GetProjectByNameAndLeadAsync(string projectName, string projectLeadId);

    /// <summary>
    /// Gets the groups by ids asynchronous.
    /// </summary>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    Task<List<Group>> GetGroupsByIdsAsync(List<string> selectedGroupIds);

    /// <summary>
    /// Adds a group approval request for a project.
    /// </summary>
    Task AddGroupApprovalRequestAsync(string projectId, string groupId);

    /// <summary>
    /// Approves a group for a project.
    /// </summary>
    Task ApproveGroupForProjectAsync(string projectId, string groupId, string managerId);

    /// <summary>
    /// Rejects a group for a project.
    /// </summary>
    Task RejectGroupForProjectAsync(string projectId, string groupId);

    /// <summary>
    /// Gets all pending group approval requests for a project.
    /// </summary>
    Task<List<GroupApprovalRequest>> GetPendingGroupApprovalRequestsAsync(string projectId);
}
