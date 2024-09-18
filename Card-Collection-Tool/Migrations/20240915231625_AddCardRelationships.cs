using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddCardRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.AlterColumn<string>(
                name: "USD",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tix",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Eur",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
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
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.AlterColumn<decimal>(
                name: "USD",
                table: "Prices",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Tix",
                table: "Prices",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Eur",
                table: "Prices",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "ImageUris",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                unique: true,
                filter: "[ScryfallCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id");
        }
    }
}
