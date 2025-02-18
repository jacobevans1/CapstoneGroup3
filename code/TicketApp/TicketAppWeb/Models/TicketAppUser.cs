using Microsoft.AspNetCore.Identity;

namespace TicketAppWeb.Models
{
	public class TicketAppUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName => $"{FirstName} {LastName}";
	}
}
