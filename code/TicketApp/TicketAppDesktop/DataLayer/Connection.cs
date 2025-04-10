namespace TicketAppDesktop.DataLayer;

public class Connection
{
    /// <summary>
    /// Using a read-only property to store the connection string.
    /// The @ symbol makes it a verbatim string literal so that you don't have to escape backslashes.
    /// </summary>
    public static string ConnectionString { get; } =
        @"Server=(localdb)\MSSQLLocalDB;Database=TicketAppDB;Trusted_Connection=True;MultipleActiveResultSets=true";
}
