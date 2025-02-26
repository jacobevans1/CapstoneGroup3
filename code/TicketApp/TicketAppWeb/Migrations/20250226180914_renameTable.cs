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
            migrationBuilder.RenameTable(
               name: "GroupProject", newName: "ProjectGroups"
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
            name: "ProjectGroups", newName: "GroupProject"
            );
        }
    }
}
