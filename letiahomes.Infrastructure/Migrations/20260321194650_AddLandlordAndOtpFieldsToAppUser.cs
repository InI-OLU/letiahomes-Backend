using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace letiahomes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandlordAndOtpFieldsToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiresAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "Address", "BankAccountName", "BankAccountNumber", "BankName", "CreatedAt", "GovernmentId", "OtpCode", "OtpExpiresAt", "PasswordHash" },
                values: new object[] { null, null, null, null, new DateTime(2026, 3, 21, 19, 46, 49, 854, DateTimeKind.Utc).AddTicks(518), null, null, null, "AQAAAAIAAYagAAAAECrcsRoTqxM8Xdgs+1Kx5ZUvadoXDXDPGmGSs5/CHUqxRfFAZxyBd6/TSoFA2wyKlQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GovernmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpiresAt",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 21, 11, 30, 37, 515, DateTimeKind.Utc).AddTicks(7367), "AQAAAAIAAYagAAAAEON3tyWSSsjnuirhJbTrW67E1QD6jG2OZGojRDQ+by55gZdtYtXuBdrQRP6QEwOzrg==" });
        }
    }
}
