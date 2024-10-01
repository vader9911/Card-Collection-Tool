using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddScryfallDataEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "LegalitiesId",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ScryfallUri",
                table: "ScryfallCards");

            migrationBuilder.AlterColumn<bool>(
                name: "Reprint",
                table: "ScryfallCards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Games",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Digital",
                table: "ScryfallCards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Colors",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ColorIdentity",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
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
                name: "ScryfallCardId",
                table: "Prices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
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

            migrationBuilder.AddColumn<string>(
                name: "EurFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "USDEtched",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "USDFoil",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "EurFoil",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "USDEtched",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "USDFoil",
                table: "Prices");

            migrationBuilder.AlterColumn<bool>(
                name: "Reprint",
                table: "ScryfallCards",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Games",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "Digital",
                table: "ScryfallCards",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Colors",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ColorIdentity",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "LegalitiesId",
                table: "ScryfallCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ScryfallUri",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "USD",
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
                name: "ScryfallCardId",
                table: "Prices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Eur",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                unique: true,
                filter: "[ScryfallCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_ScryfallCards_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                principalTable: "ScryfallCards",
                principalColumn: "Id");
        }
    }
}
