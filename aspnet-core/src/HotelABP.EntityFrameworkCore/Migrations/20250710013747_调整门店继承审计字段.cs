using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 调整门店继承审计字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "StoreInfo",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "StoreInfo",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "StoreInfo");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "StoreInfo");
        }
    }
}
