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
    /// <summary>
    /// Adds the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    /// <exception cref="System.Exception">Error adding/updating project: {ex.Message}</exception>
    public async Task AddProjectAsync(Project project, List<string> selectedGroupIds)
    {
        try
        {
            var existingProject = await GetProjectByNameAndLeadAsync(project.ProjectName!, project.LeadId!);

            if (existingProject != null)
            {
                // Update existing project
                existingProject.Description = project.Description;
                existingProject.Groups = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id))
                    .ToListAsync();

                await context.SaveChangesAsync();
                return;
            }

            // If project doesn't exist, create a new one
            project.Groups = await context.Groups
                .Where(g => selectedGroupIds.Contains(g.Id))
                .ToListAsync();

            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding/updating project: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Project not found.</exception>
    /// <exception cref="System.Exception">Error updating project: {ex.Message}</exception>
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
            existingProject.Description = project.Description;
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

    /// <summary>
    /// Deletes the project asynchronous.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <exception cref="System.Exception">Error deleting project: {ex.Message}</exception>
    public async Task DeleteProjectAsync(Project project)
    {
        try
        {
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting project: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the project by name and lead asynchronous.
    /// </summary>
    /// <param name="projectName">Name of the project.</param>
    /// <param name="leadId">The lead identifier.</param>
    public async Task<Project?> GetProjectByNameAndLeadAsync(string projectName, string leadId)
    {
        return await context.Projects
            .Include(p => p.Groups)
            .FirstOrDefaultAsync(p => p.ProjectName == projectName && p.LeadId == leadId);
    }

    /// <summary>
    /// Gets all projects asynchronous.
    /// </summary>
    public Task<List<Project>> GetAllProjectsAsync()
    {
        return context.Projects.Include(p => p.Groups).ToListAsync();
    }

    /// <summary>
    /// Gets the available groups asynchronous.
    /// </summary>
    public Task<List<Group>> GetAvailableGroupsAsync()
    {
        return context.Groups.ToListAsync();
    }

    /// <summary>
    /// Gets the group leads asynchronous.
    /// </summary>
    /// <param name="groupIds">The group ids.</param>
    public Task<List<TicketAppUser>> GetGroupLeadsAsync(List<string> groupIds)
    {
        return context.Groups
            .Where(g => groupIds.Contains(g.Id))
            .Select(g => g.Manager)
            .Distinct()
            .ToListAsync()!;
    }

    /// <summary>
    /// Gets the project by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public Task<Project?> GetProjectByIdAsync(string id)
    {
        return context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Gets the projects their assigned groups.
    /// </summary>
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

    /// <summary>
    /// Sets the project leads.
    /// </summary>
    /// <param name="projects">The projects.</param>
    private List<Project> setProjectLeads(List<Project> projects)
	{
		foreach (var project in projects)
		{
			project.Lead = context.Users.Find(project.LeadId);
		}

		return projects;
	}

    /// <summary>
    /// Gets the groups by ids asynchronous.
    /// </summary>
    /// <param name="selectedGroupIds">The selected group ids.</param>
    public async Task<List<Group>> GetGroupsByIdsAsync(List<string> selectedGroupIds)
    {
        if (selectedGroupIds == null || !selectedGroupIds.Any())
            return new List<Group>();

        return await context.Groups
            .Where(g => selectedGroupIds.Contains(g.Id))
            .ToListAsync();
    }
}