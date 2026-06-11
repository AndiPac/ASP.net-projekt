using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetAmb.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesForOibAndJmbg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_JMBG",
                table: "AspNetUsers",
                column: "JMBG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OIB",
                table: "AspNetUsers",
                column: "OIB",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_JMBG",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OIB",
                table: "AspNetUsers");
        }
    }
}
