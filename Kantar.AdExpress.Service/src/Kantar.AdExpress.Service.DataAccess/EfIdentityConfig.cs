using Kantar.AdExpress.Service.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess
{
    public class EfIdentityConfig
    {
        public static void ConfigEfIdentity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationIdentityUser>()
                 .Property(e => e.Id)
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ApplicationIdentityRole>()
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ApplicationIdentityUserClaim>()
                 .Property(e => e.Id)
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
