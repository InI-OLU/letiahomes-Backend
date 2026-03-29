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
                NormalizedUserName = "ADMIN@LETIAHOMES.COM",
                Email = "gbolahanagbeleye@gmail.com",          
                NormalizedEmail = "ADMIN@LETIAHOMES.COM",
                EmailConfirmed = true,
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
        }
    }
}