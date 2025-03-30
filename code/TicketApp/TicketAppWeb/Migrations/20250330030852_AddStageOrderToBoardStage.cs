using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddStageOrderToBoardStage : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "StageOrder",
				table: "BoardStages",
				type: "int",
				nullable: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "StageOrder",
				table: "BoardStages");
		}
	}
}
