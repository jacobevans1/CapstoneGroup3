using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAppWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddProjectAndGroupTables : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
	   name: "Groups",
	   columns: table => new
	   {
		   Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
		   GroupName = table.Column<string>(type: "nvarchar(50)", nullable: false),
		   Description = table.Column<string>(type: "nvarchar(50)", nullable: true),
		   ManagerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
		   CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
	   },
	   constraints: table =>
	   {
		   table.PrimaryKey("PK_Groups", x => x.Id);
		   table.ForeignKey(
			   name: "FK_Groups_AspNetUsers_ManagerId",
			   column: x => x.ManagerId,
			   principalTable: "AspNetUsers",
			   principalColumn: "Id");
	   });

			migrationBuilder.CreateTable(
				name: "Projects",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProjectName = table.Column<string>(type: "nvarchar(50)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(50)", nullable: true),
					LeadId = table.Column<string>(type: "nvarchar(450)", nullable: true),
					CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Projects", x => x.Id);
					table.ForeignKey(
						name: "FK_Projects_AspNetUsers_LeadId",
						column: x => x.LeadId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Projects_AspNetUsers_CreatedById",
						column: x => x.CreatedById,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "UserGroups",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserGroups", x => new { x.UserId, x.GroupId });
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

			migrationBuilder.CreateTable(
				name: "ProjectGroups",
				columns: table => new
				{
					ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProjectGroups", x => new { x.ProjectId, x.GroupId });
					table.ForeignKey(
						name: "FK_ProjectGroups_Projects_ProjectId",
						column: x => x.ProjectId,
						principalTable: "Projects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProjectGroups_Groups_GroupId",
						column: x => x.GroupId,
						principalTable: "Groups",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
