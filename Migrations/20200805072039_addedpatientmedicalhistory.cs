using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedpatientmedicalhistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DoctorSpecialityInfo",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "PatientPastMedicalHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 256, nullable: true),
                    TreatmentName = table.Column<string>(maxLength: 50, nullable: true),
                    DoctorName = table.Column<string>(maxLength: 50, nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientPastMedicalHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientPastMedicalHistory");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DoctorSpecialityInfo",
                type: "int",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256);
        }
    }
}
