using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedtableappointmentmediasession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentMediaSession",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AppointmentId = table.Column<string>(maxLength: 450, nullable: false),
                    DoctorId = table.Column<string>(maxLength: 450, nullable: false),
                    PatientId = table.Column<string>(maxLength: 450, nullable: false),
                    SessionId = table.Column<string>(maxLength: 450, nullable: false),
                    connectionId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: false),
                    ExpireTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SessionDetails = table.Column<string>(nullable: true),
                    SessionStatus = table.Column<int>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    CompletedTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMediaSession", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentMediaSession");
        }
    }
}
