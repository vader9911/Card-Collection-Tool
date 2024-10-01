using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailsScryfallCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Cmc",
                table: "ScryfallCards",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollectorNumber",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorIdentity",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Digital",
                table: "ScryfallCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FullArt",
                table: "ScryfallCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Games",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Legalities_Banned",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Legalities_Legal",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Legalities_NotLegal",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Legalities_Restricted",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Power",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reprint",
                table: "ScryfallCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Set",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetId",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Toughness",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Variation",
                table: "ScryfallCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariationOf",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cmc",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "CollectorNumber",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ColorIdentity",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Colors",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Digital",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "FullArt",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Games",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Legalities_Banned",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Legalities_Legal",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Legalities_NotLegal",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Legalities_Restricted",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Reprint",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Set",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Toughness",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "Variation",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "VariationOf",
                table: "ScryfallCards");
        }
    }
}
