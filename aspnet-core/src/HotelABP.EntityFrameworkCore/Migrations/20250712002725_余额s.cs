using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 余额s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Balancerecord");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Balancerecord");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Balancerecord");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Balancerecord");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "Balancerecord");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Balancerecord",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Balancerecord",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Balancerecord",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Balancerecord",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "Balancerecord",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }
    }
}
