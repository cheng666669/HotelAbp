using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 客户添加字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerDesc",
                table: "HotelABPCustoimers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Rechargeamount",
                table: "HotelABPCustoimers",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sumofconsumption",
                table: "HotelABPCustoimers",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerDesc",
                table: "HotelABPCustoimers");

            migrationBuilder.DropColumn(
                name: "Rechargeamount",
                table: "HotelABPCustoimers");

            migrationBuilder.DropColumn(
                name: "Sumofconsumption",
                table: "HotelABPCustoimers");
        }
    }
}
