using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class Addedfavouriterestaurnattoclient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients_FavouriteRestaurants",
                columns: table => new
                {
                    FavouriteForClientsId = table.Column<int>(type: "int", nullable: false),
                    FavouriteRestaurantsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients_FavouriteRestaurants", x => new { x.FavouriteForClientsId, x.FavouriteRestaurantsId });
                    table.ForeignKey(
                        name: "FK_Clients_FavouriteRestaurants_Clients_FavouriteForClientsId",
                        column: x => x.FavouriteForClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clients_FavouriteRestaurants_Restaurants_FavouriteRestaurantsId",
                        column: x => x.FavouriteRestaurantsId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_FavouriteRestaurants_FavouriteRestaurantsId",
                table: "Clients_FavouriteRestaurants",
                column: "FavouriteRestaurantsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients_FavouriteRestaurants");
        }
    }
}
