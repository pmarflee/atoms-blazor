using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atoms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LocalStorageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ColourScheme = table.Column<int>(type: "INTEGER", nullable: false),
                    AtomShape = table.Column<int>(type: "INTEGER", nullable: false),
                    Move = table.Column<int>(type: "INTEGER", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Board_Data = table.Column<string>(type: "TEXT", nullable: false),
                    Rng_Iterations = table.Column<int>(type: "INTEGER", nullable: false),
                    Rng_Seed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalStorageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    IsWinner = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LocalStorageId",
                table: "Games",
                column: "LocalStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
