using Microsoft.EntityFrameworkCore;

namespace TicketAppWeb.Models
{
	public class TicketAppContext : DbContext
	{
		public TicketAppContext(DbContextOptions<TicketAppContext> options) : base(options) { }

		public DbSet<Number> Numbers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Number>().HasKey(b => b.Id);

			modelBuilder.Entity<Number>().Property(b => b.Value).IsRequired();

			modelBuilder.Entity<Number>().HasData(
				new Number(10) { Id = 1 },
				new Number(82) { Id = 2 },
				new Number(33) { Id = 3 },
				new Number(400) { Id = 4 },
				new Number(65) { Id = 5 },
				new Number(2) { Id = 6 },
				new Number(720) { Id = 7 },
				new Number(90) { Id = 8 },
				new Number(23) { Id = 9 }
			);
		}
	}
}