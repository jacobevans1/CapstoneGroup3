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

            // Check if the current user is adding groups they don't manage and need approval
            var userId = project.CreatedById;
            var groupsToApprove = new List<Group>();

            foreach (var groupId in selectedGroupIds)
            {
                var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group != null && group.ManagerId != userId)
                {
                    // If the group is not managed by the user, initiate approval
                    groupsToApprove.Add(group);
                    // Add the approval request
                    await AddGroupApprovalRequestAsync(project.Id!, group.Id);
                }
            }

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

            var userId = project.CreatedById;
            var groupsToApprove = new List<Group>();

            // Check if any group has changed and requires approval
            foreach (var groupId in selectedGroupIds)
            {
                var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group != null && group.ManagerId != userId)
                {
                    // If the group is not managed by the user, initiate approval
                    groupsToApprove.Add(group);
                    // Add the approval request
                    await AddGroupApprovalRequestAsync(project.Id!, group.Id);
                }
            }

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
    public async Task<Dictionary<Project, List<Group>>> GetFilteredProjectsAndGroups(string? projectName, string? projectLead)
    {
        var query = context.Projects.AsQueryable();

        if (!string.IsNullOrEmpty(projectName))
        {
            query = query.Where(p => p.ProjectName!.Contains(projectName));
        }

        if (!string.IsNullOrEmpty(projectLead))
        {
            query = query.Where(p => p.Lead != null && p.Lead.FullName.Contains(projectLead));
        }

        var projects = await query.ToListAsync();
        setProjectLeads(projects);

        var projectsGroups = new Dictionary<Project, List<Group>>();
        foreach (var project in projects)
        {
            var groups = await context.Groups
                .Where(g => g.Projects.Any(p => p.Id == project.Id))
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

    public async Task AddGroupApprovalRequestAsync(string projectId, string groupId)
    {
        try
        {
            var request = new GroupApprovalRequest
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                GroupId = groupId,
                Status = "Pending"
            };

            context.GroupApprovalRequests.Add(request);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding group approval request: {ex.Message}");
        }
    }

    public async Task ApproveGroupForProjectAsync(string projectId, string groupId, string managerId)
    {
        try
        {
            var request = await context.GroupApprovalRequests
                .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.GroupId == groupId && r.Status == "Pending");

            if (request == null)
            {
                throw new Exception("No pending approval request found.");
            }

            request.Status = "Approved";

            // Add the group to the project
            var project = await context.Projects
                .Include(p => p.Groups)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null)
            {
                var group = await context.Groups.FindAsync(groupId);
                if (group != null)
                {
                    project.Groups.Add(group);
                }
            }

            context.GroupApprovalRequests.Update(request);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error approving group for project: {ex.Message}");
        }
    }

    public async Task RejectGroupForProjectAsync(string projectId, string groupId)
    {
        try
        {
            var request = await context.GroupApprovalRequests
                .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.GroupId == groupId && r.Status == "Pending");

            if (request == null)
            {
                throw new Exception("No pending approval request found.");
            }

            request.Status = "Rejected";

            context.GroupApprovalRequests.Update(request);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error rejecting group for project: {ex.Message}");
        }
    }

    public async Task<List<GroupApprovalRequest>> GetPendingGroupApprovalRequestsAsync(string projectId)
    {
        return await context.GroupApprovalRequests
            .Where(r => r.ProjectId == projectId && r.Status == "Pending")
            .ToListAsync();
    }
}