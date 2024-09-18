using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScryfallDataSchemaWithNewRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId");
        }
    }
}
