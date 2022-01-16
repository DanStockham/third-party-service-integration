using Microsoft.EntityFrameworkCore.Migrations;

namespace CUNAMUTUAL_TAKEHOME.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequests");
        }
    }
}
