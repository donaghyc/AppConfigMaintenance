using Microsoft.EntityFrameworkCore.Migrations;

namespace AppConfigMaintenance.DataAccess.Migrations
{
    public partial class MakeNameUniqueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigSettings",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigSettings_Name",
                table: "ConfigSettings",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConfigSettings_Name",
                table: "ConfigSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigSettings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
