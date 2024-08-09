using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addeddoctorslotsandappointmenttables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorTimeSlots",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 256, nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    SlotFrom = table.Column<TimeSpan>(nullable: false),
                    SlotTo = table.Column<TimeSpan>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    LockedTime = table.Column<DateTime>(nullable: true),
                    IsSlotAvailable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorTimeSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TimeSlotId = table.Column<long>(nullable: false),
                    DoctorId = table.Column<string>(maxLength: 256, nullable: false),
                    PatientId = table.Column<string>(maxLength: 256, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_DoctorTimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "DoctorTimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorTimeSlots");
        }
    }
}
