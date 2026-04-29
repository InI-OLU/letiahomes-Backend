using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace letiahomes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "PropertyImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 29, 4, 59, 53, 867, DateTimeKind.Utc).AddTicks(7864), "AQAAAAIAAYagAAAAEN6kQ/d4B0wHH/T04VsHx9CEVzog0e6WiuVhmnbX5qLwYtHJhzDg1JVAxU7eQv2xNQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "PropertyImages");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 11, 0, 18, 47, 808, DateTimeKind.Utc).AddTicks(5010), "AQAAAAIAAYagAAAAEAzz0cFB3IkA5ur57n/L7/gnyyQAKmsW0VQmLwPD7+bad+cp9yY/0XJcPK6GOf5Q7w==" });
        }
    }
}
