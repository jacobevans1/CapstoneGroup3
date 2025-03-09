// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DomainModels
{
	/// <summary>
	/// Singleton service that will be created once per application.
	/// </summary>
	public class SingletonService
	{
		/// <summary>
		/// Unique identifier of the service.
		/// </summary>
		public Guid Id { get; } = Guid.NewGuid();

		/// <summary>
		/// Gets the message of the service.
		/// </summary>
		public string GetMessage() => $"SingletonService with Id: {Id}";

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		public TicketAppUser CurrentUser { get; set; }

		/// <summary>
		///	Gets or sets the current user role.
		/// </summary>
		public virtual string? CurrentUserRole { get; set; }
	}
}
