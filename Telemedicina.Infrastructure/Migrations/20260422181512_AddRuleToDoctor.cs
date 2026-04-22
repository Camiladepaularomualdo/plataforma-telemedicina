using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemedicina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRuleToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rule",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "usr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rule",
                table: "Doctors");
        }
    }
}
