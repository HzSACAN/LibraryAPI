using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    public partial class ekstra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmployeesId",
                table: "BorrowBook",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowBook_EmployeesId",
                table: "BorrowBook",
                column: "EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowBook_Employee_EmployeesId",
                table: "BorrowBook",
                column: "EmployeesId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowBook_Employee_EmployeesId",
                table: "BorrowBook");

            migrationBuilder.DropIndex(
                name: "IX_BorrowBook_EmployeesId",
                table: "BorrowBook");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeesId",
                table: "BorrowBook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
