using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TaskAppWeb.Data
{
	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options)
		: base(options)
		{ }

		public DbSet<Object> Values { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder) {}
	}
}
