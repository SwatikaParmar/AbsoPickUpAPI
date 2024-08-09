using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedtablesprescriptionslabrecomendations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PrescriptionId = table.Column<string>(maxLength: 450, nullable: false),
                    MedicineName = table.Column<string>(maxLength: 200, nullable: false),
                    MedicineType = table.Column<string>(maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Strength = table.Column<int>(nullable: false),
                    StrengthUnitOfMeasurement = table.Column<string>(nullable: false),
                    Usage = table.Column<string>(maxLength: 500, nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Type = table.Column<string>(maxLength: 256, nullable: false),
                    Quantity = table.Column<string>(maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientLabRecommendations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PrescriptionId = table.Column<string>(maxLength: 450, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientLabRecommendations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PatientId = table.Column<string>(maxLength: 450, nullable: false),
                    DoctorId = table.Column<string>(maxLength: 450, nullable: false),
                    AppointmentId = table.Column<string>(maxLength: 450, nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "MedicineMaster");

            migrationBuilder.DropTable(
                name: "PatientLabRecommendations");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
