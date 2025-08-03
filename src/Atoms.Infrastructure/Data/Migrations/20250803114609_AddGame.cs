using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
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
                name: "PlayerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalStorageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    IsWinner = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AbbreviatedName = table.Column<string>(type: "TEXT", nullable: true),
                    InviteCode = table.Column<string>(type: "TEXT", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Players_PlayerTypes_PlayerTypeId",
                        column: x => x.PlayerTypeId,
                        principalTable: "PlayerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PlayerTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Human", "Human" },
                    { 2, "CPU (Easy)", "CPU (E)" },
                    { 3, "CPU (Medium)", "CPU (M)" },
                    { 4, "CPU (Hard)", "CPU (H)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LastUpdatedDateUtc",
                table: "Games",
                column: "LastUpdatedDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LocalStorageId",
                table: "Games",
                column: "LocalStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserId",
                table: "Games",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_IsWinner",
                table: "Players",
                column: "IsWinner");

            migrationBuilder.CreateIndex(
                name: "IX_Players_LocalStorageId",
                table: "Players",
                column: "LocalStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerTypeId",
                table: "Players",
                column: "PlayerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "PlayerTypes");
        }
    }
}
