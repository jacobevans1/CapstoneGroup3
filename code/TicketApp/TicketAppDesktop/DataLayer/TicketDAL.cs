using Microsoft.Data.SqlClient;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.DataLayer;

/// <summary>
/// Ticket data layer
/// </summary>
public static class TicketDAL
{
	private static readonly string connectionString = Connection.ConnectionString;

	/// <summary>
	/// Retrieves unassigned tickets in stages assigned to any group of the current user.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns></returns>
	public static List<Ticket> GetAvailableTasksForUserGroups(string userId)
	{
		var tickets = new List<Ticket>();
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"SELECT DISTINCT t.Id, t.Title, t.Description, t.CreatedDate, t.CreatedBy,
							t.AssignedTo, t.Stage, t.IsComplete, t.BoardId
					FROM Tickets t
					JOIN BoardStages bs         ON t.BoardId = bs.BoardId AND t.Stage = bs.StageId
					JOIN BoardStageGroups bsg   ON bs.BoardId = bsg.BoardId AND bs.StageId = bsg.StageId
					JOIN GroupUser gu           ON bsg.GroupId = gu.GroupId
					WHERE gu.MemberId = @UserId
						AND (t.AssignedTo IS NULL OR t.AssignedTo = 'Unassigned')";

			using var cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@UserId", userId);
			conn.Open();
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					tickets.Add(MapReaderToTicket(reader));
				}
			}
		}
		return tickets;
	}

	/// <summary>
	/// Retrieves tickets assigned to the specified user.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns></returns>
	public static List<Ticket> GetTasksByAssignee(string userId)
	{
		var tickets = new List<Ticket>();
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"SELECT Id, Title, Description, CreatedDate, CreatedBy,
					   AssignedTo, Stage, IsComplete, BoardId
					  FROM Tickets
					  WHERE AssignedTo = @UserId";

			using var cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@UserId", userId);
			conn.Open();
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					tickets.Add(MapReaderToTicket(reader));
				}
			}
		}
		return tickets;
	}

	/// <summary>
	/// Retrieves a ticket by its identifier.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <returns></returns>
	public static Ticket GetTicketById(string ticketId)
	{
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"SELECT Id, Title, Description, CreatedDate, CreatedBy,
							   AssignedTo, Stage, IsComplete, BoardId
					  FROM Tickets
					  WHERE Id = @TicketId";

			using (var cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@TicketId", ticketId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return MapReaderToTicket(reader);
					}
				}
			}
		}
		return null!;
	}

	/// <summary>
	/// Updates an existing ticket.
	/// </summary>
	/// <param name="ticket">The ticket.</param>
	public static void UpdateTicket(Ticket ticket)
	{
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"UPDATE Tickets
					  SET Title = @Title,
						  Description = @Description,
						  Stage = @Stage,
						  AssignedTo = @AssignedTo
					  WHERE Id = @TicketId";

			using (var cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@Title", ticket.Title ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@Description", ticket.Description ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@Stage", ticket.Stage);
				cmd.Parameters.AddWithValue("@AssignedTo", ticket.AssignedTo ?? "Unassigned");
				cmd.Parameters.AddWithValue("@TicketId", ticket.Id);

				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}
	}

	private static Ticket MapReaderToTicket(SqlDataReader reader)
	{
		return new Ticket
		{
			Id = reader.GetString(0),
			Title = reader.GetString(1),
			Description = reader.IsDBNull(2) ? null : reader.GetString(2),
			CreatedDate = reader.GetDateTime(3),
			CreatedBy = reader.GetString(4),
			AssignedTo = reader.IsDBNull(5) ? null : reader.GetString(5),
			Stage = reader.GetString(6),
			IsComplete = reader.GetBoolean(7),
			BoardId = reader.GetString(8)
		};
	}
}
