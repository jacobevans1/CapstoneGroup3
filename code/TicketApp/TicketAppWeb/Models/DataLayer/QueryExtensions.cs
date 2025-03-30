namespace TicketAppWeb.Models.DataLayer;

/// <summary>
/// The QueryExtensions class provides a method to page a queryable collection.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
public static class QueryExtensions
{
	/// <summary>
	/// The PageBy method pages a queryable collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="items">The items.</param>
	/// <param name="pagenumber">The pagenumber.</param>
	/// <param name="pagesize">The pagesize.</param>
	/// <returns></returns>
	public static IQueryable<T> PageBy<T>(this IQueryable<T> items,
        int pagenumber, int pagesize)
    {
        return items
            .Skip((pagenumber - 1) * pagesize)
            .Take(pagesize);
    }
}