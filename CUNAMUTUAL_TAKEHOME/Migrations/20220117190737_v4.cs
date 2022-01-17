using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CUNAMUTUAL_TAKEHOME.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "ServiceItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateAt",
                table: "ServiceItems",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "LastUpdateAt",
                table: "ServiceItems");
        }
    }
}
