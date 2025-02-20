using Microsoft.AspNetCore.Identity;
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

			// Admin user
			var adminUser = new TicketAppUser
			{
				Id = "b74ddd14-6340-4840-95c2-db12554843e5",
				UserName = "admin",
				NormalizedUserName = "ADMIN",
				Email = "admin@example.com",
				NormalizedEmail = "ADMIN@EXAMPLE.COM",
				EmailConfirmed = true,
				FirstName = "System",
				LastName = "Administrator",
				SecurityStamp = "A1B2C3D4E5F6G7H8I9J0",
				PasswordHash = "AQAAAAIAAYagAAAAECxosoODgz+v/No4ynkryfzVjdi9+zi6EzYlKJ91Vgn1SuRJYnpgNdZAcgZnXcEQMw==",
				ConcurrencyStamp = "93ba2f22-16e5-4753-99cc-6cab07467e79",
			};

			modelBuilder.Entity<TicketAppUser>().HasData(adminUser);


			// Admin role
			var adminRole = new IdentityRole
			{
				Id = "1b1c2d3e-4f5g-678h-91ij-23456789ab5d",
				Name = "Admin",
				NormalizedName = "ADMIN",
			};

			modelBuilder.Entity<IdentityRole>().HasData(adminRole);


			// Admin user role
			modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
			{
				UserId = adminUser.Id,
				RoleId = adminRole.Id,
			});
		}
	}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}