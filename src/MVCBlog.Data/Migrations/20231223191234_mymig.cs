using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCBlog.Data.Migrations
{
    public partial class mymig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WrapUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstCommittedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstCommittedDateMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommittedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommittedDateCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinCommittedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinCommittedDateCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MergedPRCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommittedDateByDeveloper = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommittedDateCountByDeveloper = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PushCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedLineCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedLineCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrapUps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrapUps");
        }
    }
}
