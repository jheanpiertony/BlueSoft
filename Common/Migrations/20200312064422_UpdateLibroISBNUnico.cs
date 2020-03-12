using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations
{
    public partial class UpdateLibroISBNUnico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Libro_ISBN",
                table: "Libro",
                column: "ISBN",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libro_ISBN",
                table: "Libro");
        }
    }
}
