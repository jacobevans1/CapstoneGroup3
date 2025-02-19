using TicketAppWeb.Models.DomainModels;
namespace TicketAppWeb.Models.DataLayer.Reposetories;

/// <summary>
/// The project repository class
/// Jabesi Abwe
/// 02/19/2025
/// </summary>

public class ProjectRepository(TicketAppContext ctx) : Repository<Project>(ctx), IProjectRepository
{
    public void AddNewProjectGroups(Project? project, int[] groupIds, IRepository<Group> groupData)
    {
        // first remove any current groups
        foreach (Group group in project.groups)
        {
            project.groups.Remove(group);
        }

        // then add new groups
        foreach (int id in groupIds)
        {
            Group? group = groupData.Get(id);
            if (group != null)
                project.groups.Add(group);
        }
    }
}