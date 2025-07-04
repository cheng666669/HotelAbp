using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 客户12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "HotelABPCustoimers",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        CustomerNickName = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CustomerType = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        CustomerName = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Gender = table.Column<int>(type: "int", nullable: true),
            //        Birthday = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        City = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        GrowthValue = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //        AvailableBalance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //        AvailableGiftBalance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //        AvailablePoints = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
            //        DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HotelABPCustoimers", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateTable(
            //    name: "ReserveRooms",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        Infomation = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Ordersource = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ReserveName = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Phone = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        BookingNumber = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Sdate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        Edate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        Day = table.Column<int>(type: "int", nullable: false),
            //        RoomTypeid = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        BreakfastNum = table.Column<int>(type: "int", nullable: true),
            //        Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        RoomNum = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Message = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        IdCard = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        NoReservRoom = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
            //        DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReserveRooms", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelABPCustoimers");

            migrationBuilder.DropTable(
                name: "ReserveRooms");
        }
    }
}
