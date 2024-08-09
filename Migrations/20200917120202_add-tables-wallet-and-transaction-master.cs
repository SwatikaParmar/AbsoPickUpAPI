using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addtableswalletandtransactionmaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionMaster",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AppointmentId = table.Column<string>(maxLength: 450, nullable: false),
                    PatientId = table.Column<string>(maxLength: 450, nullable: false),
                    OrderId = table.Column<string>(maxLength: 256, nullable: true),
                    ReferenceId = table.Column<string>(maxLength: 256, nullable: true),
                    PaymentStatus = table.Column<string>(maxLength: 50, nullable: false),
                    TransactionType = table.Column<string>(maxLength: 50, nullable: false),
                    PaymentMode = table.Column<string>(maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletBillingInfo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 256, nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletBillingInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionMaster");

            migrationBuilder.DropTable(
                name: "WalletBillingInfo");
        }
    }
}
