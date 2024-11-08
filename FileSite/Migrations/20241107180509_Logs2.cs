using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileSite.Migrations
{
    /// <inheritdoc />
    public partial class Logs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileActionLogs_FileActionLogs_ActionTypeId",
                table: "FileActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_FileActionLogs_ActionTypeId",
                table: "FileActionLogs");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "FileActionLogs");

            migrationBuilder.AlterColumn<string>(
                name: "FileHash",
                table: "FileActionLogs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "FileActionLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "FileActionLogs");

            migrationBuilder.AlterColumn<string>(
                name: "FileHash",
                table: "FileActionLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<long>(
                name: "ActionTypeId",
                table: "FileActionLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_FileActionLogs_ActionTypeId",
                table: "FileActionLogs",
                column: "ActionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileActionLogs_FileActionLogs_ActionTypeId",
                table: "FileActionLogs",
                column: "ActionTypeId",
                principalTable: "FileActionLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
