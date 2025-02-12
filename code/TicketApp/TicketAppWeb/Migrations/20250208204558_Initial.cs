using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Numbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numbers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Numbers",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, 10.0 },
                    { 2, 82.0 },
                    { 3, 33.0 },
                    { 4, 400.0 },
                    { 5, 65.0 },
                    { 6, 2.0 },
                    { 7, 720.0 },
                    { 8, 90.0 },
                    { 9, 23.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Numbers");
        }
    }
}
