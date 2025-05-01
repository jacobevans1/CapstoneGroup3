using Microsoft.Data.SqlClient;
namespace TicketAppDesktop.DataLayer;

/// <summary>
/// User data layer
/// </summary>
public class UsersDAL
{
	private static readonly string cs = Connection.ConnectionString;

	/// <summary>
	/// Gets the full name of the user from the databse.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns></returns>
	public static string GetFullName(string userId)
	{
		using var conn = new SqlConnection(cs);
		const string q = @"SELECT FirstName, LastName FROM AspNetUsers WHERE Id = @Id";
		using var cmd = new SqlCommand(q, conn);
		cmd.Parameters.AddWithValue("@Id", userId);
		conn.Open();
		using var rdr = cmd.ExecuteReader();
		if (rdr.Read())
			return $"{rdr.GetString(0)} {rdr.GetString(1)}";
		return "Unknown User";
	}
}
