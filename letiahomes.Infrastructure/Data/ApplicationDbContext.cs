using letiahomes.Domain.Common;
using letiahomes.Domain.Entities;
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