using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetAmb.Migrations
{
    /// <inheritdoc />
    public partial class CroatianSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Sve u redu, vitalni znakovi uredni", "Godišnji pregled" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Zahvat uspješan, oporavak teče uredno", "Uklanjanje lipoma" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Zdrava, bez promjena", "Rutinski pregled" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Zubni kamenac uklonjen, preporučuje se kontrola za 6 mj.", "Problemi sa zubima" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Notes", "Reason", "RescheduleReason" },
                values: new object[] { "Odgođeno na 12. svibnja 2026. – 15:00", "Bol u nozi", "Vlasnik nedostupan zbog puta" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Zahvat obavljen bez komplikacija", "Čišćenje zubi" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "Ilica 42, Zagreb", "info@vetklinika.hr", "VetKlinika Centar", "01 234 5678" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "Varšavska 15, Zagreb", "kontakt@vetambulanta.hr", "Veterinarska Ambulanta Petar", "01 345 6789" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "Marmontova 3, Split", "info@vetcentar-split.hr", "Vet Centar Split", "021 456 7890" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Zdrav pacijent – rutinski pregled", "Godišnji pregled – svi vitalni znakovi uredni", "Nije potrebno liječenje" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Lipom (benigna tvorba)", "Postoperativni oporavak teče uredno", "Kirurško uklanjanje" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Zdrav pacijent – rutinski pregled", "Tjelesna masa stabilna", "Nije potrebno liječenje" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Parodontna bolest stupanj 2", "Kontrola za 6 mjeseci", "Profesionalno čišćenje zubi" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Nakupljanje zubnog kamenca", "Problem riješen", "Čišćenje zubi" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Ilica 10, Zagreb", "ivan.horvat@gmail.com", "091 234 5678" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Maksimirska 22, Zagreb", "petra.babic@gmail.com", "098 765 4321" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Gajeva 5, Zagreb", "tomislav.knezevic@gmail.com", "095 111 2233" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Šubićeva 8, Zagreb", "sara.petrovic@gmail.com", "099 876 5432" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Marmontova 3, Split", "nina.vukovic@gmail.com", "091 500 6001" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Obala 17, Split", "filip.radic@gmail.com", "098 600 1700" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Njemački ovčar", "Crno-smeđa" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Sijamska", "Kremasta" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Labrador retriver", "Zlatna", "Bruno" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Perzijska", "Bijela" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Kakadu", "Žuta" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Patuljasti kunić", "Smeđa", "Šarko" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Pudl", "Bijela", "Pahuljica" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Sirijski hrčak", "Narančasta", "Hrčko" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 9,
                column: "Color",
                value: "Tigasta");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Rutinski zdravstveni pregled životinje", "Opći pregled" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Standardno cijepljenje prema protokolu", "Cijepljenje" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Manja kirurška intervencija", "Manji kirurški zahvat" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Profesionalno čišćenje zubnog kamenca", "Čišćenje zubi" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Dijagnostičko slikanje rentgenom", "Rendgensko snimanje" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "All good", "Annual checkup" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Successful", "Lump removal" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Healthy", "Routine visit" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Treated", "Dental issues" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Notes", "Reason", "RescheduleReason" },
                values: new object[] { "Rescheduled to May 12, 2026 - 15:00", "Leg pain", "Owner unavailable due to travel" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Notes", "Reason" },
                values: new object[] { "Done", "Teeth cleaning" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "123 Main St", "info@pawsclaws.com", "Paws & Claws Vet", "555-1000" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "456 Park Rd", "contact@happytails.com", "Happy Tails Clinic", "555-3000" });

            migrationBuilder.UpdateData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Email", "Name", "Phone" },
                values: new object[] { "789 River Blvd", "hello@vetcareplus.com", "VetCare Plus", "555-5000" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Healthy - routine exam", "Annual visit - all vitals normal", "None required" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Lipoma (benign lump)", "Post-op recovery good", "Surgical removal" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Healthy - routine exam", "Weight stable", "None required" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Periodontal disease grade 2", "Follow-up in 6 months", "Professional dental cleaning" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Diagnosis", "Notes", "Treatment" },
                values: new object[] { "Dental tartar buildup", "Resolved", "Teeth cleaning" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "10 Oak Ave", "ivan@mail.com", "555-2001" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "22 Elm St", "petra@mail.com", "555-2002" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "5 Pine Ln", "tomi@mail.com", "555-4001" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "8 Birch Dr", "sara@mail.com", "555-4002" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "3 Cedar Ct", "nina@mail.com", "555-6001" });

            migrationBuilder.UpdateData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "17 Maple Way", "filip@mail.com", "555-6002" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "German Shepherd", "Black/Tan" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Siamese", "Cream" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Labrador", "Golden", "Buddy" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Persian", "White" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Breed", "Color" },
                values: new object[] { "Cockatiel", "Yellow" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Holland Lop", "Brown", "Rocky" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Poodle", "White", "Max" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Breed", "Color", "Name" },
                values: new object[] { "Syrian", "Orange", "Coco" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 9,
                column: "Color",
                value: "Tabby");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Routine health examination", "General Checkup" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Standard vaccination", "Vaccination" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Minor surgical procedure", "Minor Surgery" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Professional teeth cleaning", "Dental Cleaning" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Diagnostic imaging", "X-Ray" });
        }
    }
}
