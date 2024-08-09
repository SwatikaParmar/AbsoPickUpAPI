using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedUserPaymentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPaymentInfo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PatientId = table.Column<string>(maxLength: 256, nullable: false),
                    DoctorId = table.Column<string>(maxLength: 256, nullable: false),
                    AppointmentId = table.Column<string>(maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    PaymentMode = table.Column<string>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false),
                    TransactionId = table.Column<string>(nullable: false),
                    CustomerId = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPaymentInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPaymentInfo");
        }
    }
}
