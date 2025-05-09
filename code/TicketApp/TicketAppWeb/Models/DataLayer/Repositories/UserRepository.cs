﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer.Repositories;

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
		if (checkIfUserExists(user))
		{
			throw new Exception("User already exists.");
		}

		user.UserName = user.FirstName + user.LastName;
		var password = user.UserName + "123!";

		user.EmailConfirmed = true;

		if (user.PhoneNumber != null)
		{
			var normalizedPhone = normalizePhone(user.PhoneNumber);
			user.PhoneNumber = normalizedPhone;
		}

		var result = await _userManager.CreateAsync(user, password);

		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(user, roleName);
		}
		else
		{
			var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
			var exception = new Exception($"Failed: {errorMessages}");
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
		if (checkIfUserExists(user))
		{
			var existingUser = await _userManager.FindByIdAsync(user.Id);

			existingUser!.FirstName = user.FirstName;
			existingUser.LastName = user.LastName;
			existingUser.Email = user.Email;

			if (user.PhoneNumber != null)
			{
				var normalizedPhone = normalizePhone(user.PhoneNumber);
				existingUser.PhoneNumber = normalizedPhone;
			}

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
	public async Task<IEnumerable<IdentityRole>> GetRolesAsync()
	{
		return await context.Roles.ToListAsync();
	}

	/// <summary>
	/// Retrieves all users along with their assigned roles.
	/// </summary>
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

	/// <summary>
	/// Retrieves all users from the database.
	/// </summary>
	public async Task<IEnumerable<TicketAppUser>> GetAllUsersAsync()
	{
		return await context.Users.ToListAsync();
	}

	/// <summary>
	/// Gets the users by group identifier asynchronous.
	/// </summary>
	public IEnumerable<TicketAppUser> GetUsersByGroupId(string groupId)
	{
		var query = @"
	    SELECT u.*
	    FROM AspNetUsers u
	    INNER JOIN GroupUser gu ON gu.MemberId = u.Id
	    WHERE gu.GroupId = @GroupId";

		var users = context.Users
			.FromSqlRaw(query, new SqlParameter("@GroupId", groupId))
			.ToList();

		return users;
	}

	/// <summary>
	/// Gets the user by id asynchronous.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	public async Task<TicketAppUser?> GetAsync(string userId)
	{
		return await context.Users.FindAsync(userId);
	}

	private bool checkIfUserExists(TicketAppUser user)
	{
		return context.Users.Any(u => u.UserName == user.UserName);
	}

	private string normalizePhone(string input)
	{
		var digits = new string(input.Where(char.IsDigit).ToArray());
		if (digits.Length == 10)
			return $"+1{digits}";
		else if (digits.Length == 11 && digits.StartsWith("1"))
			return $"+{digits}";
		else
			return input;
	}
}
