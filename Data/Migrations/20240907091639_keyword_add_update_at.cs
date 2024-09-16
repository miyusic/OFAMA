using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFAMA.Data.Migrations
{
    public partial class keyword_add_update_at : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "Keyword",
                newName: "Update_at");
            */
            migrationBuilder.AddColumn<DateTime>(
                        name: "Created_at",
                        table: "Keyword",
                        type: "datetime2",
                        nullable: false,
                        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.RenameColumn(
                name: "Update_at",
                table: "Keyword",
                newName: "Created_at");
            */
            migrationBuilder.DropColumn(
              name: "Created_at",
              table: "Keyword");
        }
    }
}
