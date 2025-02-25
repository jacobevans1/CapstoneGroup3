using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models.DomainModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.Configuration
{
	/// <summary>
	/// The SeedData class initializes the database with default data.
	/// </summary>
	public class SeedData
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<TicketAppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			var roleNames = new[] { "Admin", "Manager", "User" };

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			var adminUser = await userManager.FindByEmailAsync("admin@domain.com");
			if (adminUser == null)
			{
				adminUser = new TicketAppUser
				{
					UserName = "Admin",
					FirstName = "Admin",
					LastName = "User",
					Email = "admin@domain.com",
					EmailConfirmed = true
				};

				var result = await userManager.CreateAsync(adminUser, "Admin123!");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");
				}
			}
		}

		/// <summary>
		///	Adds users to the database.
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="userManager"></param>
		/// <param name="roleManager"></param>
		/// <returns></returns>
		public static async Task AddUsers(IServiceProvider serviceProvider, UserManager<TicketAppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
		}
	}
}
