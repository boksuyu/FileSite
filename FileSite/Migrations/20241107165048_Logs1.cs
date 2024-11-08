using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileSite.Migrations
{
    /// <inheritdoc />
    public partial class Logs1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Entry = table.Column<string>(type: "text", nullable: false),
                    Service = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileActionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileHash = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ActionTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ParentLogId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileActionLogs_FileActionLogs_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "FileActionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileActionLogs_ServiceLogs_ParentLogId",
                        column: x => x.ParentLogId,
                        principalTable: "ServiceLogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileActionLogs_ActionTypeId",
                table: "FileActionLogs",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileActionLogs_ParentLogId",
                table: "FileActionLogs",
                column: "ParentLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileActionLogs");

            migrationBuilder.DropTable(
                name: "ServiceLogs");
        }
    }
}
