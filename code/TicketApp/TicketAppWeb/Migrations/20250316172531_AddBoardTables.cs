using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddBoardTables : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Create Boards Table
			migrationBuilder.CreateTable(
				name: "Boards",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					BoardName = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(50)", nullable: true),
					ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Boards", x => x.Id);
					table.ForeignKey(
						name: "FK_Boards_Projects",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade
					);
				});

			// Create Tickets Table
			migrationBuilder.CreateTable(
				name: "Tickets",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Title = table.Column<string>(type: "nvarchar(50)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(50)", nullable: true),
					Status = table.Column<string>(type: "nvarchar(50)", nullable: false),
					BoardId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Tickets", x => x.Id);
					table.ForeignKey(
						name: "FK_Tickets_Boards",
						column: x => x.BoardId,
						principalTable: "Boards",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			// Create TicketAssignees (Middle Table)
			migrationBuilder.CreateTable(
				name: "TicketAssignees",
				columns: table => new
				{
					TicketId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TicketAssignees", x => new { x.TicketId, x.UserId });
					table.ForeignKey(
						name: "FK_TicketAssignees_Tickets",
						column: x => x.TicketId,
						principalTable: "Tickets",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_TicketAssignees_AspNetUsers",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			// Create Stages Table
			migrationBuilder.CreateTable(
				name: "Stages",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Statuses", x => x.Id);
				});

			// Create BoardStages Table
			migrationBuilder.CreateTable(
				name: "BoardStages",
				columns: table => new
				{
					BoardId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					StageId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BoardStatuses", x => new { x.BoardId, x.StageId });
					table.ForeignKey(
						name: "FK_BoardStatuses_Boards",
						column: x => x.BoardId,
						principalTable: "Boards",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_BoardStatuses_Statuses",
						column: x => x.StageId,
						principalTable: "Stages",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			// Indexes
			migrationBuilder.CreateIndex(
				name: "IX_Tickets_BoardId",
				table: "Tickets",
				column: "BoardId");

			migrationBuilder.CreateIndex(
				name: "IX_TicketAssignees_UserId",
				table: "TicketAssignees",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "BoardStages");
			migrationBuilder.DropTable(name: "Stages");
			migrationBuilder.DropTable(name: "TicketAssignees");
			migrationBuilder.DropTable(name: "Tickets");
			migrationBuilder.DropTable(name: "Boards");
		}
	}
}
