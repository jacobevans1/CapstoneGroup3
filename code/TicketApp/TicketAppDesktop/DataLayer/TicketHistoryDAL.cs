using Microsoft.Data.SqlClient;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.DataLayer;

/// <summary>
/// The ticket history data layer
/// </summary>
public static class TicketHistoryDAL
{
	private static readonly string connectionString = Connection.ConnectionString;

	/// <summary>
	/// Retrieves all history entries for a given ticket, ordered by change date.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <returns></returns>
	public static List<TicketHistory> GetHistoryByTicketId(string ticketId)
	{
		var history = new List<TicketHistory>();

		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"SELECT Id, TicketId, PropertyChanged, OldValue, NewValue, 
                             ChangedByUserId, ChangeDate, ChangeDescription
                      FROM TicketHistories
                      WHERE TicketId = @TicketId
                      ORDER BY ChangeDate";

			using (var cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@TicketId", ticketId);
				conn.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						history.Add(new TicketHistory
						{
							Id = reader.GetString(0),
							TicketId = reader.GetString(1),
							PropertyChanged = reader.GetString(2),
							OldValue = reader.IsDBNull(3) ? null : reader.GetString(3),
							NewValue = reader.IsDBNull(4) ? null : reader.GetString(4),
							ChangedByUserId = reader.GetString(5),
							ChangeDate = reader.GetDateTime(6),
							ChangeDescription = reader.GetString(7)
						});
					}
				}
			}
		}

		return history;
	}

	/// <summary>
	/// Inserts a new history entry for a ticket.
	/// </summary>
	/// <param name="entry">The entry.</param>
	public static void SaveHistoryEntry(TicketHistory entry)
	{
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"INSERT INTO TicketHistories 
                      (Id, TicketId, PropertyChanged, OldValue, NewValue, 
                       ChangedByUserId, ChangeDate, ChangeDescription)
                      VALUES
                      (@Id, @TicketId, @PropertyChanged, @OldValue, @NewValue, 
                       @ChangedByUserId, @ChangeDate, @ChangeDescription)";

			using (var cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@Id", entry.Id);
				cmd.Parameters.AddWithValue("@TicketId", entry.TicketId);
				cmd.Parameters.AddWithValue("@PropertyChanged", entry.PropertyChanged);
				cmd.Parameters.AddWithValue("@OldValue", (object?)entry.OldValue ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@NewValue", (object?)entry.NewValue ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@ChangedByUserId", entry.ChangedByUserId);
				cmd.Parameters.AddWithValue("@ChangeDate", entry.ChangeDate);
				cmd.Parameters.AddWithValue("@ChangeDescription", entry.ChangeDescription);

				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}
	}
}
