using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_Email",
                table: "UserAccounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_IsDeleted",
                table: "UserAccounts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_Role",
                table: "UserAccounts",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccount_Email",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccount_IsDeleted",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccount_Role",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
