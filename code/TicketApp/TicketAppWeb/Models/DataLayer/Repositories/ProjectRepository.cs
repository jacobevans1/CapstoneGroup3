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
    public async Task AddProjectAsync(Project project, List<string> selectedGroupIds)
    {
        try
        {
            // Check if project with the same name and lead already exists
            var existingProject = await GetProjectByNameAndLeadAsync(project.ProjectName!, project.LeadId!);

            if (existingProject != null)
            {
                throw new InvalidOperationException("A project with the same name and lead already exists.");
            }

            var selectedGroups = await context.Groups
                .Where(g => selectedGroupIds.Contains(g.Id))
                .ToListAsync();

            project.Groups = selectedGroups;

            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding project: {ex.Message}");
        }
    }

    public async Task<Project?> GetProjectByNameAndLeadAsync(string projectName, string leadId)
    {
        return await context.Projects
            .Include(p => p.Groups)
            .FirstOrDefaultAsync(p => p.ProjectName == projectName && p.LeadId == leadId);
    }

    public Task DeleteProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task<List<Project>> GetAllProjectsAsync()
    {
        return context.Projects.Include(p => p.Groups).ToListAsync();
    }

    public Task<List<Group>> GetAvailableGroupsAsync()
    {
        return context.Groups.ToListAsync();
    }

    public Task<List<TicketAppUser>> GetGroupLeadsAsync(List<string> groupIds)
    {
        return context.Groups
            .Where(g => groupIds.Contains(g.Id))
            .Select(g => g.Manager)
            .Distinct()
            .ToListAsync()!;
    }

    public Task<Project?> GetProjectByIdAsync(string id)
    {
        return context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == id);
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
                          JOIN GroupProject pg ON g.Id = pg.GroupsId
                          WHERE pg.ProjectsId = {0}", project.Id)
                .ToListAsync();

            projectsGroups.Add(project, groups);
        }

        return projectsGroups;
    }

    public async Task UpdateProjectAsync(Project project, List<string> selectedGroupIds)
    {
        try
        {
            var existingProject = await context.Projects
                .Include(p => p.Groups)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (existingProject == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            existingProject.ProjectName = project.ProjectName;
            existingProject.LeadId = project.LeadId;
            existingProject.Groups = await context.Groups
                .Where(g => selectedGroupIds.Contains(g.Id))
                .ToListAsync();

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating project: {ex.Message}");
        }
    }

    private List<Project> setProjectLeads(List<Project> projects)
	{
		foreach (var project in projects)
		{
			project.Lead = context.Users.Find(project.LeadId);
		}

		return projects;
	}
    public async Task<List<Group>> GetGroupsByIdsAsync(List<string> selectedGroupIds)
    {
        if (selectedGroupIds == null || !selectedGroupIds.Any())
            return new List<Group>();

        return await context.Groups
            .Where(g => selectedGroupIds.Contains(g.Id))
            .ToListAsync();
    }
}