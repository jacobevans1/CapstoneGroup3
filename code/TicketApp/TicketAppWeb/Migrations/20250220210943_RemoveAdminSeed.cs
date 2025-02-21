using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class RemoveAdminSeed : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Drop the existing PK
			migrationBuilder.DropPrimaryKey(
				name: "PK_AspNetUserTokens",
				table: "AspNetUserTokens");

			// Alter the column size
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			// Recreate the PK with the modified column
			migrationBuilder.AddPrimaryKey(
				name: "PK_AspNetUserTokens",
				table: "AspNetUserTokens",
				columns: new[] { "UserId", "LoginProvider", "Name" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Drop the modified PK
			migrationBuilder.DropPrimaryKey(
				name: "PK_AspNetUserTokens",
				table: "AspNetUserTokens");

			// Revert the column change back to nvarchar(450)
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "nvarchar(450)",
				maxLength: 450,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			// Restore the original PK
			migrationBuilder.AddPrimaryKey(
				name: "PK_AspNetUserTokens",
				table: "AspNetUserTokens",
				columns: new[] { "UserId", "LoginProvider", "Name" });
		}

	}
}
