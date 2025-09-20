using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atoms.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInviteCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "Players",
                type: "text",
                nullable: true);
        }
    }
}
