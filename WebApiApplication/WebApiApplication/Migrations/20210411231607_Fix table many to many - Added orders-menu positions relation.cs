using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class FixtablemanytomanyAddedordersmenupositionsrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuPositionOrder_MenuPositions_MenuPositionsId",
                table: "MenuPositionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuPositionOrder_Orders_OrdersId",
                table: "MenuPositionOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuPositionOrder",
                table: "MenuPositionOrder");

            migrationBuilder.RenameTable(
                name: "MenuPositionOrder",
                newName: "Orders_MenuPositions");

            migrationBuilder.RenameIndex(
                name: "IX_MenuPositionOrder_OrdersId",
                table: "Orders_MenuPositions",
                newName: "IX_Orders_MenuPositions_OrdersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders_MenuPositions",
                table: "Orders_MenuPositions",
                columns: new[] { "MenuPositionsId", "OrdersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MenuPositions_MenuPositions_MenuPositionsId",
                table: "Orders_MenuPositions",
                column: "MenuPositionsId",
                principalTable: "MenuPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MenuPositions_Orders_OrdersId",
                table: "Orders_MenuPositions",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MenuPositions_MenuPositions_MenuPositionsId",
                table: "Orders_MenuPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MenuPositions_Orders_OrdersId",
                table: "Orders_MenuPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders_MenuPositions",
                table: "Orders_MenuPositions");

            migrationBuilder.RenameTable(
                name: "Orders_MenuPositions",
                newName: "MenuPositionOrder");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_MenuPositions_OrdersId",
                table: "MenuPositionOrder",
                newName: "IX_MenuPositionOrder_OrdersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuPositionOrder",
                table: "MenuPositionOrder",
                columns: new[] { "MenuPositionsId", "OrdersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MenuPositionOrder_MenuPositions_MenuPositionsId",
                table: "MenuPositionOrder",
                column: "MenuPositionsId",
                principalTable: "MenuPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuPositionOrder_Orders_OrdersId",
                table: "MenuPositionOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
