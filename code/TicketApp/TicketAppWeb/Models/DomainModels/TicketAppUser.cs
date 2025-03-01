using Microsoft.AspNetCore.Identity;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels
{
	/// <summary>
	/// The TicketAppUser class represents a user of the TicketApp application.
	/// </summary>
	public class TicketAppUser : IdentityUser
	{
		/// <summary>
		/// The first name of the user.
		/// </summary>
		public string FirstName { get; set; } = string.Empty;

		/// <summary>
		/// The last name of the user.
		/// </summary>
		public string LastName { get; set; } = string.Empty;

		/// <summary>
		/// The full name of the user.
		/// </summary>
		public string FullName => $"{FirstName} {LastName}";
	}
}
