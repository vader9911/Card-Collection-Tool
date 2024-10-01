using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card_Collection_Tool.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "LegalitiesId",
                table: "ScryfallCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Legalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Standard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Future = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Historic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timeless = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gladiator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pioneer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explorer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Legacy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pauper = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vintage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Penny = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commander = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oathbreaker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardBrawl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brawl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alchemy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PauperCommander = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldSchool = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Premodern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Predh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScryfallCardId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Legalities_ScryfallCards_ScryfallCardId",
                        column: x => x.ScryfallCardId,
                        principalTable: "ScryfallCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Legalities_ScryfallCardId",
                table: "Legalities",
                column: "ScryfallCardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Legalities");

            migrationBuilder.DropColumn(
                name: "LegalitiesId",
                table: "ScryfallCards");

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
        }
    }
}
