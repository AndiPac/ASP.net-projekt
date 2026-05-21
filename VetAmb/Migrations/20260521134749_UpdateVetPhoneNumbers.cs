using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetAmb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVetPhoneNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Phone",
                value: "091 234 1001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Phone",
                value: "092 456 1002");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 3,
                column: "Phone",
                value: "095 678 3001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 4,
                column: "Phone",
                value: "099 321 3002");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 5,
                column: "Phone",
                value: "098 123 5001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 6,
                column: "Phone",
                value: "091 765 5002");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Phone",
                value: "555-1001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Phone",
                value: "555-1002");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 3,
                column: "Phone",
                value: "555-3001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 4,
                column: "Phone",
                value: "555-3002");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 5,
                column: "Phone",
                value: "555-5001");

            migrationBuilder.UpdateData(
                table: "Vets",
                keyColumn: "Id",
                keyValue: 6,
                column: "Phone",
                value: "555-5002");
        }
    }
}
