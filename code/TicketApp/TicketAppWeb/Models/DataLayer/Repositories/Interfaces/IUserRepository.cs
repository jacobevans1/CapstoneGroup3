using Microsoft.AspNetCore.Identity;
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
		new Task CreateUser(TicketAppUser user, IdentityRole role);
		Task<IEnumerable<IdentityRole>> GetRolesAsync();
		Task<Dictionary<TicketAppUser, string>> GetUserRolesAsync();
	}
}
