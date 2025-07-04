using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelABP.Migrations
{
    /// <inheritdoc />
    public partial class 会员等级 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HotelAbpGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GradeName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_HotelAbpGrades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

        //    migrationBuilder.CreateTable(
        //        name: "HotelABPLabels",
        //        columns: table => new
        //        {
        //            Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
        //            LabelName = table.Column<string>(type: "longtext", nullable: false)
        //                .Annotation("MySql:CharSet", "utf8mb4"),
        //            TagType = table.Column<int>(type: "int", nullable: true),
        //            MustMeetAllConditions = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //            StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
        //            TradeTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
        //            MemberLevel = table.Column<string>(type: "longtext", nullable: false)
        //                .Annotation("MySql:CharSet", "utf8mb4"),
        //            MemberGender = table.Column<string>(type: "longtext", nullable: false)
        //                .Annotation("MySql:CharSet", "utf8mb4"),
        //            TradeCountMin = table.Column<int>(type: "int", nullable: true),
        //            TradeCountMax = table.Column<int>(type: "int", nullable: true),
        //            AvgOrderValueMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
        //            AvgOrderValueMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
        //            TotalSpentMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
        //            TotalSpentMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
        //            CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
        //            CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
        //            LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
        //            LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
        //            IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
        //            DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
        //            DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_HotelABPLabels", x => x.Id);
        //        })
        //        .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelAbpGrades");

            //migrationBuilder.DropTable(
            //    name: "HotelABPLabels");
        }
    }
}
