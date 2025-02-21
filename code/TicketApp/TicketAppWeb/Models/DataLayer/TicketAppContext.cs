using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer
{
	public class TicketAppContext : IdentityDbContext<TicketAppUser>
	{
		public TicketAppContext(DbContextOptions<TicketAppContext> options) : base(options) { }

		public DbSet<TicketAppUser> Users { get; set; }
		public DbSet<IdentityRole> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}