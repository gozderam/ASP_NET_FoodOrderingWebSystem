using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class Notrequiredrestauranttorestaurateur : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantEmployees_Restaurants_RestaurantId",
                table: "RestaurantEmployees");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "RestaurantEmployees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantEmployees_Restaurants_RestaurantId",
                table: "RestaurantEmployees",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantEmployees_Restaurants_RestaurantId",
                table: "RestaurantEmployees");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "RestaurantEmployees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantEmployees_Restaurants_RestaurantId",
                table: "RestaurantEmployees",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
