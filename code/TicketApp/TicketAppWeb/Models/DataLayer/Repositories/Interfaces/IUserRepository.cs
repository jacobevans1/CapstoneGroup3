using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces
{
	/// <summary>
	/// The IUserRepository interface defines the methods that must be implemented by all User repository classes.
	/// Jacob Evans
	/// 02/22/2025
	/// </summary>
	public interface IUserRepository : IRepository<TicketAppUser>
	{
		Task<Dictionary<TicketAppUser, string>> GetUsersAndRoleAsync();
	}
}
