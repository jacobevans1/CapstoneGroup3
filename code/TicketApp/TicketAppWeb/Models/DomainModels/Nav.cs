namespace TicketAppWeb.Models.DomainModels;

public static class Nav
{

    public static string Active(int value, int current)
    {
        return value == current ? "active" : "";
    }


    public static string Active(string value, string current)
    {
        return string.Equals(value, current, StringComparison.OrdinalIgnoreCase) ? "nav-active" : string.Empty;
    }
}