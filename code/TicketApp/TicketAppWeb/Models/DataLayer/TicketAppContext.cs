using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Models.DataLayer
{
	/// <summary>
	/// The TicketAppContext class represents the database context for the TicketApp application.
	/// </summary>
	public class TicketAppContext : IdentityDbContext<TicketAppUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TicketAppContext"/> class using the specified options.
		/// </summary>
		/// <param name="options">The options to be used by the DbContext.</param>
		public TicketAppContext(DbContextOptions<TicketAppContext> options) : base(options) { }

		/// <summary>
		/// Gets or sets the DbSet representing the Users table in the database.
		/// </summary>
		public DbSet<TicketAppUser> Users { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the AvailableRoles table in the database.
		/// </summary>
		public DbSet<IdentityRole> Roles { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Projects table in the database.
		/// </summary>
		public DbSet<Project> Projects { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Groups table in the database.
		/// </summary>
		public DbSet<Group> Groups { get; set; }

		/// <summary>
		/// Gets or sets the group approval requests.
		/// </summary>
		public DbSet<GroupApprovalRequest> GroupApprovalRequests { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Boards table in the database.
		/// </summary>
		public DbSet<Board> Boards { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Tickets table in the database.
		/// </summary>
		public DbSet<Ticket> Tickets { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the TicketComments table in the database.
		/// </summary>
		public DbSet<Status> Statuses { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the TicketAssignees table in the database.
		/// </summary>
		public DbSet<TicketAssignee> TicketAssignees { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the BoardStatus table in the database.
		/// </summary>
		public DbSet<BoardStatus> BoardStatus { get; set; }

		/// <summary>
		/// Configures the model for the context.
		/// </summary>
		/// <param name="modelBuilder">The builder used to construct the model for this context.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Many-to-Many Relationship: Groups <-> Members (Users)
			modelBuilder.Entity<Group>()
				.HasMany(g => g.Members)
				.WithMany(u => u.Groups)
				.UsingEntity<Dictionary<string, object>>(
					"GroupUser",
					j => j.HasOne<TicketAppUser>().WithMany().HasForeignKey("MemberId"),
					j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
					j =>
					{
						j.HasKey("GroupId", "MemberId");
						j.ToTable("GroupUser");
					}
				);

			// One-to-Many Relationship: Group <-> Manager (User)
			modelBuilder.Entity<Group>()
				.HasOne(g => g.Manager)
				.WithMany()
				.HasForeignKey(g => g.ManagerId)
				.OnDelete(DeleteBehavior.Restrict);




			// BoardStatus Configuration
			modelBuilder.Entity<BoardStatus>()
				.HasKey(bs => new { bs.BoardId, bs.StatusId });

			modelBuilder.Entity<BoardStatus>()
				.HasOne(bs => bs.Board)
				.WithMany()
				.HasForeignKey(bs => bs.BoardId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<BoardStatus>()
				.HasOne(bs => bs.Status)
				.WithMany()
				.HasForeignKey(bs => bs.StatusId)
				.OnDelete(DeleteBehavior.Cascade);




			// TicketAssignee Configuration
			modelBuilder.Entity<TicketAssignee>()
				.HasKey(ta => new { ta.TicketId, ta.UserId });

			modelBuilder.Entity<TicketAssignee>()
				.HasOne(ta => ta.Ticket)
				.WithMany()
				.HasForeignKey(ta => ta.TicketId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TicketAssignee>()
				.HasOne(u => u.User)
				.WithMany()
				.HasForeignKey(u => u.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}


		/// <summary>
		/// Configures the database options for the context.
		/// </summary>
		/// <param name="optionsBuilder">The builder used to configure the database options.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TicketAppDB;Trusted_Connection=True;MultipleActiveResultSets=true");
		}
	}
}