using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class updateschedulestableswithdeletedfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DoctorTimeSlots",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "DoctorTimeSlots",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "DoctorTimeSlots",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DoctorTimeSlots",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "DoctorTimeSlots",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DoctorSchedule",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "DoctorSchedule",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "DoctorSchedule",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DoctorSchedule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "DoctorSchedule",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Appointments",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Appointments",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Appointments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DoctorTimeSlots");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "DoctorTimeSlots");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "DoctorTimeSlots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DoctorTimeSlots");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "DoctorTimeSlots");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DoctorSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "DoctorSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "DoctorSchedule");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DoctorSchedule");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "DoctorSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Appointments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
