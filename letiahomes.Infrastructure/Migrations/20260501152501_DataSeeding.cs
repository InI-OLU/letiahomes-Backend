using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace letiahomes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 1, 15, 24, 59, 606, DateTimeKind.Utc).AddTicks(9861), "AQAAAAIAAYagAAAAEMcur/vuEm1fcovJUc0Yp6WPPpTxPyUXZN7hB+PLbFrYpqSRmdP93tJ3fnV+CnoZMg==" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsActive", "IsVerified", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { "e5f6a7b8-c9d0-1234-efab-456789012346", 0, "b2c3d4e5-f6a7-8901-bcde-111111111111", new DateTime(2026, 5, 1, 15, 24, 59, 712, DateTimeKind.Utc).AddTicks(3992), "emeka.obi@gmail.com", true, "Chukwuemeka", true, true, "Obi", false, null, "EMEKA.OBI@GMAIL.COM", "EMEKA.OBI@GMAIL.COM", "AQAAAAIAAYagAAAAEEV08s9NTzQgNShR9SbNfmyjtGQs+3Gf6tpwsztzlQtsBUEoqw1zHuENvnUeWTV08g==", null, false, "a1b2c3d4-e5f6-7890-abcd-111111111111", false, null, "emeka.obi@gmail.com" },
                    { "f6a7b8c9-d0e1-2345-fabc-567890123457", 0, "d4e5f6a7-b8c9-0123-defa-222222222222", new DateTime(2026, 5, 1, 15, 24, 59, 819, DateTimeKind.Utc).AddTicks(3958), "amara.nwosu@gmail.com", true, "Amara", true, true, "Nwosu", false, null, "AMARA.NWOSU@GMAIL.COM", "AMARA.NWOSU@GMAIL.COM", "AQAAAAIAAYagAAAAELnaRXBMk6PvalsH4vbJZ4Nuhu0HJhL/7NCwBmVNfeNoLaA9Z4e6NB29hCpWiEZKMw==", null, false, "c3d4e5f6-a7b8-9012-cdef-222222222222", false, null, "amara.nwosu@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "LandlordProfiles",
                columns: new[] { "Id", "AppUserId", "CreatedAt", "IsVerified", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"), "e5f6a7b8-c9d0-1234-efab-456789012346", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null },
                    { new Guid("b2c3d4e5-6789-01bc-def0-222222222222"), "f6a7b8c9-d0e1-2345-fabc-567890123457", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c3d4e5f6-a7b8-9012-cdef-234567890123", "e5f6a7b8-c9d0-1234-efab-456789012346" },
                    { "c3d4e5f6-a7b8-9012-cdef-234567890123", "f6a7b8c9-d0e1-2345-fabc-567890123457" }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "Bathrooms", "Bedrooms", "City", "Country", "CreatedAt", "Description", "IsApproved", "IsAvailable", "LandlordProfileId", "ListingType", "MaxGuests", "PricePerNightKobo", "PropertyType", "State", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), "15 Admiralty Way", 3, 3, "Lagos", "Nigeria", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A stunning fully furnished apartment with ocean views, 24/7 power supply, and top-tier security. Perfect for business travellers and families.", true, true, new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"), 1, 6, 7500000L, 0, "Lagos", "Luxury 3-Bedroom Apartment in Lekki Phase 1", null },
                    { new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), "42 Adeola Odeku Street", 1, 1, "Lagos", "Nigeria", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "A sleek, compact studio ideal for solo business travellers. Walking distance to major banks, restaurants, and the Lagos business district.", true, true, new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"), 1, 2, 3500000L, 1, "Lagos", "Modern Studio in Victoria Island", null },
                    { new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), "8 Mobolaji Johnson Avenue", 4, 4, "Lagos", "Nigeria", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A beautifully maintained 4-bedroom duplex in the serene Ikeja GRA. Comes with a private pool, generator, and dedicated parking for 3 cars.", true, true, new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"), 1, 8, 12000000L, 2, "Lagos", "Spacious Duplex in Ikeja GRA", null },
                    { new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), "23 Aminu Kano Crescent", 1, 1, "Abuja", "Nigeria", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "A clean, well-maintained self-contain apartment in the heart of Wuse 2. Great for short stays in Abuja with easy access to major roads.", true, true, new Guid("b2c3d4e5-6789-01bc-def0-222222222222"), 1, 2, 2500000L, 7, "FCT", "Cosy Self-Contain in Wuse 2", null },
                    { new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), "5 Gana Street", 3, 3, "Abuja", "Nigeria", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "A breathtaking penthouse with panoramic views of Abuja. Floor-to-ceiling windows, a rooftop terrace, and premium finishes throughout.", true, true, new Guid("b2c3d4e5-6789-01bc-def0-222222222222"), 1, 6, 25000000L, 6, "FCT", "Executive Penthouse in Maitama", null },
                    { new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), "12 Peter Odili Road", 2, 2, "Port Harcourt", "Nigeria", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A tastefully furnished 2-bedroom bungalow in New GRA. Quiet neighbourhood, 24-hour security, and close to major shopping centres.", true, true, new Guid("b2c3d4e5-6789-01bc-def0-222222222222"), 1, 4, 4500000L, 4, "Rivers", "Charming Bungalow in New GRA Port Harcourt", null }
                });

            migrationBuilder.InsertData(
                table: "PropertyAmenities",
                columns: new[] { "Id", "CreatedAt", "Name", "PropertyId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), null },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), null },
                    { new Guid("e1000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "24hr Generator", new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), null },
                    { new Guid("e1000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Security", new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), null },
                    { new Guid("e1000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Parking", new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), null },
                    { new Guid("e2000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), null },
                    { new Guid("e2000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), null },
                    { new Guid("e2000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "24hr Generator", new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), null },
                    { new Guid("e3000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swimming Pool", new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), null },
                    { new Guid("e3000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), null },
                    { new Guid("e3000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), null },
                    { new Guid("e3000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24hr Generator", new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), null },
                    { new Guid("e3000000-0000-0000-0000-000000000005"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gym", new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), null },
                    { new Guid("e4000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), null },
                    { new Guid("e4000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), null },
                    { new Guid("e4000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Security", new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), null },
                    { new Guid("e5000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Rooftop Terrace", new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), null },
                    { new Guid("e5000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), null },
                    { new Guid("e5000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), null },
                    { new Guid("e5000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Concierge Service", new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), null },
                    { new Guid("e5000000-0000-0000-0000-000000000005"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "24hr Generator", new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), null },
                    { new Guid("e6000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "WiFi", new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), null },
                    { new Guid("e6000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Parking", new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), null },
                    { new Guid("e6000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Security", new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), null },
                    { new Guid("e6000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Air Conditioning", new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), null }
                });

            migrationBuilder.InsertData(
                table: "PropertyImages",
                columns: new[] { "Id", "CreatedAt", "ImageUrl", "IsCoverImage", "PropertyId", "PublicId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800&q=80", true, new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), "", null },
                    { new Guid("d1000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&q=80", false, new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), "", null },
                    { new Guid("d1000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800&q=80", false, new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"), "", null },
                    { new Guid("d2000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80", true, new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), "", null },
                    { new Guid("d2000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1536376072261-38c75010e6c9?w=800&q=80", false, new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"), "", null },
                    { new Guid("d3000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?w=800&q=80", true, new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), "", null },
                    { new Guid("d3000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800&q=80", false, new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), "", null },
                    { new Guid("d3000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1576941089067-2de3c901e126?w=800&q=80", false, new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"), "", null },
                    { new Guid("d4000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1554995207-c18c203602cb?w=800&q=80", true, new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), "", null },
                    { new Guid("d4000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=800&q=80", false, new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"), "", null },
                    { new Guid("d5000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&q=80", true, new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), "", null },
                    { new Guid("d5000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1560185007-cde436f6a4d0?w=800&q=80", false, new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), "", null },
                    { new Guid("d5000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800&q=80", false, new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"), "", null },
                    { new Guid("d6000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&q=80", true, new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), "", null },
                    { new Guid("d6000000-0000-0000-0000-000000000002"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800&q=80", false, new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"), "", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e2000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e2000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e2000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e3000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e3000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e3000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e3000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e3000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e4000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e4000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e4000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e5000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e5000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e5000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e5000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e5000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e6000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e6000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e6000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyAmenities",
                keyColumn: "Id",
                keyValue: new Guid("e6000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d2000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d2000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d3000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d3000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d3000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d4000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d4000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d5000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d5000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d5000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d6000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: new Guid("d6000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c3d4e5f6-a7b8-9012-cdef-234567890123", "e5f6a7b8-c9d0-1234-efab-456789012346" });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c3d4e5f6-a7b8-9012-cdef-234567890123", "f6a7b8c9-d0e1-2345-fabc-567890123457" });

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-3456-789a-cdef-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c4d5e6f7-4567-89ab-def0-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("c6d7e8f9-6789-abcd-f012-ffffffffffff"));

            migrationBuilder.DeleteData(
                table: "LandlordProfiles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-5678-90ab-cdef-111111111111"));

            migrationBuilder.DeleteData(
                table: "LandlordProfiles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-6789-01bc-def0-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "e5f6a7b8-c9d0-1234-efab-456789012346");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "f6a7b8c9-d0e1-2345-fabc-567890123457");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 29, 4, 59, 53, 867, DateTimeKind.Utc).AddTicks(7864), "AQAAAAIAAYagAAAAEN6kQ/d4B0wHH/T04VsHx9CEVzog0e6WiuVhmnbX5qLwYtHJhzDg1JVAxU7eQv2xNQ==" });
        }
    }
}
