using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LocalisationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateInstallation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    CanalCommunication = table.Column<int>(type: "INTEGER", nullable: false),
                    UrlDevice = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CredentialsDevice = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    TypeActionneur = table.Column<int>(type: "INTEGER", nullable: true),
                    TypeSonde = table.Column<int>(type: "INTEGER", nullable: true),
                    UniteMesureId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Localisations_LocalisationId",
                        column: x => x.LocalisationId,
                        principalTable: "Localisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_UnitesMesures_UniteMesureId",
                        column: x => x.UniteMesureId,
                        principalTable: "UnitesMesures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LocalisationId",
                table: "Devices",
                column: "LocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UniteMesureId",
                table: "Devices",
                column: "UniteMesureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
