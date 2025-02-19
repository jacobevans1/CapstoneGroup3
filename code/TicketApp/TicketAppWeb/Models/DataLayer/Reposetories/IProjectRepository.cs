using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Reposetories;

/// <summary>
/// The IProjectRepository interface defines the methods that must be implemented by all Project repository classes.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    // Adds the new project groups.
    void AddNewProjectGroups(Project? project, int[] groupIds, IRepository<Group> groupData);
}
