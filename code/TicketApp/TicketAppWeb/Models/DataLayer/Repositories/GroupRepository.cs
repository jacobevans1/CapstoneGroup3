using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	public class GroupRepository(TicketAppContext ctx) : Repository<Group>(ctx), IGroupRepository
	{
        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await context.Groups
                .Include(g => g.Members)  // Ensure members are loaded
                .Include(g => g.Manager)  // Include manager info
                .ToListAsync();
        }

        public async Task<Group?> GetAsync(string id)
        {
            return await context.Groups
                .Include(g => g.Members)
                .Include(g => g.Manager)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData)
        {
            if (group == null) return;

            // Get current members' IDs
            var currentMemberIds = group.Members.Select(m => m.Id).ToList();

            // Add only new users that are not already in the group
            foreach (string id in userIds)
            {
                if (!currentMemberIds.Contains(id)) // Prevent duplicate members
                {
                    TicketAppUser? member = memberData.Get(id);
                    if (member != null)
                    {
                        group.Members.Add(member);
                    }
                }
            }
        }

        public async Task InsertAsync(Group group)
        {
            await context.Groups.AddAsync(group);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(Group group)
        {
            if (group == null) return;

            // Step 1: Remove group from projects (junction table)
            var projectsWithGroup = await context.Projects
                .Where(p => p.Groups.Any(g => g.Id == group.Id))
                .ToListAsync();

            foreach (var project in projectsWithGroup)
            {
                project.Groups.Remove(group);
            }

            await context.SaveChangesAsync();

            // Step 2: Remove the group itself
            context.Groups.Remove(group);
            await context.SaveChangesAsync();
        }


    }
}