using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class ChangedOrderStatenamesandaddedResponsibleEmployeepart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ResponsibleEmployeeId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders",
                column: "ResponsibleEmployeeId",
                principalTable: "RestaurantEmployees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ResponsibleEmployeeId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders",
                column: "ResponsibleEmployeeId",
                principalTable: "RestaurantEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
