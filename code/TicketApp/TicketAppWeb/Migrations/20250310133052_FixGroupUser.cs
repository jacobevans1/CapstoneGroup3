using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class FixGroupUser : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "UserGroups");

			migrationBuilder.CreateTable(
				name: "GroupUser",
				columns: table => new
				{
					GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_GroupUser", x => new { x.GroupId, x.MemberId });
					table.ForeignKey(
						name: "FK_GroupUser_AspNetUsers_MemberId",
						column: x => x.MemberId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_GroupUser_Groups_GroupId",
						column: x => x.GroupId,
						principalTable: "Groups",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_GroupUser_MemberId",
				table: "GroupUser",
				column: "MemberId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "GroupUser");

			migrationBuilder.CreateTable(
				name: "UserGroups",
				columns: table => new
				{
					GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserGroups", x => new { x.GroupId, x.UserId });
					table.ForeignKey(
						name: "FK_UserGroups_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_UserGroups_Groups_GroupId",
						column: x => x.GroupId,
						principalTable: "Groups",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_UserGroups_UserId",
				table: "UserGroups",
				column: "UserId");
		}
	}
}
