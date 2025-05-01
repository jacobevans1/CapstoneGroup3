using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
		/// Gets or sets the email of the user.
		/// </summary>
		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email (e.g., name@example.com)")]
		public override string Email { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the user.
		/// </summary>
		[RegularExpression(@"^(\+1\s?)?(\d{3})[-.\s]?(\d{3})[-.\s]?(\d{4})$",
			ErrorMessage = "Please enter a valid phone number (e.g., +1 123-456-7890)")]
		public override string? PhoneNumber { get; set; }

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