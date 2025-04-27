using TicketAppDesktop.Models;
using TicketAppDesktop.Services;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// The log in view model
/// </summary>
public class LoginViewModel
{
	/// <summary>
	/// The authentication
	/// </summary>
	private readonly IAuthenticationService _auth;
	/// <summary>
	/// The MSG
	/// </summary>
	private readonly IMessageBoxService _msg;

	/// <summary>
	/// Gets or sets the user.
	/// </summary>
	/// <value>
	/// The user.
	/// </value>
	public TicketAppUser User { get; set; }
	/// <summary>
	/// Gets or sets the input password.
	/// </summary>
	/// <value>
	/// The input password.
	/// </value>
	public string InputPassword { get; set; } = "";

	/// <summary>
	/// Gets the logged in user.
	/// </summary>
	/// <value>
	/// The logged in user.
	/// </value>
	public string LoggedInUser { get; private set; } = "";
	/// <summary>
	/// Gets the user role.
	/// </summary>
	/// <value>
	/// The user role.
	/// </value>
	public string UserRole { get; private set; } = "NONE";
	/// <summary>
	/// Gets a value indicating whether this instance is project leader.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is project leader; otherwise, <c>false</c>.
	/// </value>
	public bool IsProjectLeader { get; private set; }
	/// <summary>
	/// Gets a value indicating whether this instance is group manager.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is group manager; otherwise, <c>false</c>.
	/// </value>
	public bool IsGroupManager { get; private set; }

	/// <summary>
	/// Now takes both your auth‐service and a message‐box‐service.
	/// </summary>
	/// <param name="authService">The authentication service.</param>
	/// <param name="messageBoxService">The message box service.</param>
	/// <exception cref="System.ArgumentNullException">
	/// authService
	/// or
	/// messageBoxService
	/// </exception>
	public LoginViewModel(
		IAuthenticationService authService,
		IMessageBoxService messageBoxService)
	{
		_auth = authService
			?? throw new ArgumentNullException(nameof(authService));
		_msg = messageBoxService
			?? throw new ArgumentNullException(nameof(messageBoxService));
		User = new TicketAppUser();
	}

	/// <summary>
	/// Logins this instance.
	/// </summary>
	/// <returns></returns>
	public bool Login()
	{
		if (string.IsNullOrWhiteSpace(User.UserName))
		{
			ShowError("Please enter your username.");
			return false;
		}
		if (string.IsNullOrWhiteSpace(InputPassword))
		{
			ShowError("Please enter your password.");
			return false;
		}

		var details = _auth.GetUserDetails(User.UserName!);
		if (details == null)
		{
			ShowError("Invalid username or password. Please try again.");
			return false;
		}

		if (!_auth.VerifyPassword(InputPassword, details.PasswordHash))
		{
			ShowError("Invalid username or password. Please try again.");
			return false;
		}

		// success!
		User.Id = details.Id;
		UserSession.CurrentUserId = details.Id;

		LoggedInUser = $"{details.UserName} ({details.FirstName} {details.LastName})";
		UserRole = details.Role;
		IsProjectLeader = _auth.IsProjectLeader(details.Id);
		IsGroupManager = _auth.IsGroupManager(details.Id);

		return true;
	}

	/// <summary>
	/// Shows the error.
	/// </summary>
	/// <param name="message">The message.</param>
	private void ShowError(string message)
		=> _msg.ShowError(message);
}
