using Microsoft.Data.SqlClient;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.DataLayer
{
    public static class GroupsDAL
    {
        // Use the connection string from the Connection class.
        private static readonly string connectionString = Connection.ConnectionString;

        /// <summary>
        /// Saves a Group record to the database.
        /// </summary>
        /// <param name="group">The Group instance to be saved.</param>
        public static void SaveGroup(Group group)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO [dbo].[Groups] ([Id], [GroupName], [Description], [ManagerId], [CreatedAt])
                    VALUES (@Id, @GroupName, @Description, @ManagerId, @CreatedAt);";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Pass the Group properties as parameters. Assuming Group.Id and Group.ManagerId are strings.
                    command.Parameters.AddWithValue("@Id", group.Id);
                    command.Parameters.AddWithValue("@GroupName", group.GroupName);
                    command.Parameters.AddWithValue("@Description", group.Description);
                    command.Parameters.AddWithValue("@ManagerId", group.ManagerId);
                    command.Parameters.AddWithValue("@CreatedAt", group.CreatedAt);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving group: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all Group records from the database.
        /// </summary>
        /// <returns>A list of Group objects.</returns>
        public static List<Group> RetrieveAllGroups()
        {
            List<Group> groups = new List<Group>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT [Id], [GroupName], [Description], [ManagerId], [CreatedAt]
                    FROM [dbo].[Groups];";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Since [Id] and [ManagerId] are stored as strings in the database, use GetString().
                                string id = reader.GetString(0);
                                string groupName = reader.GetString(1);
                                string description = reader.GetString(2);
                                string managerId = reader.GetString(3);
                                DateTime createdAt = reader.GetDateTime(4);

                                // Create a new Group instance using the provided constructor.
                                Group group = new Group(id, groupName, description, managerId, createdAt);
                                groups.Add(group);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving groups: " + ex.Message);
                    }
                }
            }
            return groups;
        }
    }
}
