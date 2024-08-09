using Microsoft.EntityFrameworkCore.Migrations;

namespace AnfasAPI.Migrations
{
    public partial class addedBankInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorBankInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 256, nullable: false),
                    BankName = table.Column<string>(maxLength: 256, nullable: true),
                    AccountNumber = table.Column<long>(maxLength: 100, nullable: false),
                    RouteNo = table.Column<string>(maxLength: 100, nullable: true),
                    BranchCode = table.Column<string>(maxLength: 50, nullable: true),
                    PostCode = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorBankInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorBankInfo");
        }
    }
}
