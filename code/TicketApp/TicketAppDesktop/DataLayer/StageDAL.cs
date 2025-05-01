using Microsoft.Data.SqlClient;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.DataLayer;

/// <summary>
/// Stage data layer
/// </summary>
public static class StageDAL
{
	private static readonly string connectionString = Connection.ConnectionString;

	/// <summary>
	/// Retrieves all stages for the specified board, ordered by StageOrder.
	/// </summary>
	/// <param name="boardId">The board identifier.</param>
	/// <returns></returns>
	public static List<Stage> GetStagesForBoard(string boardId)
	{
		var stages = new List<Stage>();
		using (var conn = new SqlConnection(connectionString))
		{
			const string query =
				@"SELECT s.Id, s.Name
                      FROM BoardStages bs
                      JOIN Stages s ON bs.StageId = s.Id
                      WHERE bs.BoardId = @BoardId
                      ORDER BY bs.StageOrder";

			using (var cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@BoardId", boardId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						stages.Add(new Stage
						{
							Id = reader.GetString(0),
							Name = reader.GetString(1)
						});
					}
				}
			}
		}
		return stages;
	}

	/// <summary>
	/// Retrieves a single stage by its identifier.
	/// </summary>
	/// <param name="stageId">The stage identifier.</param>
	/// <returns></returns>
	public static Stage? GetStageById(string stageId)
	{
		using var conn = new SqlConnection(connectionString);
		const string query =
			@"SELECT Id, Name
                  FROM Stages
                  WHERE Id = @StageId";

		using var cmd = new SqlCommand(query, conn);
		cmd.Parameters.AddWithValue("@StageId", stageId);
		conn.Open();

		using var reader = cmd.ExecuteReader();
		if (reader.Read())
		{
			return new Stage
			{
				Id = reader.GetString(0),
				Name = reader.GetString(1)
			};
		}

		return null;
	}
}
