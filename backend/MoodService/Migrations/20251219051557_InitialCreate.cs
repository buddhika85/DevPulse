using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoodEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoodTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoodLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntry_Day",
                table: "MoodEntries",
                column: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntry_Time",
                table: "MoodEntries",
                column: "MoodTime");

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntry_UserId",
                table: "MoodEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_MoodEntry_User_Day_Time",
                table: "MoodEntries",
                columns: new[] { "UserId", "Day", "MoodTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoodEntries");
        }
    }
}
