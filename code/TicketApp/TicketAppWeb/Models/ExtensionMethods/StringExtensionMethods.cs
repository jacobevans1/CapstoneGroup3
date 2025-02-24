
namespace TicketAppWeb.Models.ExtensionMethods;

/// <summary>
/// The StringExtensionMethods class contains extension methods for the string class.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public static class StringExtensionMethods
{
    // Equalses the no case.
    public static bool EqualsNoCase(this string str, string tocompare) =>
        str?.ToLower() == tocompare?.ToLower();

    // Converts the string representation of a number to an integer.
    public static int ToInt(this string str)
    {
        int.TryParse(str, out int id);
        return id;
    }

    // Capitalize the specified string
    public static string Capitalize(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }
}
