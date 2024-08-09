using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedTableForPrescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicineType",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "StrengthUnitOfMeasurement",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "EntitlementNumber",
                table: "Prescriptions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ISPBSSafetyEntitlementCardHolder",
                table: "Prescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBrandNotPermitted",
                table: "Prescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPBSPrescriptionFromStateManager",
                table: "Prescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPBSSafetyConcessionCardHolder",
                table: "Prescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRBPSPrescription",
                table: "Prescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MedicareNumber",
                table: "Prescriptions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Prescriptions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "Medications",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Medications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageDirections",
                table: "Medications",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumberOfRepeats",
                table: "Medications",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntitlementNumber",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "ISPBSSafetyEntitlementCardHolder",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsBrandNotPermitted",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsPBSPrescriptionFromStateManager",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsPBSSafetyConcessionCardHolder",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsRBPSPrescription",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "MedicareNumber",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DosageDirections",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "NumberOfRepeats",
                table: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prescriptions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Prescriptions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Medications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Medications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MedicineType",
                table: "Medications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Strength",
                table: "Medications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StrengthUnitOfMeasurement",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Usage",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
