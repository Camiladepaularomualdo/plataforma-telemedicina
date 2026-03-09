using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemedicina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGmailConfigToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GmailAddress",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GmailAppPassword",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GmailAddress",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "GmailAppPassword",
                table: "Doctors");
        }
    }
}
