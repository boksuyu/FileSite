using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileSite.Migrations
{
    public partial class FileDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreationDate",
                table: "FileDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "FileDatas");
        }
    }
}
