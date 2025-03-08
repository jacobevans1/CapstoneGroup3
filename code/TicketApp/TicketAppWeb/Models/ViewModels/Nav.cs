namespace TicketAppWeb.Models.ViewModels;

// Capstone Group 3
// Spring 2025
public static class Nav
{
	/// <summary>
	/// Returns the "active" class if the value is equal to the current value.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="current"></param>
	/// <returns></returns>
	public static string Active(int value, int current)
	{
		return value == current ? "active" : "";
	}

    /// <summary>
    /// Returns the "nav-active" class if the value is equal to the current value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public static string Active(string expectedController, string currentController, string expectedAction, string currentAction)
    {
        return string.Equals(expectedController, currentController, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(expectedAction, currentAction, StringComparison.OrdinalIgnoreCase)
            ? "nav-active"
            : string.Empty;
    }
}