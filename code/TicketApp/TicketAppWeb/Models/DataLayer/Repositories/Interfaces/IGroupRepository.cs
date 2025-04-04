using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces;

/// <summary>
/// The IGroupRepository interface defines the methods that must be implemented by all Group repository classes.
/// Jacob Evans
/// 02/22/2025
/// </summary>
public interface IGroupRepository : IRepository<Group>
{
	Task<IEnumerable<Group>> GetAllAsync();

	void AddNewGroupMembers(Group? group, string[] userIds, IRepository<TicketAppUser> memberData);
	Task InsertAsync(Group group);
	Task SaveAsync();
	Task<Group?> GetAsync(string id);
	Task DeleteGroupAsync(Group group);

    /// <summary>
    /// Gets the group by manager identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Task<List<Group>> GetGroupByManagerIdAsync(string id);
}
