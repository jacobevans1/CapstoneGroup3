using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.ViewModels
{
    public class TicketDetailsViewModel
    {
        /// <summary>
        /// The currently logged in user
        /// </summary>
        [ValidateNever]
        public TicketAppUser CurrentUser { get; set; }

        /// <summary>
        /// The currently logged in user's role
        /// </summary>
        [ValidateNever]
        public string CurrentUserRole { get; set; }

        /// <summary>
        /// The ticket being viewed
        /// </summary>
        public Ticket Ticket { get; set; }

        /// <summary>
        /// The project this ticket belongs to
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// The board this ticket belongs to
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Collection of ticket history entries
        /// </summary>
        public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();

		public string StageName { get; set; }


		/// <summary>
		/// Initializes a new instance of the TicketDetailsViewModel class
		/// </summary>
		public TicketDetailsViewModel()
        {
            Ticket = new Ticket();
            Project = new Project();
            Board = new Board();
        }
    }
}