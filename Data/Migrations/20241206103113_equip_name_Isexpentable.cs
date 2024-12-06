using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFAMA.Data.Migrations
{
    public partial class equip_name_Isexpentable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isExpandable",
                table: "Equipment",
                newName: "IsExpandable");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Borrow",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsExpandable",
                table: "Equipment",
                newName: "isExpandable");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Borrow",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
