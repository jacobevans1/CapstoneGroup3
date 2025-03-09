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
    public async Task AddProjectAsync(Project project, List<string> selectedGroupIds, bool isAdmin)
    {
        try
        {
            // Check if the project already exists
            var existingProject = await GetProjectByNameAndLeadAsync(project.ProjectName!, project.LeadId!);
            var userId = project.CreatedById;

            if (existingProject != null)
            {
                existingProject.Description = project.Description;
                existingProject.LeadId = project.LeadId;

                var groupsToAdd = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id!))
                    .ToListAsync();

                foreach (var group in groupsToAdd)
                {
                    if (!existingProject.Groups.Contains(group))
                    {
                        existingProject.Groups.Add(group);
                    }
                }

                foreach (var group in existingProject.Groups.ToList())
                {
                    if (!selectedGroupIds.Contains(group.Id!))
                    {
                        existingProject.Groups.Remove(group);
                    }
                }

                await context.SaveChangesAsync();
                return;
            }

            List<Group> groupsToAddDirectly;
            List<string> groupsNeedingApproval = new List<string>();

            if (isAdmin)
            {
                groupsToAddDirectly = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id!))
                    .ToListAsync();
            }
            else
            {
                groupsToAddDirectly = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id!) && g.ManagerId == userId)
                    .ToListAsync();

				groupsNeedingApproval = selectedGroupIds
					.Where(gId => !groupsToAddDirectly.Any(g => g.Id == gId))
					.ToList();
			}

			project.Groups = groupsToAddDirectly;
			context.Projects.Add(project);
			await context.SaveChangesAsync();

			foreach (var groupId in groupsNeedingApproval)
			{
				await AddGroupApprovalRequestAsync(project.Id!, groupId);
			}

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
    /// <param name="isAdmin"></param>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Project not found.</exception>
    /// <exception cref="System.Exception">Error updating project: {ex.Message}</exception>
    public async Task UpdateProjectAsync(Project project, List<string> selectedGroupIds, bool isAdmin)
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
            List<Group> groupsToAddDirectly;
            List<string> groupsNeedingApproval = new List<string>();

            if (isAdmin)
            {
                groupsToAddDirectly = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id!))
                    .ToListAsync();
            }
            else
            {
                groupsToAddDirectly = await context.Groups
                    .Where(g => selectedGroupIds.Contains(g.Id!) && g.ManagerId == userId)
                    .ToListAsync();

                groupsNeedingApproval = selectedGroupIds
                    .Where(gId => !groupsToAddDirectly.Any(g => g.Id == gId))
                    .ToList();
            }

            foreach (var group in groupsToAddDirectly)
            {
                if (!existingProject.Groups.Contains(group))
                {
                    existingProject.Groups.Add(group);
                }
            }

            foreach (var group in existingProject.Groups.ToList())
            {
                if (!selectedGroupIds.Contains(group.Id!))
                {
                    existingProject.Groups.Remove(group);
                }
            }

            await context.SaveChangesAsync();

            foreach (var groupId in groupsNeedingApproval)
            {
                await AddGroupApprovalRequestAsync(project.Id!, groupId);
            }

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
            .Where(g => groupIds.Contains(g.Id!))
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

        var projects = await query.ToListAsync();
        setProjectLeads(projects);

        if (!string.IsNullOrEmpty(projectLead))
        {
            projects = projects.Where(p => p.Lead!.FullName.Contains(projectLead))
            .ToList();

        }

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
    /// Adds a group approval request for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="groupId"></param>
    /// <exception cref="System.Exception">Error adding group approval request: {ex.Message}</exception>
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


    /// <summary>
    /// Approves a group for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="groupId"></param>
    /// <exception cref="System.Exception">
    /// No pending approval request found.
    /// or
    /// Error approving group for project: {ex.Message}
    /// </exception>
    public async Task ApproveGroupForProjectAsync(string projectId, string groupId)
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

    /// <summary>
    /// Rejects a group for a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="groupId"></param>
    /// <exception cref="System.Exception">
    /// No pending approval request found.
    /// or
    /// Error rejecting group for project: {ex.Message}
    /// </exception>
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

    /// <summary>
    /// Gets the pending group approval requests asynchronous.
    /// </summary>
    /// <param name="managerId">The manager identifier.</param>
    /// <returns></returns>
    public async Task<List<GroupApprovalRequest>> GetPendingGroupApprovalRequestsAsync(string managerId)
    {
        var managedGroupIds = await context.Groups
            .Where(g => g.ManagerId == managerId)
            .Select(g => g.Id)
            .ToListAsync();

        return await context.GroupApprovalRequests
            .Where(r => managedGroupIds.Contains(r.GroupId!) && r.Status == "Pending")
            .Include(r => r.Project)
            .Include(r => r.Group)
            .ToListAsync();
    }
}