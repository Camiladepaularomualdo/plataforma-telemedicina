using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemedicina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingUrlToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingUrl",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingUrl",
                table: "Appointments");
        }
    }
}
