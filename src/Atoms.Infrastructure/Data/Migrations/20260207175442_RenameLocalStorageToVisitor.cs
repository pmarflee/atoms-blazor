using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atoms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameLocalStorageToVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_LocalStorageUsers_LocalStorageUserId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_LocalStorageUsers_LocalStorageUserId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "LocalStorageUserId",
                table: "Players",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_LocalStorageUserId",
                table: "Players",
                newName: "IX_Players_VisitorId");

            migrationBuilder.RenameColumn(
                name: "LocalStorageUserId",
                table: "Games",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_LocalStorageUserId",
                table: "Games",
                newName: "IX_Games_VisitorId");

            migrationBuilder.RenameTable(
                name: "LocalStorageUsers",
                newName: "Visitors");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Visitors_VisitorId",
                table: "Games",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Visitors_VisitorId",
                table: "Players",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Visitors_VisitorId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Visitors_VisitorId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "Players",
                newName: "LocalStorageUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_VisitorId",
                table: "Players",
                newName: "IX_Players_LocalStorageUserId");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "Games",
                newName: "LocalStorageUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_VisitorId",
                table: "Games",
                newName: "IX_Games_LocalStorageUserId");

            migrationBuilder.RenameTable(
                name: "Visitors",
                newName: "LocalStorageUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_LocalStorageUsers_LocalStorageUserId",
                table: "Games",
                column: "LocalStorageUserId",
                principalTable: "LocalStorageUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_LocalStorageUsers_LocalStorageUserId",
                table: "Players",
                column: "LocalStorageUserId",
                principalTable: "LocalStorageUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
