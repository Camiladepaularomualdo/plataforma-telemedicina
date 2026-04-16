using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemedicina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditRenewalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRenewalDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRenewalDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanCredits",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRenewalDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "NextRenewalDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PlanCredits",
                table: "Doctors");
        }
    }
}
