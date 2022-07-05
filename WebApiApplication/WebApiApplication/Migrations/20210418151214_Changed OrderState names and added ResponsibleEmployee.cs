using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class ChangedOrderStatenamesandaddedResponsibleEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsibleEmployeeId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ResponsibleEmployeeId",
                table: "Orders",
                column: "ResponsibleEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders",
                column: "ResponsibleEmployeeId",
                principalTable: "RestaurantEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_RestaurantEmployees_ResponsibleEmployeeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ResponsibleEmployeeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ResponsibleEmployeeId",
                table: "Orders");
        }
    }
}
