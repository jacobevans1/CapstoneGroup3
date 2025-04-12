using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class renameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename columns first (while the table is still named "ProjectGroups")
            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "ProjectGroups",
                newName: "ProjectsId"
            );

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "ProjectGroups",
                newName: "GroupsId"
            );

            // Rename table
            migrationBuilder.RenameTable(
                name: "ProjectGroups",
                newName: "GroupProject"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert table name first (so the column renaming happens on the correct table)
            migrationBuilder.RenameTable(
                name: "GroupProject",
                newName: "ProjectGroups"
            );

            // Revert column names
            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "ProjectGroups",
                newName: "ProjectId"
            );

            migrationBuilder.RenameColumn(
                name: "GroupsId",
                table: "ProjectGroups",
                newName: "GroupId"
            );
        }
    }
}
