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
        public TicketAppUser()
        {
            Groups = new HashSet<Group>();
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        // Many-to-Many relationship: User <-> Groups
        public virtual ICollection<Group> Groups { get; set; }
    }
}