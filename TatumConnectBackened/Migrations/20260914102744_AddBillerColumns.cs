using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TatumConnectBackened.Migrations
{
    /// <inheritdoc />
    public partial class AddBillerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Billers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Billers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Billers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Billers");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Billers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Billers");
        }
    }
}
