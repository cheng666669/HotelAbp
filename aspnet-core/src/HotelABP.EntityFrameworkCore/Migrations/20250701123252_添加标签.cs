using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 添加标签 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "HotelABPLabels");

            migrationBuilder.CreateTable(
                name: "HotelABPLabelss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LabelName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TagType = table.Column<int>(type: "int", nullable: true),
                    MustMeetAllConditions = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StartTime = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TradeTime = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MemberLevel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MemberGender = table.Column<int>(type: "int", nullable: true),
                    TradeCountMin = table.Column<int>(type: "int", nullable: true),
                    TradeCountMax = table.Column<int>(type: "int", nullable: true),
                    AvgOrderValueMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    AvgOrderValueMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TotalSpentMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TotalSpentMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelABPLabelss", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelABPLabelss");

            //migrationBuilder.CreateTable(
            //    name: "HotelABPLabels",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        AvgOrderValueMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
            //        AvgOrderValueMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
            //        LabelName = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //        MemberGender = table.Column<int>(type: "int", nullable: true),
            //        MemberLevel = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        MustMeetAllConditions = table.Column<bool>(type: "tinyint(1)", nullable: false),
            //        StartTime = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        TagType = table.Column<int>(type: "int", nullable: true),
            //        TotalSpentMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
            //        TotalSpentMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
            //        TradeCountMax = table.Column<int>(type: "int", nullable: true),
            //        TradeCountMin = table.Column<int>(type: "int", nullable: true),
            //        TradeTime = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HotelABPLabels", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
