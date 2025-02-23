using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	public class UserRepository : Repository<TicketAppUser>, IUserRepository
	{
		private readonly UserManager<TicketAppUser> _userManager;

		public UserRepository(TicketAppContext ctx, UserManager<TicketAppUser> userManager) : base(ctx)
		{
			_userManager = userManager;
		}

		public Task<Dictionary<TicketAppUser, string>> GetUsersAndRoleAsync()
		{
			var users = context.Users.ToList();
			var roleDictionary = new Dictionary<TicketAppUser, string>();

			foreach (var user in users)
			{
				var role = _userManager.GetRolesAsync(user);
				roleDictionary.Add(user, role.Result.FirstOrDefault() ?? "No Role");
			}

			return Task.FromResult(roleDictionary);
		}
	}
}
