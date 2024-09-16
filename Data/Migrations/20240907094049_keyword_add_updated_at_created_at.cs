using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFAMA.Data.Migrations
{
    public partial class keyword_add_updated_at_created_at : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.RenameColumn(
                name: "Update_at",
                table: "Keyword",
                newName: "Updated_at");

            migrationBuilder.AlterColumn<string>(
                name: "Keyword",
                table: "Keyword",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_at",
                table: "Keyword",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "Created_at",
                table: "Keyword");

            migrationBuilder.RenameColumn(
                name: "Updated_at",
                table: "Keyword",
                newName: "Update_at");

            migrationBuilder.AlterColumn<string>(
                name: "Keyword",
                table: "Keyword",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
            */
        }
    }
}
