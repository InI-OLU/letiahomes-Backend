using letiahomes.Domain.Common;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace letiahomes.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LandlordProfile> LandlordProfiles { get; set; }
        public DbSet<TenantProfile> TenantProfiles { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<UnavailableDate> UnavailableDates { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Payout> Payouts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            var auditableEntries = ChangeTracker
                .Entries<AuditableEntity>();

            foreach (var entry in auditableEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            var refreshTokenEntries = ChangeTracker
                .Entries<RefreshToken>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in refreshTokenEntries)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            var adminRoleId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
            var landlordRoleId = "c3d4e5f6-a7b8-9012-cdef-234567890123";
            var tenantRoleId = "d4e5f6a7-b8c9-0123-defa-345678901234";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "f1e2d3c4-b5a6-7890-abcd-ef0987654321"
                },
                new IdentityRole
                {
                    Id = landlordRoleId,
                    Name = "Landlord",
                    NormalizedName = "LANDLORD",
                    ConcurrencyStamp = "e5f6a7b8-c9d0-1234-efab-456789012345"
                },
                new IdentityRole
                {
                    Id = tenantRoleId,
                    Name = "Tenant",
                    NormalizedName = "TENANT",
                    ConcurrencyStamp = "f6a7b8c9-d0e1-2345-fabc-567890123456"
                }
            );


            var adminUserId = "b2c3d4e5-f6a7-8901-bcde-f12345678901";

            var hasher = new PasswordHasher<AppUser>();
            var adminUser = new AppUser
            {
                Id = adminUserId,
                FirstName = "Inioluwa",
                LastName = "Agbeleye",
                UserName = "gbolahanagbeleye@gmail.com",
                NormalizedUserName = "GBOLAHANAGBELEYE@GMAIL.COM",
                Email = "gbolahanagbeleye@gmail.com",
                NormalizedEmail = "GBOLAHANAGBELEYE@GMAIL.COM",
                EmailConfirmed = true,
                IsActive = true,      
                IsVerified = true,    
                SecurityStamp = "c3d4e5f6-a7b8-9012-cdef-123456789012",
                ConcurrencyStamp = "d4e5f6a7-b8c9-0123-defa-234567890123"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123!");

            builder.Entity<AppUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            });


            // Landlord 1
            var landlord1UserId = "e5f6a7b8-c9d0-1234-efab-456789012346";
            var landlord1ProfileId = Guid.Parse("a1b2c3d4-5678-90ab-cdef-111111111111");

            var landlord1User = new AppUser
            {
                Id = landlord1UserId,
                FirstName = "Chukwuemeka",
                LastName = "Obi",
                UserName = "emeka.obi@gmail.com",
                NormalizedUserName = "EMEKA.OBI@GMAIL.COM",
                Email = "emeka.obi@gmail.com",
                NormalizedEmail = "EMEKA.OBI@GMAIL.COM",
                EmailConfirmed = true,
                IsActive = true,
                IsVerified = true,
                SecurityStamp = "a1b2c3d4-e5f6-7890-abcd-111111111111",
                ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-111111111111",
            };
            landlord1User.PasswordHash = hasher.HashPassword(landlord1User, "Landlord@123!");

            // Landlord 2
            var landlord2UserId = "f6a7b8c9-d0e1-2345-fabc-567890123457";
            var landlord2ProfileId = Guid.Parse("b2c3d4e5-6789-01bc-def0-222222222222");

            var landlord2User = new AppUser
            {
                Id = landlord2UserId,
                FirstName = "Amara",
                LastName = "Nwosu",
                UserName = "amara.nwosu@gmail.com",
                NormalizedUserName = "AMARA.NWOSU@GMAIL.COM",
                Email = "amara.nwosu@gmail.com",
                NormalizedEmail = "AMARA.NWOSU@GMAIL.COM",
                EmailConfirmed = true,
                IsActive = true,
                IsVerified = true,
                SecurityStamp = "c3d4e5f6-a7b8-9012-cdef-222222222222",
                ConcurrencyStamp = "d4e5f6a7-b8c9-0123-defa-222222222222",
            };
            landlord2User.PasswordHash = hasher.HashPassword(landlord2User, "Landlord@123!");

            builder.Entity<AppUser>().HasData(landlord1User, landlord2User);

            // Assign Landlord role to both
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = landlord1UserId, RoleId = landlordRoleId },
                new IdentityUserRole<string> { UserId = landlord2UserId, RoleId = landlordRoleId }
            );


            builder.Entity<LandlordProfile>().HasData(
              new LandlordProfile
            {
                 Id = landlord1ProfileId,
                 AppUserId = landlord1UserId,
                 IsVerified = true,
                  CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
             new LandlordProfile
            {
                    Id = landlord2ProfileId,
                   AppUserId = landlord2UserId,
                   IsVerified = true,
                   CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
             }
            );




            var property1Id = Guid.Parse("c1d2e3f4-1234-5678-abcd-aaaaaaaaaaaa");
            var property2Id = Guid.Parse("c2d3e4f5-2345-6789-bcde-bbbbbbbbbbbb");
            var property3Id = Guid.Parse("c3d4e5f6-3456-789a-cdef-cccccccccccc");
            var property4Id = Guid.Parse("c4d5e6f7-4567-89ab-def0-dddddddddddd");
            var property5Id = Guid.Parse("c5d6e7f8-5678-9abc-ef01-eeeeeeeeeeee");
            var property6Id = Guid.Parse("c6d7e8f9-6789-abcd-f012-ffffffffffff");

            builder.Entity<Property>().HasData(

                new Property
                {
                    Id = property1Id,
                    Title = "Luxury 3-Bedroom Apartment in Lekki Phase 1",
                    Description = "A stunning fully furnished apartment with ocean views, 24/7 power supply, and top-tier security. Perfect for business travellers and families.",
                    Address = "15 Admiralty Way",
                    City = "Lagos",
                    State = "Lagos",
                    Country = "Nigeria",
                    PricePerNightKobo = 7500000,    // ₦75,000 per night
                    MaxGuests = 6,
                    Bedrooms = 3,
                    Bathrooms = 3,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord1ProfileId,
                    PropertyType = PropertyType.Apartment,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },

                new Property
                {
                    Id = property2Id,
                    Title = "Modern Studio in Victoria Island",
                    Description = "A sleek, compact studio ideal for solo business travellers. Walking distance to major banks, restaurants, and the Lagos business district.",
                    Address = "42 Adeola Odeku Street",
                    City = "Lagos",
                    State = "Lagos",
                    Country = "Nigeria",
                    PricePerNightKobo = 3500000,    // ₦35,000 per night
                    MaxGuests = 2,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord1ProfileId,
                    PropertyType = PropertyType.Studio,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc)
                },

                new Property
                {
                    Id = property3Id,
                    Title = "Spacious Duplex in Ikeja GRA",
                    Description = "A beautifully maintained 4-bedroom duplex in the serene Ikeja GRA. Comes with a private pool, generator, and dedicated parking for 3 cars.",
                    Address = "8 Mobolaji Johnson Avenue",
                    City = "Lagos",
                    State = "Lagos",
                    Country = "Nigeria",
                    PricePerNightKobo = 12000000,   // ₦120,000 per night
                    MaxGuests = 8,
                    Bedrooms = 4,
                    Bathrooms = 4,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord1ProfileId,
                    PropertyType = PropertyType.Duplex,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
                },

                new Property
                {
                    Id = property4Id,
                    Title = "Cosy Self-Contain in Wuse 2",
                    Description = "A clean, well-maintained self-contain apartment in the heart of Wuse 2. Great for short stays in Abuja with easy access to major roads.",
                    Address = "23 Aminu Kano Crescent",
                    City = "Abuja",
                    State = "FCT",
                    Country = "Nigeria",
                    PricePerNightKobo = 2500000,    // ₦25,000 per night
                    MaxGuests = 2,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord2ProfileId,
                    PropertyType = PropertyType.SelfContain,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc)
                },

                new Property
                {
                    Id = property5Id,
                    Title = "Executive Penthouse in Maitama",
                    Description = "A breathtaking penthouse with panoramic views of Abuja. Floor-to-ceiling windows, a rooftop terrace, and premium finishes throughout.",
                    Address = "5 Gana Street",
                    City = "Abuja",
                    State = "FCT",
                    Country = "Nigeria",
                    PricePerNightKobo = 25000000,   // ₦250,000 per night
                    MaxGuests = 6,
                    Bedrooms = 3,
                    Bathrooms = 3,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord2ProfileId,
                    PropertyType = PropertyType.Penthouse,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc)
                },

                new Property
                {
                    Id = property6Id,
                    Title = "Charming Bungalow in New GRA Port Harcourt",
                    Description = "A tastefully furnished 2-bedroom bungalow in New GRA. Quiet neighbourhood, 24-hour security, and close to major shopping centres.",
                    Address = "12 Peter Odili Road",
                    City = "Port Harcourt",
                    State = "Rivers",
                    Country = "Nigeria",
                    PricePerNightKobo = 4500000,    // ₦45,000 per night
                    MaxGuests = 4,
                    Bedrooms = 2,
                    Bathrooms = 2,
                    IsAvailable = true,
                    IsApproved = true,
                    LandlordProfileId = landlord2ProfileId,
                    PropertyType = PropertyType.Bungalow,
                    ListingType = ListingType.EntirePlace,
                    CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc)
                }
            );


            builder.Entity<PropertyImage>().HasData(

    // Property 1 — Lekki Apartment
    new PropertyImage { Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"), PropertyId = property1Id, ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d1000000-0000-0000-0000-000000000002"), PropertyId = property1Id, ImageUrl = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d1000000-0000-0000-0000-000000000003"), PropertyId = property1Id, ImageUrl = "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },

    // Property 2 — VI Studio
    new PropertyImage { Id = Guid.Parse("d2000000-0000-0000-0000-000000000001"), PropertyId = property2Id, ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d2000000-0000-0000-0000-000000000002"), PropertyId = property2Id, ImageUrl = "https://images.unsplash.com/photo-1536376072261-38c75010e6c9?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },

    // Property 3 — Ikeja Duplex
    new PropertyImage { Id = Guid.Parse("d3000000-0000-0000-0000-000000000001"), PropertyId = property3Id, ImageUrl = "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d3000000-0000-0000-0000-000000000002"), PropertyId = property3Id, ImageUrl = "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d3000000-0000-0000-0000-000000000003"), PropertyId = property3Id, ImageUrl = "https://images.unsplash.com/photo-1576941089067-2de3c901e126?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Property 4 — Wuse Self Contain
    new PropertyImage { Id = Guid.Parse("d4000000-0000-0000-0000-000000000001"), PropertyId = property4Id, ImageUrl = "https://images.unsplash.com/photo-1554995207-c18c203602cb?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d4000000-0000-0000-0000-000000000002"), PropertyId = property4Id, ImageUrl = "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc) },

    // Property 5 — Maitama Penthouse
    new PropertyImage { Id = Guid.Parse("d5000000-0000-0000-0000-000000000001"), PropertyId = property5Id, ImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d5000000-0000-0000-0000-000000000002"), PropertyId = property5Id, ImageUrl = "https://images.unsplash.com/photo-1560185007-cde436f6a4d0?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d5000000-0000-0000-0000-000000000003"), PropertyId = property5Id, ImageUrl = "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },

    // Property 6 — Port Harcourt Bungalow
    new PropertyImage { Id = Guid.Parse("d6000000-0000-0000-0000-000000000001"), PropertyId = property6Id, ImageUrl = "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&q=80", IsCoverImage = true, CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyImage { Id = Guid.Parse("d6000000-0000-0000-0000-000000000002"), PropertyId = property6Id, ImageUrl = "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800&q=80", IsCoverImage = false, CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) }
);

            builder.Entity<PropertyAmenity>().HasData(

    // Property 1
    new PropertyAmenity { Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"), PropertyId = property1Id, Name = "WiFi", CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e1000000-0000-0000-0000-000000000002"), PropertyId = property1Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e1000000-0000-0000-0000-000000000003"), PropertyId = property1Id, Name = "24hr Generator", CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e1000000-0000-0000-0000-000000000004"), PropertyId = property1Id, Name = "Security", CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e1000000-0000-0000-0000-000000000005"), PropertyId = property1Id, Name = "Parking", CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },

    // Property 2
    new PropertyAmenity { Id = Guid.Parse("e2000000-0000-0000-0000-000000000001"), PropertyId = property2Id, Name = "WiFi", CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e2000000-0000-0000-0000-000000000002"), PropertyId = property2Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e2000000-0000-0000-0000-000000000003"), PropertyId = property2Id, Name = "24hr Generator", CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },

    // Property 3
    new PropertyAmenity { Id = Guid.Parse("e3000000-0000-0000-0000-000000000001"), PropertyId = property3Id, Name = "Swimming Pool", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e3000000-0000-0000-0000-000000000002"), PropertyId = property3Id, Name = "WiFi", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e3000000-0000-0000-0000-000000000003"), PropertyId = property3Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e3000000-0000-0000-0000-000000000004"), PropertyId = property3Id, Name = "24hr Generator", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e3000000-0000-0000-0000-000000000005"), PropertyId = property3Id, Name = "Gym", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Property 4
    new PropertyAmenity { Id = Guid.Parse("e4000000-0000-0000-0000-000000000001"), PropertyId = property4Id, Name = "WiFi", CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e4000000-0000-0000-0000-000000000002"), PropertyId = property4Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e4000000-0000-0000-0000-000000000003"), PropertyId = property4Id, Name = "Security", CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc) },

    // Property 5
    new PropertyAmenity { Id = Guid.Parse("e5000000-0000-0000-0000-000000000001"), PropertyId = property5Id, Name = "Rooftop Terrace", CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e5000000-0000-0000-0000-000000000002"), PropertyId = property5Id, Name = "WiFi", CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e5000000-0000-0000-0000-000000000003"), PropertyId = property5Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e5000000-0000-0000-0000-000000000004"), PropertyId = property5Id, Name = "Concierge Service", CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e5000000-0000-0000-0000-000000000005"), PropertyId = property5Id, Name = "24hr Generator", CreatedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc) },

    // Property 6
    new PropertyAmenity { Id = Guid.Parse("e6000000-0000-0000-0000-000000000001"), PropertyId = property6Id, Name = "WiFi", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e6000000-0000-0000-0000-000000000002"), PropertyId = property6Id, Name = "Parking", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e6000000-0000-0000-0000-000000000003"), PropertyId = property6Id, Name = "Security", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
    new PropertyAmenity { Id = Guid.Parse("e6000000-0000-0000-0000-000000000004"), PropertyId = property6Id, Name = "Air Conditioning", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) }
);


            // LandlordProfile → AppUser (one-to-one)
            builder.Entity<LandlordProfile>()
                .HasOne(l => l.AppUser)
                .WithOne()
                .HasForeignKey<LandlordProfile>(l => l.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);  // delete profile if user deleted

            // TenantProfile → AppUser (one-to-one)
            builder.Entity<TenantProfile>()
                .HasOne(t => t.AppUser)
                .WithOne()
                .HasForeignKey<TenantProfile>(t => t.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            

            // Property → LandlordProfile (one landlord, many properties)
            builder.Entity<Property>()
                .HasOne(p => p.Landlord)
                .WithMany(l => l.Properties)
                .HasForeignKey(p => p.LandlordProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // PropertyImage → Property
            builder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // PropertyAmenity → Property
            builder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.Amenities)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // UnavailableDate → Property
            builder.Entity<UnavailableDate>()
                .HasOne(u => u.Property)
                .WithMany(p => p.UnavailableDates)
                .HasForeignKey(u => u.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking → Property
            builder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Booking → TenantProfile
            builder.Entity<Booking>()
                .HasOne(b => b.Tenant)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TenantProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment → Booking (one-to-one)
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payout → LandlordProfile
            builder.Entity<Payout>()
                .HasOne(p => p.Landlord)
                .WithMany(l => l.Payouts)
                .HasForeignKey(p => p.LandlordProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            //RefreshToken 
            builder.Entity<RefreshToken>()
                 .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}