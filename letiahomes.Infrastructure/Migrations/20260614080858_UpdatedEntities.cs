using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace letiahomes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "UnavailableDates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancellationCount",
                table: "LandlordProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NightsCount",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "PlatformFeeKobo",
                table: "Bookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubtotalKobo",
                table: "Bookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "LandlordProfiles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"),
                column: "CancellationCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "LandlordProfiles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-6789-01bc-def0-222222222222"),
                column: "CancellationCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 8, 8, 56, 521, DateTimeKind.Utc).AddTicks(3172), "AQAAAAIAAYagAAAAEGONCdGZobFX/q+MXNWS3uYvYYeZFBmKNU53kZAUVZWj47iinAUe8RqCCCmM3DRLXg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "e5f6a7b8-c9d0-1234-efab-456789012346",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 8, 8, 56, 623, DateTimeKind.Utc).AddTicks(4896), "AQAAAAIAAYagAAAAEMmpooE4GfU9Kkf90Gv83BdzYdT+8PdkpBXDFeQyXDeLT98ef3lQH+RumRKUuaMdGQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "f6a7b8c9-d0e1-2345-fabc-567890123457",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 8, 8, 56, 726, DateTimeKind.Utc).AddTicks(5042), "AQAAAAIAAYagAAAAEG5jdTjtuuWfcpd6A90A7q4OZNCHRoyJZaUQT2ocIYD3tx5arYnqPugYNOM38ynv9A==" });

            migrationBuilder.CreateIndex(
                name: "IX_UnavailableDates_BookingId",
                table: "UnavailableDates",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnavailableDates_Bookings_BookingId",
                table: "UnavailableDates",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnavailableDates_Bookings_BookingId",
                table: "UnavailableDates");

            migrationBuilder.DropIndex(
                name: "IX_UnavailableDates_BookingId",
                table: "UnavailableDates");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "UnavailableDates");

            migrationBuilder.DropColumn(
                name: "CancellationCount",
                table: "LandlordProfiles");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "NightsCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PlatformFeeKobo",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SubtotalKobo",
                table: "Bookings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 1, 15, 24, 59, 606, DateTimeKind.Utc).AddTicks(9861), "AQAAAAIAAYagAAAAEMcur/vuEm1fcovJUc0Yp6WPPpTxPyUXZN7hB+PLbFrYpqSRmdP93tJ3fnV+CnoZMg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "e5f6a7b8-c9d0-1234-efab-456789012346",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 1, 15, 24, 59, 712, DateTimeKind.Utc).AddTicks(3992), "AQAAAAIAAYagAAAAEEV08s9NTzQgNShR9SbNfmyjtGQs+3Gf6tpwsztzlQtsBUEoqw1zHuENvnUeWTV08g==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "f6a7b8c9-d0e1-2345-fabc-567890123457",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 1, 15, 24, 59, 819, DateTimeKind.Utc).AddTicks(3958), "AQAAAAIAAYagAAAAELnaRXBMk6PvalsH4vbJZ4Nuhu0HJhL/7NCwBmVNfeNoLaA9Z4e6NB29hCpWiEZKMw==" });
        }
    }
}
