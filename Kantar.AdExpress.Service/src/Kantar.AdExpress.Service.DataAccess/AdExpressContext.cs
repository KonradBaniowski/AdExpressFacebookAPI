using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.Mau;
using Kantar.AdExpress.Service.DataAccess.Identity;
using Kantar.AdExpress.Service.DataAccess.MauMapping;
using Microsoft.AspNet.Identity.EntityFramework;
using Oracle.DataAccess.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace Kantar.AdExpress.Service.DataAccess
{
    public class AdExpressContext : DbContext
    {
        const string schemaMau = "MAU01";
        const string schema = "ADEXPR03";
        const string connectionString = "adexpr03";

        public AdExpressContext() : base(connectionString)
        {
            
        }
         

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assemblyTypes = GetType().Assembly.GetTypes().ToList();

            assemblyTypes.Where(p => p.Name.EndsWith("Mapping")
                && p.BaseType != null
                && p.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList().ForEach(
                p =>
                {
                    dynamic mapping = Activator.CreateInstance(p, schema);
                    modelBuilder.Configurations.Add(mapping);
                });
            modelBuilder.Configurations.Add(new LoginMappingMau(schemaMau));
            modelBuilder.Configurations.Add(new ModuleAssignmentMappingMau(schemaMau));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Address> Adress { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<ModuleAssignment> ModuleAssignment { get; set; }
    }

    public class ModelConfiguration : DbConfiguration
    {
        public ModelConfiguration()
        {
            SetProviderServices("Oracle.DataAccess.Client", EFOracleProviderServices.Instance);

        }
    }

    public class IdentityContext : IdentityDbContext<ApplicationIdentityUser, ApplicationIdentityRole, int, ApplicationIdentityUserLogin, ApplicationIdentityUserRole, ApplicationIdentityUserClaim>
    {
        const string connectionLOCAL = "IdentityContext";

        public IdentityContext() : base(connectionLOCAL)
        {
           
        }        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EfIdentityConfig.ConfigEfIdentity(modelBuilder);
        }
    }
}
