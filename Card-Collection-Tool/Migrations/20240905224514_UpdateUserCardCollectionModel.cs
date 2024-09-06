using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserCardCollectionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCardCollections_AspNetUsers_UserId",
                table: "UserCardCollections");

            migrationBuilder.DropIndex(
                name: "IX_UserCardCollections_UserId",
                table: "UserCardCollections");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserCardCollections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserCardCollections",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserCardCollections_UserId",
                table: "UserCardCollections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCardCollections_AspNetUsers_UserId",
                table: "UserCardCollections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
