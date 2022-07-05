using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class Discountcoderelationchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppliedToRestaurantId",
                table: "DiscountCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AppliesToAllRestaurants",
                table: "DiscountCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodes_AppliedToRestaurantId",
                table: "DiscountCodes",
                column: "AppliedToRestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCodes_Restaurants_AppliedToRestaurantId",
                table: "DiscountCodes",
                column: "AppliedToRestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCodes_Restaurants_AppliedToRestaurantId",
                table: "DiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCodes_AppliedToRestaurantId",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "AppliedToRestaurantId",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "AppliesToAllRestaurants",
                table: "DiscountCodes");
        }
    }
}
