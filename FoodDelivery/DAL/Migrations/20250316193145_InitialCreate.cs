using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StationName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    WMOCode = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Temperature = table.Column<double>(type: "REAL", nullable: false),
                    WindSpeed = table.Column<double>(type: "REAL", nullable: false),
                    Phenomenon = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherData");
        }
    }
}
