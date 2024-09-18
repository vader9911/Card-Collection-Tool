using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOneToOneRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "Prices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "ImageUris",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                filter: "[ScryfallCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "Prices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "ImageUris",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
