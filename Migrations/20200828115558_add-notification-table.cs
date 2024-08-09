using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addnotificationtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FromUser = table.Column<string>(maxLength: 450, nullable: false),
                    ToUser = table.Column<string>(maxLength: 450, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Purpose = table.Column<int>(nullable: false),
                    PurposeId = table.Column<string>(maxLength: 450, nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
