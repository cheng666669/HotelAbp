using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 积分表新增字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Accumulativeintegral",
                table: "HotelABPCustoimers",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pointsmodifydesc",
                table: "HotelABPCustoimers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accumulativeintegral",
                table: "HotelABPCustoimers");

            migrationBuilder.DropColumn(
                name: "Pointsmodifydesc",
                table: "HotelABPCustoimers");
        }
    }
}
