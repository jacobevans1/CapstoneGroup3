using TicketAppWeb.Models.DomainModels;
namespace TicketAppWeb.Models.DataLayer.Reposetories;

/// <summary>
/// The project repository class
/// Jabesi Abwe
/// 02/19/2025
/// </summary>

public class ProjectRepository(TicketAppContext ctx) : Repository<Project>(ctx), IProjectRepository
{
    public void AddNewProjectGroups(Project? project, string[] groupIds, IRepository<Group> groupData)
    {

        project?.Groups.Clear();

        // then add new groups
        foreach (string id in groupIds)
        {
            Group? group = groupData.Get(id);
            if (group != null)
                project?.Groups.Add(group);
        }
    }
}