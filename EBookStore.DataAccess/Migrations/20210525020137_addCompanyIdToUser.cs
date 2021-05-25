using Microsoft.EntityFrameworkCore.Migrations;

namespace EBookStore.DataAccess.Migrations
{
    public partial class addCompanyIdToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Company Id",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Company Id",
                table: "AspNetUsers",
                column: "Company Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_Company Id",
                table: "AspNetUsers",
                column: "Company Id",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_Company Id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Company Id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Company Id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");
        }
    }
}
