using System.Linq.Expressions;

namespace TicketAppWeb.Models.DataLayer;

/// <summary>
/// The QueryOptions class provides properties for sorting, filtering, and paging query results.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
public class QueryOptions<T>
{
	/// <summary>
	/// Gets or sets the order by.
	/// </summary>
	public Expression<Func<T, Object>> OrderBy { get; set; } = null!;

	/// <summary>
	/// Gets or sets the where.
	/// </summary>
	public Expression<Func<T, bool>> Where { get; set; } = null!;

	/// <summary>
	/// Gets or sets the order by direction.
	/// </summary>
	public string OrderByDirection { get; set; } = "asc";

	/// <summary>
	/// Gets or sets the page number.
	/// </summary>
	public int PageNumber { get; set; }

	/// <summary>
	/// Gets or sets the size of the page.
	/// </summary>
	public int PageSize { get; set; }

	/// <summary>
	/// The includes
	/// </summary>
	private string[] includes = Array.Empty<string>();

	/// <summary>
	/// Sets the includes.
	/// </summary>
	public string Includes
    {
        set => includes = value.Replace(" ", "").Split(',');
    }

	/// <summary>
	/// public get method for Include strings - returns private string array, or
	/// empty string array if private backing field is null
	/// </summary>
	public string[] GetIncludes() => includes;

	/// <summary>
	/// Gets a value indicating whether this instance has where.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance has where; otherwise, <c>false</c>.
	/// </value>
	public bool HasWhere => Where != null;

	/// <summary>
	/// Gets a value indicating whether this instance has order by.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance has order by; otherwise, <c>false</c>.
	/// </value>
	public bool HasOrderBy => OrderBy != null;

	/// <summary>
	/// Gets a value indicating whether this instance has paging.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance has paging; otherwise, <c>false</c>.
	/// </value>
	public bool HasPaging => PageNumber > 0 && PageSize > 0;
}