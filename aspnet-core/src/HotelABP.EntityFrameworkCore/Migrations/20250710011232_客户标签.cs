using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 客户标签 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerLabel",
                table: "HotelABPCustoimerss",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerLabel",
                table: "HotelABPCustoimerss");
        }
    }
}
