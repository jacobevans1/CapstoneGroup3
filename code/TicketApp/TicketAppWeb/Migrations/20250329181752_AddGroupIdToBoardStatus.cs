using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddGroupIdToBoardStatus : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "GroupId",
				table: "BoardStages",
				type: "nvarchar(450)",
				nullable: true,
				defaultValue: "");

			migrationBuilder.CreateIndex(
				name: "IX_BoardStatuses_GroupId",
				table: "BoardStages",
				column: "GroupId");

			migrationBuilder.AddForeignKey(
				name: "FK_BoardStatuses_Groups_GroupId",
				table: "BoardStages",
				column: "GroupId",
				principalTable: "Groups",
				principalColumn: "Id",
				onDelete: ReferentialAction.SetNull);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_BoardStatuses_Groups_GroupId",
				table: "BoardStages");

			migrationBuilder.DropIndex(
				name: "IX_BoardStatuses_GroupId",
				table: "BoardStages");

			migrationBuilder.DropColumn(
				name: "GroupId",
				table: "BoardStages");
		}
	}
}
