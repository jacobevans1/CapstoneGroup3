using Microsoft.AspNetCore.Identity;

namespace TicketAppWeb.Models.DomainModels
{
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
