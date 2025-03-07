using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class addsNewTableGAP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValue: "Pending"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupApprovalRequests_Projects",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupApprovalRequests_Groups",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupApprovalRequests_ProjectId",
                table: "GroupApprovalRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupApprovalRequests_GroupId",
                table: "GroupApprovalRequests",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
