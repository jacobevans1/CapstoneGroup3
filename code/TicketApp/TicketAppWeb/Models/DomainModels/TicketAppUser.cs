using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace TicketAppWeb.Models.DomainModels;

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
		/// Initializes a new instance of the <see cref="TicketAppUser"/> class.
		/// </summary>
		public TicketAppUser()
		{
			Groups = new HashSet<Group>();
		}

		/// <summary>
		/// Gets or sets the first name of the user.
		/// </summary>
		[Required(ErrorMessage = "Please enter a first name")]
		public string FirstName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the last name of the user.
		/// </summary>
		[Required(ErrorMessage = "Please enter a last name")]
		public string LastName { get; set; } = string.Empty;

		/// <summary>
		/// Gets the full name of the user by combining the first and last names.
		/// </summary>
		public string FullName => $"{FirstName} {LastName}";

		/// <summary>
		/// Gets or sets the groups that the user is a member of.
		/// </summary>
		public virtual ICollection<Group> Groups { get; set; }
	}
}