using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsermmanagementWithIdentity.Models;

namespace UsermmanagementWithIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define schema and table names for Identity entities
            builder.Entity<ApplicationUser>()
                .ToTable("Users", "Security");
            builder.Entity<IdentityRole>()
                .ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>()
                .ToTable("UserRoles", "Security");
            builder.Entity<IdentityUserClaim<string>>()
                .ToTable("UserClaims", "Security");
            builder.Entity<IdentityUserLogin<string>>()
                .ToTable("UserLogins", "Security");
            builder.Entity<IdentityRoleClaim<string>>()
                .ToTable("RoleClaims", "Security");
            builder.Entity<IdentityUserToken<string>>()
                .ToTable("UserTokens", "Security");

            // Define composite keys for Identity entities that need them
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(login => new { login.LoginProvider, login.ProviderKey });

            builder.Entity<IdentityUserRole<string>>()
                .HasKey(userRole => new { userRole.UserId, userRole.RoleId });

            builder.Entity<IdentityUserToken<string>>()
                .HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
        }
    }
}
