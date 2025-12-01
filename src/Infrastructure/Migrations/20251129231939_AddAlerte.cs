using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlerte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SondeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeuilAlerteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeSeuil = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeAlerte = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    DateAcquittement = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateResolution = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertes_Devices_SondeId",
                        column: x => x.SondeId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alertes_SeuilsAlerte_SeuilAlerteId",
                        column: x => x.SeuilAlerteId,
                        principalTable: "SeuilsAlerte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_SeuilAlerteId",
                table: "Alertes",
                column: "SeuilAlerteId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_SondeId_Statut",
                table: "Alertes",
                columns: new[] { "SondeId", "Statut" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertes");
        }
    }
}
