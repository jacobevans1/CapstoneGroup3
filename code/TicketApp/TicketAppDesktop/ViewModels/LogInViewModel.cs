using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// The view model for the login form.
/// Logs the user in, retrieves their role,
/// checks if they lead a project, and checks if they manage a group.
/// Jabesi Abwe 
/// Spring 2025
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// In a login form you’d normally have a separate field for the plain text password.
    /// Here we assume that TicketAppUser.UserName holds the username, and we add a Password property for input.
    /// </summary> 
    public TicketAppUser User { get; set; }
    public string InputPassword { get; set; } = string.Empty;

    /// <summary>
    /// A formatted display of the user (e.g., "jdoe (John Doe)")
    /// </summary>
    public string LoggedInUser { get; set; } = string.Empty;

    /// <summary>
    /// This will be retrieved from the join table between AspNetUserRoles and AspNetRoles.
    /// </summary>
    public string UserRole { get; set; } = "NONE";

    // Flags to indicate if the user is a project leader or group manager.
    public bool IsProjectLeader { get; private set; } = false;
    public bool IsGroupManager { get; private set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    public LoginViewModel()
    {
        User = new TicketAppUser();
    }

    /// <summary>
    /// Attempts to log in the user using the provided username and password.
    /// It retrieves the user’s details, verifies the password, obtains the user's role,
    /// and checks if the user leads a project or manages a group.
    /// </summary>
    /// <returns>True if the login succeeds; otherwise, false.</returns>
    public bool Login()
    {
        if (!ValidateInputs())
            return false;

        using (var connection = new SqlConnection(Connection.ConnectionString))
        {
            try
            {
                connection.Open();

                var userDetails = GetUserDetails(connection, User.UserName!);
                if (userDetails == null)
                {
                    ShowErrorMessage("Invalid username or password. Please try again.");
                    return false;
                }

                if (!VerifyPassword(InputPassword, userDetails.PasswordHash))
                {
                    ShowErrorMessage("Invalid username or password. Please try again.");
                    return false;
                }
				User.Id = userDetails.Id;                      
				UserSession.CurrentUserId = userDetails.Id;
				LoggedInUser = $"{userDetails.UserName} ({userDetails.FirstName} {userDetails.LastName})";
                UserRole = userDetails.Role;

                IsProjectLeader = CheckIfProjectLeader(connection, userDetails.Id);
                IsGroupManager = CheckIfGroupManager(connection, userDetails.Id);

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred while connecting to the database.\nError: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Validates that the username and password have been entered.
    /// </summary>
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(User.UserName))
        {
            ShowErrorMessage("Please enter your username.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(InputPassword))
        {
            ShowErrorMessage("Please enter your password.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Retrieves user details from the AspNetUsers table for the given username.
    /// </summary>
    private UserDetails? GetUserDetails(SqlConnection connection, string username)
    {
        string query = @"
                SELECT 
                    Id,
                    FirstName,
                    LastName,
                    UserName,
                    PasswordHash
                FROM [dbo].[AspNetUsers]
                WHERE UserName = @username";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@username", username);
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                    return null;

                var id = reader["Id"].ToString();
                return new UserDetails
                {
                    Id = id!,
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString()!,
                    Role = GetUserRole(connection, id!)
                };
            }
        }
    }

    /// <summary>
    /// Retrieves the user role by joining AspNetUserRoles and AspNetRoles.
    /// Returns "NONE" if no role is assigned.
    /// </summary>
    private string GetUserRole(SqlConnection connection, string userId)
    {
        string query = @"
                SELECT r.NormalizedName
                FROM [dbo].[AspNetUserRoles] ur
                JOIN [dbo].[AspNetRoles] r ON ur.RoleId = r.Id
                WHERE ur.UserId = @userId";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            var role = command.ExecuteScalar();
            return role?.ToString() ?? "NONE";
        }
    }

    /// <summary>
    /// Verifies the entered password against the stored hash.
    /// </summary>
    private bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
        var dummyUser = new TicketAppUser();
        PasswordHasher<TicketAppUser> passwordHasher = new PasswordHasher<TicketAppUser>();
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(dummyUser, storedPasswordHash, inputPassword);
        return result == PasswordVerificationResult.Success;
    }

    /// <summary>
    /// Checks if the user leads any project by counting rows in the Projects table.
    /// </summary>
    private bool CheckIfProjectLeader(SqlConnection connection, string userId)
    {
        string query = "SELECT COUNT(*) FROM [dbo].[Projects] WHERE LeadId = @userId";
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    /// <summary>
    /// Checks if the user manages any group by counting rows in the Groups table.
    /// </summary>
    private bool CheckIfGroupManager(SqlConnection connection, string userId)
    {
        string query = "SELECT COUNT(*) FROM [dbo].[Groups] WHERE ManagerId = @userId";
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    /// <summary>
    /// Displays an error message to the user.
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// A private class used to hold user details retrieved from the database.
    /// </summary>
    private class UserDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; } = "NONE";
    }
}
