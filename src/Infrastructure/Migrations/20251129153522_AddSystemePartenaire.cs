using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemePartenaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemesPartenaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UrlBase = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UsernameAppel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PasswordChiffre = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UsernameAcces = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PasswordHashAcces = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EstAppelant = table.Column<bool>(type: "INTEGER", nullable: false),
                    EstAppele = table.Column<bool>(type: "INTEGER", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemesPartenaires", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemesPartenaires_UsernameAcces",
                table: "SystemesPartenaires",
                column: "UsernameAcces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemesPartenaires");
        }
    }
}
