using System.Linq.Expressions;

namespace TicketAppWeb.Models.DataLayer;

/// <summary>
/// The QueryOptions class provides properties for sorting, filtering, and paging query results.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
public class QueryOptions<T>
{
    // public properties for sorting, filtering, and paging
    public Expression<Func<T, Object>> OrderBy { get; set; } = null!;
    public Expression<Func<T, bool>> Where { get; set; } = null!;
    public string OrderByDirection { get; set; } = "asc";  // default
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    // read-only properties 
    public bool HasWhere => Where != null;
    public bool HasOrderBy => OrderBy != null;
    public bool HasPaging => PageNumber > 0 && PageSize > 0;
}