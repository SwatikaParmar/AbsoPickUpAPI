using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedSpecialityImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "SpecialityMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "SpecialityMaster");
        }
    }
}
