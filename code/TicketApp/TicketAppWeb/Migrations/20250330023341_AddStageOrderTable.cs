using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddStageOrderTable : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Create BoardStageOrders Table
			migrationBuilder.CreateTable(
				name: "BoardStageOrders",
				columns: table => new
				{
					BoardId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					StageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					StageOrder = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BoardStageOrders", x => new { x.BoardId, x.StageId });
					table.ForeignKey(
						name: "FK_BoardStageOrders_Boards",
						column: x => x.BoardId,
						principalTable: "Boards",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_BoardStageOrders_Stages",
						column: x => x.StageId,
						principalTable: "Stages",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_BoardStageOrders_StageId",
				table: "BoardStageOrders",
				column: "StageId");

			migrationBuilder.CreateIndex(
				name: "IX_BoardStageOrders_BoardId_StageId",
				table: "BoardStageOrders",
				columns: new[] { "BoardId", "StageId" },
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "BoardStageOrders");
		}
	}
}
