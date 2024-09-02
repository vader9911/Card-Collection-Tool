using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrisToScryfallCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUri",
                table: "ScryfallCards");

            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlavorText",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUris_Large",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUris_Normal",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUris_Small",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prices_USD",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prices_USDEtched",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prices_USDFoil",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rarity",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScryfallUri",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artist",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "FlavorText",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Large",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Normal",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Small",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Prices_USD",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Prices_USDEtched",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Prices_USDFoil",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ScryfallUri",
                table: "ScryfallCards");

            migrationBuilder.AddColumn<string>(
                name: "ImageUri",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
