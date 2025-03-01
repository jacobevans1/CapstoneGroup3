using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces;

/// <summary>
/// The IProjectRepository interface defines the methods that must be implemented by all Project repository classes.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
	Task<Dictionary<Project, List<Group>>> GetProjectsAndGroups();

    Task<List<Project>> GetAllProjectsAsync();
    Task<List<Group>> GetAvailableGroupsAsync();
    Task<Project?> GetProjectByIdAsync(string id);
    Task<List<TicketAppUser>> GetGroupLeadsAsync(List<string> groupIds);
    Task AddProjectAsync(Project project, List<string> selectedGroupIds);
    Task UpdateProjectAsync(Project project, List<string> selectedGroupIds);
    Task DeleteProjectAsync(Project project);
    Task<Project?> GetProjectByNameAndLeadAsync(string projectName, string projectLeadId);
    Task<List<Group>> GetGroupsByIdsAsync(List<string> selectedGroupIds);
}
