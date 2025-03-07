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
            var roleNames = new[] { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create Admin User
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

            // Create standard users
            await CreateUserIfNotExists(userManager, "jabesi@domain.com", "Jabesi", "Abwe", "User");
            await CreateUserIfNotExists(userManager, "jacob@domain.com", "Jacob", "Evans", "User");
        }

        /// <summary>
        /// Creates a user if they do not already exist.
        /// </summary>
        private static async Task CreateUserIfNotExists(UserManager<TicketAppUser> userManager, string email, string firstName, string lastName, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new TicketAppUser
                {
                    UserName = firstName,  // Using first name as the username
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, firstName + "123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
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
