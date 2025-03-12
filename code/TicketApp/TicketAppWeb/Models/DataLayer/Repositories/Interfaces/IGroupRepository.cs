using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces
{
	/// <summary>
	/// The IGroupRepository interface defines the methods that must be implemented by all Group repository classes.
	/// Jacob Evans
	/// 02/22/2025
	/// </summary>
	public interface IGroupRepository : IRepository<Group>
	{
        Task<IEnumerable<Group>> GetAllGroups();

        void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData);
        Task InsertGroup(Group group);
        Task SaveChanges();
        Task<Group?> GetGroupById(string id);
        Task DeleteGroup(Group group);


    }
}
