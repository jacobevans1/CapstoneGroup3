using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;

namespace TicketAppWeb.Models.DataLayer.Repositories;

/// <summary>
/// The Repository class implements the IRepository interface and provides a generic implementation of the methods.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
	protected TicketAppContext context { get; set; }
	private DbSet<T> dbset { get; set; }

	/// <summary>
	/// Initializes a new instance of the Repository{T} class.
	/// </summary>
	/// <param name="ctx">The CTX.</param>
	public Repository(TicketAppContext ctx)
	{
		context = ctx;
		dbset = context.Set<T>();
	}

	/// <summary>
	/// Gets the count of x entity from the database
	/// </summary>
	public int Count => dbset.Count();

	/// <summary>
	/// Returns the lists of x entity specified by options.
	/// </summary>
	/// <param name="options">The options.</param>
	public virtual IEnumerable<T> List(QueryOptions<T> options) =>
		BuildQuery(options).ToList();

	/// <summary>
	/// Gets the entity by specified identifier (int type).
	/// </summary>
	/// <param name="id">The identifier.</param>
	public virtual T? Get(int id) => dbset.Find(id);

	/// <summary>
	/// Gets the entity by specified identifier (string type).
	/// </summary>
	/// <param name="id">The identifier.</param>
	public virtual T? Get(string id) => dbset.Find(id);

	/// <summary>
	/// Gets the entiy with specified options.
	/// </summary>
	/// <param name="options">The options.</param>
	public virtual T? Get(QueryOptions<T> options) =>
		BuildQuery(options).FirstOrDefault();

	/// <summary>
	/// Inserts the specified entity in the database.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public virtual void Insert(T entity) => dbset.Add(entity);

	/// <summary>
	/// Updates the specified entity in the database.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public virtual void Update(T entity) => dbset.Update(entity);

	/// <summary>
	/// Deletes the specified entity from the database
	/// </summary>
	/// <param name="entity">The entity.</param>
	public virtual void Delete(T entity) => dbset.Remove(entity);

	/// <summary>
	/// Saves the changes made to the database
	/// </summary>
	public virtual void Save() => context.SaveChanges();

	/// <summary>
	/// private helper method to build query expression
	/// </summary>
	/// <param name="options">The options.</param>
	private IQueryable<T> BuildQuery(QueryOptions<T> options)
	{
		IQueryable<T> query = dbset;
		if (options.HasWhere)
		{
			query = query.Where(options.Where);
		}
		if (options.HasOrderBy)
		{
			if (options.OrderByDirection == "asc")
				query = query.OrderBy(options.OrderBy);
			else
				query = query.OrderByDescending(options.OrderBy);
		}
		if (options.HasPaging)
		{
			query = query.PageBy(options.PageNumber, options.PageSize);
		}

		return query;
	}
}