using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCBlog.Data.Migrations
{
    public partial class mymig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinCommittedDateByDeveloper",
                table: "WrapUps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinCommittedDateCountByDeveloper",
                table: "WrapUps",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinCommittedDateByDeveloper",
                table: "WrapUps");

            migrationBuilder.DropColumn(
                name: "MinCommittedDateCountByDeveloper",
                table: "WrapUps");
        }
    }
}
