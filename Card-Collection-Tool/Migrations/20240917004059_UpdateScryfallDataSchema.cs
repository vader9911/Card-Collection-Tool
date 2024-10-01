using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScryfallDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Legalities_ScryfallCards_ScryfallCardId",
                table: "Legalities");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Legalities_ScryfallCardId",
                table: "Legalities");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.RenameColumn(
                name: "USDFoil",
                table: "Prices",
                newName: "UsdFoil");

            migrationBuilder.RenameColumn(
                name: "USDEtched",
                table: "Prices",
                newName: "UsdEtched");

            migrationBuilder.RenameColumn(
                name: "USD",
                table: "Prices",
                newName: "Usd");

            migrationBuilder.AlterColumn<bool>(
                name: "Variation",
                table: "ScryfallCards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FullArt",
                table: "ScryfallCards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsdFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UsdEtched",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Usd",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Tix",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EurFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Eur",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "Legalities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Small",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Png",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Normal",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Large",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BorderCrop",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArtCrop",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Legalities_ScryfallCardId",
                table: "Legalities",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Legalities_ScryfallCards_ScryfallCardId",
                table: "Legalities",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Legalities_ScryfallCards_ScryfallCardId",
                table: "Legalities");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Legalities_ScryfallCardId",
                table: "Legalities");

            migrationBuilder.DropIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris");

            migrationBuilder.RenameColumn(
                name: "UsdFoil",
                table: "Prices",
                newName: "USDFoil");

            migrationBuilder.RenameColumn(
                name: "UsdEtched",
                table: "Prices",
                newName: "USDEtched");

            migrationBuilder.RenameColumn(
                name: "Usd",
                table: "Prices",
                newName: "USD");

            migrationBuilder.AlterColumn<bool>(
                name: "Variation",
                table: "ScryfallCards",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "FullArt",
                table: "ScryfallCards",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "USDFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "USDEtched",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "USD",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tix",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EurFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Eur",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ScryfallCardId",
                table: "Legalities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Small",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Png",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Normal",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Large",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BorderCrop",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArtCrop",
                table: "ImageUris",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Legalities_ScryfallCardId",
                table: "Legalities",
                column: "ScryfallCardId",
                unique: true,
                filter: "[ScryfallCardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Legalities_ScryfallCards_ScryfallCardId",
                table: "Legalities",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id");
        }
    }
}
