using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories;

/// <summary>
/// The project repository class
/// Jabesi Abwe
/// 02/19/2025
/// </summary>

public class ProjectRepository(TicketAppContext ctx) : Repository<Project>(ctx), IProjectRepository
{
	public void AddNewProjectGroups(Project? project, string[] groupIds, IRepository<Group> groupData)
	{
		// first remove any current groups
		foreach (Group group in project.Groups)
		{
			project.Groups.Remove(group);
		}

		// then add new groups
		foreach (string id in groupIds)
		{
			Group? group = groupData.Get(id);
			if (group != null)
				project.Groups.Add(group);
		}
	}
}