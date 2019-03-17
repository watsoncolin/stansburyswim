using FillThePool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillThePool.Data.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            this.Property(p => p.UserId).HasColumnOrder(0);
            this.Property(p => p.UserName).IsRequired().HasMaxLength(200);
            this.Property(p => p.FirstName).IsOptional().HasMaxLength(100);
            this.Property(p => p.LastName).IsOptional().HasMaxLength(100);
            this.Property(p => p.Email).IsRequired().HasMaxLength(200);
            this.HasMany(a => a.Roles).WithMany(b => b.Users).Map(m =>
                {
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                    m.ToTable("webpages_UsersInRoles");
                });
        }
    }
}
