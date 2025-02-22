using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	public class GroupRepository(TicketAppContext ctx) : Repository<Group>(ctx), IGroupRepository
	{
		public void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData)
		{
			// first remove any current members
			foreach (TicketAppUser member in group.Members)
			{
				group.Members.Remove(member);
			}

			// then add new members
			foreach (string id in userIds)
			{
				TicketAppUser? member = memberData.Get(id);
				if (member != null)
					group.Members.Add(member);
			}
		}
	}
}
