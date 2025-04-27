using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;
using TicketAppDesktop.Services;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// The data layer for the loging view model and view
/// </summary>
/// <seealso cref="TicketAppDesktop.Services.IAuthenticationService" />
public class LoginDAL : IAuthenticationService
{
	/// <summary>
	/// Returns null if no such username.
	/// </summary>
	/// <param name="username">The username.</param>
	/// <returns></returns>
	public UserDetails? GetUserDetails(string username)
	{
		using var conn = new SqlConnection(Connection.ConnectionString);
		conn.Open();

		const string q = @"
                SELECT Id, FirstName, LastName, UserName, PasswordHash
                  FROM AspNetUsers
                 WHERE UserName = @u";

		using var cmd = new SqlCommand(q, conn);
		cmd.Parameters.AddWithValue("@u", username);

		using var rdr = cmd.ExecuteReader();
		if (!rdr.Read())
			return null;

		var id = rdr.GetString(0);
		var hash = rdr.GetString(4);

		return new UserDetails
		{
			Id = id,
			FirstName = rdr.GetString(1),
			LastName = rdr.GetString(2),
			UserName = rdr.GetString(3),
			PasswordHash = hash,
			Role = GetUserRole(conn, id)
		};
	}

	/// <summary>
	/// Gets the user role.
	/// </summary>
	/// <param name="conn">The connection.</param>
	/// <param name="userId">The user identifier.</param>
	/// <returns></returns>
	private string GetUserRole(SqlConnection conn, string userId)
	{
		const string q = @"
                SELECT r.NormalizedName
                  FROM AspNetUserRoles ur
                  JOIN AspNetRoles r ON ur.RoleId = r.Id
                 WHERE ur.UserId = @id";

		using var cmd = new SqlCommand(q, conn);
		cmd.Parameters.AddWithValue("@id", userId);
		var o = cmd.ExecuteScalar();
		return o?.ToString() ?? "NONE";
	}

	/// <summary>
	/// Verifies the plaintext against the stored hash.
	/// </summary>
	/// <param name="plainText">The plain text.</param>
	/// <param name="passwordHash">The password hash.</param>
	/// <returns></returns>
	public bool VerifyPassword(string plainText, string passwordHash)
	{
		var hasher = new PasswordHasher<TicketAppUser>();
		return hasher.VerifyHashedPassword(new TicketAppUser(), passwordHash, plainText)
			   == PasswordVerificationResult.Success;
	}

	/// <summary>
	/// True if the user leads at least one project.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns>
	///   <c>true</c> if [is project leader] [the specified user identifier]; otherwise, <c>false</c>.
	/// </returns>
	public bool IsProjectLeader(string userId)
	{
		using var conn = new SqlConnection(Connection.ConnectionString);
		conn.Open();
		using var cmd = new SqlCommand(
			"SELECT COUNT(*) FROM Projects WHERE LeadId = @u", conn);
		cmd.Parameters.AddWithValue("@u", userId);
		return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
	}

	/// <summary>
	/// True if the user manages at least one group.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns>
	///   <c>true</c> if [is group manager] [the specified user identifier]; otherwise, <c>false</c>.
	/// </returns>
	public bool IsGroupManager(string userId)
	{
		using var conn = new SqlConnection(Connection.ConnectionString);
		conn.Open();
		using var cmd = new SqlCommand(
			"SELECT COUNT(*) FROM Groups WHERE ManagerId = @u", conn);
		cmd.Parameters.AddWithValue("@u", userId);
		return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
	}
}
