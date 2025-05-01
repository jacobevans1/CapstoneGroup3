namespace TicketAppDesktop.Models;

/// <summary>
/// Holds the current user's session state.
/// </summary>
public static class UserSession
{
	/// <summary>
	/// Gets or sets the ID of the currently logged-in user.
	/// This should be set after successful login.
	/// </summary>
	/// <value>
	/// The current user identifier.
	/// </value>
	public static string CurrentUserId { get; set; } = string.Empty;
}
