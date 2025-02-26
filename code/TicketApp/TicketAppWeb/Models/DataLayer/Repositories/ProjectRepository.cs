using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
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

	public async Task<Dictionary<Project, List<Group>>> GetProjectsAndGroups()
	{
		var projects = context.Projects.ToList();
		setProjectLeads(projects);

		var projectsGroups = new Dictionary<Project, List<Group>>();

		foreach (var project in projects)
		{
			var groups = await context.Groups
				.FromSqlRaw(@"SELECT g.*
                          FROM Groups g
                          JOIN ProjectGroups pg ON g.Id = pg.GroupId
                          WHERE pg.ProjectId = {0}", project.Id)
				.ToListAsync();

			projectsGroups.Add(project, groups);
		}

		return projectsGroups;
	}

	private List<Project> setProjectLeads(List<Project> projects)
	{
		foreach (var project in projects)
		{
			project.Lead = context.Users.Find(project.LeadId);
		}

		return projects;
	}
}