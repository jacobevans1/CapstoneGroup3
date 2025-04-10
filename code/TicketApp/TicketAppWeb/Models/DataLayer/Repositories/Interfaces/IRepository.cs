﻿namespace TicketAppWeb.Models.DataLayer.Repositories.Interfaces;

/// <summary>
/// The IRepository interface defines the methods that must be implemented by all repository classes.
/// Jabesi Abwe
/// 02/019/2025
/// </summary>
public interface IRepository<T> where T : class
{
	IEnumerable<T> List(QueryOptions<T> options);
	int Count { get; }
	T? Get(QueryOptions<T> options);
	T? Get(int id);
	T? Get(string id);
	void Insert(T entity);
	void Update(T entity);
	void Delete(T entity);
	void Save();
}