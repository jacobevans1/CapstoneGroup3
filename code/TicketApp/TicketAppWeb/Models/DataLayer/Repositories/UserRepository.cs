using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories
{
	/// <summary>
	/// The UserRepository class implements IUserRepository
	/// </summary>
	public class UserRepository : Repository<TicketAppUser>, IUserRepository
	{
		private readonly UserManager<TicketAppUser> _userManager;


		/// <summary>
		/// Initializes a new instance of the <see cref="UserRepository"/> class.
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="userManager"></param>
		public UserRepository(TicketAppContext ctx, UserManager<TicketAppUser> userManager) : base(ctx)
		{
			_userManager = userManager;
		}


		/// <summary>
		/// Creates a new user and assigns them a specified role.
		/// </summary>
		/// <param name="user">The user to be created.</param>
		/// <param name="roleName">The name of the role to assign to the user.</param>
		public async Task CreateUser(TicketAppUser user, string roleName)
		{
			if (DoesUserExist(user))
			{
				throw new Exception("User already exists.");
			}

			user.UserName = user.FirstName + user.LastName;
			var password = user.UserName + "123!";
			user.EmailConfirmed = true;

			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, roleName);
			}
			else
			{
				var exception = new Exception(result.ToString());
				exception.Data.Add("Errors", result.Errors);
				throw exception;
			}
		}


		/// <summary>
		/// Updates an existing user's details and their assigned role.
		/// </summary>
		/// <param name="user">The user with updated details.</param>
		/// <param name="roleName">The name of the new role to assign to the user.</param>
		public async Task UpdateUser(TicketAppUser user, string roleName)
		{
			if (DoesUserExist(user))
			{
				var existingUser = await _userManager.FindByIdAsync(user.Id);

				existingUser.FirstName = user.FirstName;
				existingUser.LastName = user.LastName;
				existingUser.Email = user.Email;
				existingUser.PhoneNumber = user.Email;

				await _userManager.UpdateAsync(existingUser);

				var roles = await _userManager.GetRolesAsync(existingUser);
				if (roles.Count > 0)
				{
					await _userManager.RemoveFromRolesAsync(existingUser, roles);
				}

				await _userManager.AddToRoleAsync(existingUser, roleName);
			}
		}

		/// <summary>
		/// Retrieves all roles from the database.
		/// </summary>
		public async Task<IEnumerable<IdentityRole>> GetDbRoles()
		{
			return await context.Roles.ToListAsync();
		}


		/// <summary>
		/// Retrieves all users along with their assigned roles.
		/// </summary>
		public async Task<Dictionary<TicketAppUser, string>> GetUserRoles()
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


		/// <summary>
		/// Retrieves all users from the database.
		/// </summary>
		public async Task<IEnumerable<TicketAppUser>> GetAllUsers()
		{
			return await context.Users.ToListAsync();
		}


		/// <summary>
		/// Gets a user by their ID.
		/// </summary>
		public async Task<TicketAppUser> GetUserById(string userId)
		{
			return await context.Users.FindAsync(userId);
		}


		/// <summary>
		/// Checks if a user is the manager of a specific group.
		/// </summary>
		public bool IsUserManagerOfGroup(TicketAppUser user, Group group)
		{
			var managerId = context.Groups.FirstOrDefault(g => g.Id == group.Id)?.ManagerId;
			return managerId == user.Id;
		}


		/// <summary>
		/// Checks if a user is the manager of multiple groups.
		/// </summary>
		public Task<IEnumerable<Group>> IsUserManagerOfMultipleGroups(TicketAppUser user)
		{
			var groups = context.Groups.Where(g => g.ManagerId == user.Id).ToList();
			return Task.FromResult(groups.AsEnumerable());
		}


		/// <summary>
		/// Checks if a user is the lead of a project.
		/// </summary>
		public bool IsUserLeadOfProject(TicketAppUser user, Project project)
		{
			var leadId = context.Projects.FirstOrDefault(p => p.Id == project.Id)?.LeadId;
			return leadId == user.Id;
		}


		/// <summary>
		/// Checks if a user is the lead of multiple projects.
		/// </summary>
		public Task<IEnumerable<Project>> IsUserLeadOfMultipleProjects(TicketAppUser user)
		{
			var projects = context.Projects.Where(p => p.LeadId == user.Id).ToList();
			return Task.FromResult(projects.AsEnumerable());
		}


		private bool DoesUserExist(TicketAppUser user)
		{
			return context.Users.Any(u => u.UserName == user.UserName);
		}
	}
}
