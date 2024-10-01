using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddPricesAndImageUrisTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUris_Large",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Normal",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Png",
                table: "ScryfallCards");

            migrationBuilder.DropColumn(
                name: "ImageUris_Small",
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

            migrationBuilder.CreateTable(
                name: "ImageUris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Small = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Normal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Large = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Png = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtCrop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorderCrop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScryfallCardId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageUris_ScryfallCards_ScryfallCardId",
                        column: x => x.ScryfallCardId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Eur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScryfallCardId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_ScryfallCards_ScryfallCardId",
                        column: x => x.ScryfallCardId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageUris_ScryfallCardId",
                table: "ImageUris",
                column: "ScryfallCardId",
                unique: true,
                filter: "[ScryfallCardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ScryfallCardId",
                table: "Prices",
                column: "ScryfallCardId",
                unique: true,
                filter: "[ScryfallCardId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageUris");

            migrationBuilder.DropTable(
                name: "Prices");

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
                name: "ImageUris_Png",
                table: "ScryfallCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUris_Small",
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
        }
    }
}
