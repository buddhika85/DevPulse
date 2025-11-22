using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserManagerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "UserAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_ManagerId",
                table: "UserAccounts",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_UserAccounts_ManagerId",
                table: "UserAccounts",
                column: "ManagerId",
                principalTable: "UserAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_UserAccounts_ManagerId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_ManagerId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "UserAccounts");
        }
    }
}
