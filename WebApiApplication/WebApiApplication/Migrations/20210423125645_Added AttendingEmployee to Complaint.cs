using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiApplication.Migrations
{
    public partial class AddedAttendingEmployeetoComplaint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendingEmployeeId",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AttendingEmployeeId",
                table: "Complaints",
                column: "AttendingEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_RestaurantEmployees_AttendingEmployeeId",
                table: "Complaints",
                column: "AttendingEmployeeId",
                principalTable: "RestaurantEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_RestaurantEmployees_AttendingEmployeeId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_AttendingEmployeeId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "AttendingEmployeeId",
                table: "Complaints");
        }
    }
}
