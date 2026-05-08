using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VetAmb.Migrations
{
    /// <inheritdoc />
    public partial class InitialVetAmbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoundationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Owners_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vets_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Species = table.Column<int>(type: "int", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MicrochipId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RescheduleReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    VetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Vets_VetId",
                        column: x => x.VetId,
                        principalTable: "Vets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentServices_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "Address", "Email", "FoundationDate", "MaxCapacity", "Name", "Phone", "RegistrationNumber" },
                values: new object[,]
                {
                    { 1, "123 Main St", "info@pawsclaws.com", new DateTime(2010, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 50, "Paws & Claws Vet", "555-1000", "CLN-001" },
                    { 2, "456 Park Rd", "contact@happytails.com", new DateTime(2015, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, "Happy Tails Clinic", "555-3000", "CLN-002" },
                    { 3, "789 River Blvd", "hello@vetcareplus.com", new DateTime(2018, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 40, "VetCare Plus", "555-5000", "CLN-003" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "EstimatedDurationMinutes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Routine health examination", 30, "General Checkup", 30m },
                    { 2, "Standard vaccination", 15, "Vaccination", 25m },
                    { 3, "Minor surgical procedure", 60, "Minor Surgery", 150m },
                    { 4, "Professional teeth cleaning", 45, "Dental Cleaning", 80m },
                    { 5, "Diagnostic imaging", 20, "X-Ray", 60m }
                });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "Address", "ClinicId", "Email", "FirstName", "IdNumber", "LastName", "Phone", "RegistrationDate" },
                values: new object[,]
                {
                    { 1, "10 Oak Ave", 1, "ivan@mail.com", "Ivan", "OWN-001", "Horvat", "555-2001", new DateTime(2020, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "22 Elm St", 1, "petra@mail.com", "Petra", "OWN-002", "Babić", "555-2002", new DateTime(2021, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "5 Pine Ln", 2, "tomi@mail.com", "Tomislav", "OWN-003", "Knežević", "555-4001", new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "8 Birch Dr", 2, "sara@mail.com", "Sara", "OWN-004", "Petrović", "555-4002", new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "3 Cedar Ct", 3, "nina@mail.com", "Nina", "OWN-005", "Vuković", "555-6001", new DateTime(2021, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "17 Maple Way", 3, "filip@mail.com", "Filip", "OWN-006", "Radić", "555-6002", new DateTime(2023, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Vets",
                columns: new[] { "Id", "ClinicId", "FirstName", "HourlyRate", "LastName", "LicenseNumber", "Phone", "Specialization", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 1, "Ana", 80m, "Kovač", "VET-101", "555-1001", 0, 10 },
                    { 2, 1, "Marko", 100m, "Novak", "VET-102", "555-1002", 1, 7 },
                    { 3, 2, "Luka", 120m, "Jurić", "VET-201", "555-3001", 3, 12 },
                    { 4, 2, "Maja", 90m, "Tomić", "VET-202", "555-3002", 4, 5 },
                    { 5, 3, "Elena", 130m, "Matić", "VET-301", "555-5001", 5, 15 },
                    { 6, 3, "Dario", 95m, "Šimić", "VET-302", "555-5002", 2, 8 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Breed", "Color", "DateOfBirth", "MicrochipId", "Name", "OwnerId", "Species", "Weight" },
                values: new object[,]
                {
                    { 1, "German Shepherd", "Black/Tan", new DateTime(2018, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0001", "Rex", 1, 0, 32.5m },
                    { 2, "Siamese", "Cream", new DateTime(2020, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0002", "Mici", 1, 1, 4.2m },
                    { 3, "Labrador", "Golden", new DateTime(2019, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0003", "Buddy", 2, 0, 28.0m },
                    { 4, "Persian", "White", new DateTime(2021, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0004", "Luna", 3, 1, 5.0m },
                    { 5, "Cockatiel", "Yellow", new DateTime(2022, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0005", "Kiki", 3, 2, 0.1m },
                    { 6, "Holland Lop", "Brown", new DateTime(2023, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0006", "Rocky", 4, 3, 1.8m },
                    { 7, "Poodle", "White", new DateTime(2020, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0007", "Max", 5, 0, 7.5m },
                    { 8, "Syrian", "Orange", new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0008", "Coco", 5, 4, 0.15m },
                    { 9, "Maine Coon", "Tabby", new DateTime(2019, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "MC-0009", "Zara", 6, 1, 6.3m }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentDateTime", "Notes", "PatientId", "Reason", "RescheduleReason", "Status", "VetId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), "All good", 1, "Annual checkup", null, 2, 1 },
                    { 2, new DateTime(2026, 5, 3, 14, 0, 0, 0, DateTimeKind.Unspecified), "Successful", 3, "Lump removal", null, 2, 2 },
                    { 3, new DateTime(2026, 5, 7, 10, 0, 0, 0, DateTimeKind.Unspecified), "Healthy", 4, "Routine visit", null, 2, 3 },
                    { 4, new DateTime(2026, 4, 18, 11, 0, 0, 0, DateTimeKind.Unspecified), "Treated", 7, "Dental issues", null, 2, 6 },
                    { 5, new DateTime(2026, 5, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), "Rescheduled to May 12, 2026 - 15:00", 9, "Leg pain", "Owner unavailable due to travel", 5, 5 },
                    { 6, new DateTime(2026, 5, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), "Done", 2, "Teeth cleaning", null, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "MedicalRecords",
                columns: new[] { "Id", "Diagnosis", "Notes", "PatientId", "RecordDate", "Treatment" },
                values: new object[,]
                {
                    { 1, "Healthy - routine exam", "Annual visit - all vitals normal", 1, new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "None required" },
                    { 2, "Lipoma (benign lump)", "Post-op recovery good", 3, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Surgical removal" },
                    { 3, "Healthy - routine exam", "Weight stable", 4, new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "None required" },
                    { 4, "Periodontal disease grade 2", "Follow-up in 6 months", 7, new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Professional dental cleaning" },
                    { 5, "Dental tartar buildup", "Resolved", 2, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teeth cleaning" }
                });

            migrationBuilder.InsertData(
                table: "AppointmentServices",
                columns: new[] { "Id", "AppointmentId", "ServiceId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 2, 3 },
                    { 4, 3, 1 },
                    { 5, 4, 4 },
                    { 6, 4, 5 },
                    { 7, 5, 5 },
                    { 8, 6, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VetId",
                table: "Appointments",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_AppointmentId_ServiceId",
                table: "AppointmentServices",
                columns: new[] { "AppointmentId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_ServiceId",
                table: "AppointmentServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_ClinicId",
                table: "Owners",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_OwnerId",
                table: "Patients",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vets_ClinicId",
                table: "Vets",
                column: "ClinicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentServices");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Vets");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Clinics");
        }
    }
}
