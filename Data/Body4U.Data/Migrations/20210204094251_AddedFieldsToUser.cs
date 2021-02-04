using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Body4U.Data.Migrations
{
    public partial class AddedFieldsToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "IdentityUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "IdentityUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sex",
                table: "IdentityUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "IdentityUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "IdentityUsers");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "IdentityUsers");
        }
    }
}
