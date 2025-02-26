using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer
{
	public class TicketAppContext : IdentityDbContext<TicketAppUser>
	{
		public TicketAppContext(DbContextOptions<TicketAppContext> options) : base(options) { }

		public DbSet<Project> Projects { get; set; }
		public DbSet<Group> Groups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TicketAppDB;Trusted_Connection=True;MultipleActiveResultSets=true");
		}
	}
}