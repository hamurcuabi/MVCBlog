using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCBlog.Data.Migrations
{
    public partial class mymig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastCommittedDate",
                table: "WrapUps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastCommittedDateMessage",
                table: "WrapUps",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCommittedDate",
                table: "WrapUps");

            migrationBuilder.DropColumn(
                name: "LastCommittedDateMessage",
                table: "WrapUps");

        }
    }
}
