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
        // Retrieves all groups asynchronously
        Task<IEnumerable<Group>> GetAllAsync();

        // Adds the new group members.
        void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData);
        Task InsertAsync(Group group);
        Task SaveAsync();

    }
}
