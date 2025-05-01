namespace TicketAppDesktop.Services;

/// <summary>
/// Abstraction over MessageBox so ViewModels can call it without
/// directly depending on System.Windows.Forms.
/// </summary>
public interface IMessageBoxService
{
	/// <summary>
	/// Shows the error.
	/// </summary>
	/// <param name="message">The message.</param>
	void ShowError(string message);
}

/// <summary>
/// Production implementation that really pops up a MessageBox.
/// </summary>
/// <seealso cref="TicketAppDesktop.Services.IMessageBoxService" />
public class MessageBoxService : IMessageBoxService
{
	/// <summary>
	/// Shows the error.
	/// </summary>
	/// <param name="message">The message.</param>
	public void ShowError(string message)
	{
		MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
	}
}

/// <summary>
/// The user detail class
/// </summary>
public class UserDetails
{
	/// <summary>
	/// Gets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public string Id { get; init; } = string.Empty;
	/// <summary>
	/// Gets the name of the user.
	/// </summary>
	/// <value>
	/// The name of the user.
	/// </value>
	public string UserName { get; init; } = string.Empty;
	/// <summary>
	/// Gets the first name.
	/// </summary>
	/// <value>
	/// The first name.
	/// </value>
	public string FirstName { get; init; } = string.Empty;
	/// <summary>
	/// Gets the last name.
	/// </summary>
	/// <value>
	/// The last name.
	/// </value>
	public string LastName { get; init; } = string.Empty;
	/// <summary>
	/// Gets the password hash.
	/// </summary>
	/// <value>
	/// The password hash.
	/// </value>
	public string PasswordHash { get; init; } = string.Empty;
	/// <summary>
	/// Gets the role.
	/// </summary>
	/// <value>
	/// The role.
	/// </value>
	public string Role { get; init; } = "NONE";
}

/// <summary>
/// Abstraction over whatever backend you use to log in a user.
/// </summary>
public interface IAuthenticationService
{
	/// <summary>
	/// Returns null if no such username.
	/// </summary>
	/// <param name="username">The username.</param>
	/// <returns></returns>
	UserDetails? GetUserDetails(string username);

	/// <summary>
	/// Verifies the plaintext against the stored hash.
	/// </summary>
	/// <param name="plainText">The plain text.</param>
	/// <param name="passwordHash">The password hash.</param>
	/// <returns></returns>
	bool VerifyPassword(string plainText, string passwordHash);

	/// <summary>
	/// True if the user leads at least one project.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns>
	///   <c>true</c> if [is project leader] [the specified user identifier]; otherwise, <c>false</c>.
	/// </returns>
	bool IsProjectLeader(string userId);

	/// <summary>
	/// True if the user manages at least one group.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns>
	///   <c>true</c> if [is group manager] [the specified user identifier]; otherwise, <c>false</c>.
	/// </returns>
	bool IsGroupManager(string userId);
}