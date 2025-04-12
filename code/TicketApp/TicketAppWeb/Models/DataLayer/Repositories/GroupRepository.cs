using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories;

/// <summary>
/// The group repository manages access to the database for anything related to group management
/// Emma
/// 03/?/2025
/// </summary>
public class GroupRepository(TicketAppContext ctx) : Repository<Group>(ctx), IGroupRepository
{
	/// <summary>
	/// Gets all gorups asynchronous.
	/// </summary>
	public async Task<IEnumerable<Group>> GetAllAsync()
    {
        return await context.Groups
            .Include(g => g.Members)
            .Include(g => g.Manager) 
            .ToListAsync();
    }

	/// <summary>
	/// Gets the group by Id asynchronous.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <returns></returns>
	public async Task<Group?> GetAsync(string id)
    {
        return await context.Groups
            .Include(g => g.Members)
            .Include(g => g.Manager)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

	/// <summary>
	/// Adds new group members.
	/// </summary>
	/// <param name="group">The group.</param>
	/// <param name="userIds">The user ids.</param>
	/// <param name="memberData">The member data.</param>
	public void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData)
    {
        if (group == null) return;

        var currentMemberIds = group.Members.Select(m => m.Id).ToList();

        foreach (string id in userIds)
        {
            if (!currentMemberIds.Contains(id))
            {
                var member = memberData.Get(id);
                if (member != null)
                {
                    group.Members.Add(member);
                }
            }
        }
    }

	/// <summary>
	/// Inserts the group in the database asynchronous.
	/// </summary>
	/// <param name="group">The group.</param>
	public async Task InsertAsync(Group group)
    {
        await context.Groups.AddAsync(group);
    }

	/// <summary>
	/// Saves the changes to the database asynchronous.
	/// </summary>
	public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }

	/// <summary>
	/// Deletes the group asynchronous.
	/// </summary>
	/// <param name="group">The group.</param>
	public async Task DeleteGroupAsync(Group group)
    {
        if (group == null) return;

        var projectsWithGroup = await context.Projects
            .Where(p => p.Groups.Any(g => g.Id == group.Id))
            .ToListAsync();

        foreach (var project in projectsWithGroup)
        {
            project.Groups.Remove(group);
        }

        await context.SaveChangesAsync();

        context.Groups.Remove(group);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the group by manager identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<List<Group>> GetGroupByManagerIdAsync(string id)
    {
        return await context.Groups
            .Where(g => g.ManagerId == id)
            .Include(p => p.Members)
            .ToListAsync();
    }
}