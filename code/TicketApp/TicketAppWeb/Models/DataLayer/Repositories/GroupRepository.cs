using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	public class GroupRepository(TicketAppContext ctx) : Repository<Group>(ctx), IGroupRepository
	{
		public async Task<IEnumerable<Group>> GetAllGroups()
		{
			return await context.Groups
				.Include(g => g.Members)
				.Include(g => g.Manager)
				.ToListAsync();
		}

		public async Task<Group?> GetGroupById(string id)
		{
			return await context.Groups
				.Include(g => g.Members)
				.Include(g => g.Manager)
				.FirstOrDefaultAsync(g => g.Id == id);
		}

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

		public async Task InsertGroup(Group group)
		{
			await context.Groups.AddAsync(group);
		}

		public async Task SaveChanges()
		{
			await context.SaveChangesAsync();
		}

		public async Task DeleteGroup(Group group)
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