using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocalisationId1",
                table: "Devices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Sonde_LocalisationId1",
                table: "Devices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LocalisationId1",
                table: "Devices",
                column: "LocalisationId1");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Sonde_LocalisationId1",
                table: "Devices",
                column: "Sonde_LocalisationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Localisations_LocalisationId1",
                table: "Devices",
                column: "LocalisationId1",
                principalTable: "Localisations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Localisations_Sonde_LocalisationId1",
                table: "Devices",
                column: "Sonde_LocalisationId1",
                principalTable: "Localisations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Localisations_LocalisationId1",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Localisations_Sonde_LocalisationId1",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_LocalisationId1",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_Sonde_LocalisationId1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "LocalisationId1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Sonde_LocalisationId1",
                table: "Devices");
        }
    }
}
