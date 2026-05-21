using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetAmb.Migrations
{
    /// <inheritdoc />
    public partial class RevertClinicNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Paws & Claws Vet");

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Happy Tails Clinic");

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "VetCare Plus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "VetKlinika Centar");

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Veterinarska Ambulanta Petar");

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Vet Centar Split");
        }
    }
}
