namespace TicketAppWeb.Models.DataLayer.Reposetories;

/// <summary>
/// The IRepository interface defines the methods that must be implemented by all repository classes.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    IEnumerable<T> List(QueryOptions<T> options);

    int Count { get; }  // read-only property

    // overloaded Get() method
    T? Get(QueryOptions<T> options);
    T? Get(int id);
    T? Get(string id);

    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);

    void Save();
}