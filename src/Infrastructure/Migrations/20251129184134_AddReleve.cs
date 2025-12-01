using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReleve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Releves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SondeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valeur = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DateHeure = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeReleve = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releves_Devices_SondeId",
                        column: x => x.SondeId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Releves_SondeId_DateHeure",
                table: "Releves",
                columns: new[] { "SondeId", "DateHeure" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Releves");
        }
    }
}
