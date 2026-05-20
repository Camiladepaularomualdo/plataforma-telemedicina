using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemedicina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Patients");
        }
    }
}
