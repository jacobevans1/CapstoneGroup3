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
					CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Stage = table.Column<string>(type: "nvarchar(50)", nullable: false),
					IsComplete = table.Column<bool>(type: "bit", nullable: false),
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
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "BoardStages");
			migrationBuilder.DropTable(name: "Stages");
			migrationBuilder.DropTable(name: "Tickets");
			migrationBuilder.DropTable(name: "Boards");
		}
	}
}
