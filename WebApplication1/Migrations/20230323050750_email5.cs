using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class email5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailLogDB",
                table: "EmailLogDB");

            migrationBuilder.RenameTable(
                name: "EmailLogDB",
                newName: "EmailLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs");

            migrationBuilder.RenameTable(
                name: "EmailLogs",
                newName: "EmailLogDB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailLogDB",
                table: "EmailLogDB",
                column: "Id");
        }
    }
}
