using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeuilAlerte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeuilsAlerte",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SondeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeSeuil = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeAlerte = table.Column<int>(type: "INTEGER", nullable: false),
                    Valeur = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeuilsAlerte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeuilsAlerte_Devices_SondeId",
                        column: x => x.SondeId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeuilsAlerte_SondeId",
                table: "SeuilsAlerte",
                column: "SondeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeuilsAlerte");
        }
    }
}
