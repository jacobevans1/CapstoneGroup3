using Microsoft.Data.SqlClient;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.Data
{
	public static class CalculatorDAL
	{
		private static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TicketAppDB;Trusted_Connection=True;MultipleActiveResultSets=true";

		public static void SendNumber(Calculation calculation)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string query = "INSERT INTO Numbers (value) VALUES (@value);";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@value", calculation.Value);

					try
					{
						connection.Open();
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error: " + ex.Message);
					}
				}
			}
		}

		public static List<Calculation> RetrieveAllNumbers()
		{
			List<Calculation> numbers = new List<Calculation>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string query = "SELECT Id, value FROM Numbers;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					try
					{
						connection.Open();
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								int id = reader.GetInt32(0);
								double value = reader.GetDouble(1);
								numbers.Add(new Calculation(id, value));
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error: " + ex.Message);
					}
				}
			}

			return numbers;
		}
	}
}
