using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class Menusectionandpositionchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuPositions_Restaurants");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "MenuSections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuSections_RestaurantId",
                table: "MenuSections",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuSections_Restaurants_RestaurantId",
                table: "MenuSections",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuSections_Restaurants_RestaurantId",
                table: "MenuSections");

            migrationBuilder.DropIndex(
                name: "IX_MenuSections_RestaurantId",
                table: "MenuSections");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "MenuSections");

            migrationBuilder.CreateTable(
                name: "MenuPositions_Restaurants",
                columns: table => new
                {
                    MenuPositionsId = table.Column<int>(type: "int", nullable: false),
                    RestaurantsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPositions_Restaurants", x => new { x.MenuPositionsId, x.RestaurantsId });
                    table.ForeignKey(
                        name: "FK_MenuPositions_Restaurants_MenuPositions_MenuPositionsId",
                        column: x => x.MenuPositionsId,
                        principalTable: "MenuPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuPositions_Restaurants_Restaurants_RestaurantsId",
                        column: x => x.RestaurantsId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuPositions_Restaurants_RestaurantsId",
                table: "MenuPositions_Restaurants",
                column: "RestaurantsId");
        }
    }
}
