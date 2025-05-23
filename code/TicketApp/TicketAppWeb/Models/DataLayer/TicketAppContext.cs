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
		/// Gets or sets the DbSet representing the AssignedTickets table in the database.
		/// </summary>
		public DbSet<Ticket> Tickets { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Boards table in the database.
		/// </summary>
		public DbSet<Board> Boards { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the BoardStage table in the database.
		/// </summary>
		public DbSet<BoardStage> BoardStages { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the Stages table in the database.
		/// </summary>
		public DbSet<Stage> Stages { get; set; }

		/// <summary>
		/// Gets or sets the DbSet representing the BoardStageGroup table in the database.
		/// </summary>
		public DbSet<BoardStageGroup> BoardStageGroups { get; set; }

        public DbSet<TicketHistory> TicketHistories { get; set; }


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


			// BoardStage Configuration
			modelBuilder.Entity<BoardStage>()
				.HasKey(bs => new { bs.BoardId, bs.StageId });

			modelBuilder.Entity<BoardStage>()
				.HasOne(bs => bs.Board)
				.WithMany(b => b.BoardStages)
				.HasForeignKey(bs => bs.BoardId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<BoardStage>()
				.HasOne(bs => bs.Stage)
				.WithMany(s => s.BoardStages)
				.HasForeignKey(bs => bs.StageId)
				.OnDelete(DeleteBehavior.Cascade);


			// BoardStageGroups Configuration
			modelBuilder.Entity<BoardStageGroup>()
				.HasKey(bsg => bsg.Id);

			modelBuilder.Entity<BoardStageGroup>()
				.HasOne(bsg => bsg.BoardStage)
				.WithMany()
				.HasForeignKey(bsg => new { bsg.BoardId, bsg.StageId })
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<BoardStageGroup>()
				.HasOne(bsg => bsg.Group)
				.WithMany()
				.HasForeignKey(bsg => bsg.GroupId)
				.OnDelete(DeleteBehavior.Cascade);

			// TicketHistory Configuration
            modelBuilder.Entity<TicketHistory>()
            .HasOne(th => th.ChangedByUser)
            .WithMany()
            .HasForeignKey(th => th.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketHistory>()
                .HasOne<Ticket>()
                .WithMany(t => t.History)
                .HasForeignKey(th => th.TicketId)
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