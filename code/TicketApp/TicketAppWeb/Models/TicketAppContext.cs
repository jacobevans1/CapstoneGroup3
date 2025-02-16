using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TicketAppWeb.Models
{
	public class TicketAppContext : IdentityDbContext<TicketAppUser>
	{
		public TicketAppContext(DbContextOptions<TicketAppContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}