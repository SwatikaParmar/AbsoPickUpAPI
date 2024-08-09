using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class changedDatatypeForStrengthinMedications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Strength",
                table: "Medications",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Strength",
                table: "Medications",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
