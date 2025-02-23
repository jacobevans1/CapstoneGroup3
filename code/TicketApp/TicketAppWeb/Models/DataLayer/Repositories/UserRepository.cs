using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

		public async Task CreateUser(TicketAppUser user, IdentityRole role)
		{
			if (checkIfUserExists(user))
			{
				throw new Exception("User already exists.");
			}

			user = generateUserDetails(user);
			var password = user.UserName + "123!";

			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, role.Name);
			}
			else
			{
				var exception = new Exception(result.ToString());
				exception.Data.Add("Errors", result.Errors);
				throw exception;
			}
		}

		public async Task UpdateUser(TicketAppUser user, IdentityRole role)
		{
			if (checkIfUserExists(user))
			{
				var trackedUser = context.Entry(user);
				if (trackedUser.State == EntityState.Detached)
				{
					context.Attach(user);
				}
				else
				{
					trackedUser.State = EntityState.Detached;
				}

				await _userManager.UpdateAsync(user);

				var roles = await _userManager.GetRolesAsync(user);

				if (roles.Count > 0)
				{
					await _userManager.RemoveFromRolesAsync(user, roles);
				}

				await _userManager.AddToRoleAsync(user, role.Name);
			}
		}


		public async Task<IEnumerable<IdentityRole>> GetRolesAsync()
		{
			return await context.Roles.ToListAsync();
		}

		public async Task<Dictionary<TicketAppUser, string>> GetUserRolesAsync()
		{
			var users = context.Users.ToList();
			var userRoleDictionary = new Dictionary<TicketAppUser, string>();

			foreach (var user in users)
			{
				var role = _userManager.GetRolesAsync(user);
				userRoleDictionary.Add(user, role.Result.FirstOrDefault() ?? "No Role");
			}

			return userRoleDictionary;
		}

		private bool checkIfUserExists(TicketAppUser user)
		{
			return context.Users.Any(u => u.UserName == user.UserName);
		}

		private TicketAppUser generateUserDetails(TicketAppUser user)
		{
			user.UserName = user.FirstName + user.LastName;
			user.Email = user.FirstName.ToLower() + "." + user.LastName.ToLower() + "@domain.com";
			return user;
		}
	}
}
