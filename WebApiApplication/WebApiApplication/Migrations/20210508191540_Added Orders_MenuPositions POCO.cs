using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class AddedOrders_MenuPositionsPOCO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders_MenuPositions");

            migrationBuilder.CreateTable(
                name: "Order_MenuPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    MenuPositionId = table.Column<int>(type: "int", nullable: false),
                    PositionsInOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_MenuPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_MenuPosition_MenuPositions_MenuPositionId",
                        column: x => x.MenuPositionId,
                        principalTable: "MenuPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_MenuPosition_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_MenuPosition_MenuPositionId",
                table: "Order_MenuPosition",
                column: "MenuPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_MenuPosition_OrderId",
                table: "Order_MenuPosition",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order_MenuPosition");

            migrationBuilder.CreateTable(
                name: "Orders_MenuPositions",
                columns: table => new
                {
                    MenuPositionsId = table.Column<int>(type: "int", nullable: false),
                    OrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_MenuPositions", x => new { x.MenuPositionsId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_Orders_MenuPositions_MenuPositions_MenuPositionsId",
                        column: x => x.MenuPositionsId,
                        principalTable: "MenuPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_MenuPositions_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MenuPositions_OrdersId",
                table: "Orders_MenuPositions",
                column: "OrdersId");
        }
    }
}
