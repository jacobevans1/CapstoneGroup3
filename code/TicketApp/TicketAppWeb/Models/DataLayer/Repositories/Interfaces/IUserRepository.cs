using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces
{
	/// <summary>
	/// The IUserRepository interface defines the methods that must be implemented by all User repository classes.
	/// </summary>
	public interface IUserRepository : IRepository<TicketAppUser>
	{
		/// <summary>
		/// Creates a new user and assigns them a specified role.
		/// </summary>
		/// <param name="user">The user to be created.</param>
		/// <param name="roleName">The name of the role to assign to the user.</param>
		Task CreateUser(TicketAppUser user, string roleName);

		/// <summary>
		/// Updates an existing user's details and their assigned role.
		/// </summary>
		/// <param name="user">The user with updated details.</param>
		/// <param name="roleName">The name of the new role to assign to the user.</param>
		Task UpdateUser(TicketAppUser user, string roleName);

		/// <summary>
		/// Retrieves all roles from the database.
		/// </summary>
		Task<IEnumerable<IdentityRole>> GetDbRoles();

		/// <summary>
		/// Retrieves all users along with their assigned roles.
		/// </summary>
		Task<Dictionary<TicketAppUser, string>> GetUserRoles();

		/// <summary>
		/// Retrieves all users from the database.
		/// </summary>
		Task<IEnumerable<TicketAppUser>> GetAllUsers();

		/// <summary>
		/// Retrieves a user by their ID.
		/// </summary>
		Task<TicketAppUser> GetUserById(string userId);

		/// <summary>
		/// Checks if a user is the manager of a specific group.
		/// </summary>
		bool IsUserManagerOfGroup(TicketAppUser user, Group group);


		/// <summary>
		/// Checks if a user is the manager of multiple groups.
		/// </summary>
		Task<IEnumerable<Group>> IsUserManagerOfMultipleGroups(TicketAppUser user);


		/// <summary>
		/// Checks if a user is the lead of a project.
		/// </summary>
		bool IsUserLeadOfProject(TicketAppUser user, Project project);


		/// <summary>
		/// Checks if a user is the lead of multiple projects.
		/// </summary>
		Task<IEnumerable<Project>> IsUserLeadOfMultipleProjects(TicketAppUser user);
	}

}
