using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer
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