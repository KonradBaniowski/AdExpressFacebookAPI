using KM.AdExpress.Health.Core.Model;
using Oracle.ManagedDataAccess.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace KM.AdExpress.Health.Infrastructure
{
    public class HealthContext : DbContext
    {
        const string SCHEMAHEALTH = "KHEALTH01";
        const string CONNECTIONSTRING = "KeyConnectionDBKHealth";

        public HealthContext() : base(CONNECTIONSTRING)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assemblyTypes = GetType().Assembly.GetTypes().ToList();
            assemblyTypes.Where(p => p.Namespace != null
                && p.Namespace.Contains("Mapping")
                && p.BaseType != null
                && p.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList().ForEach(
                p =>
                {
                    dynamic mapping = Activator.CreateInstance(p, SCHEMAHEALTH);
                    modelBuilder.Configurations.Add(mapping);
                });
        }

        public DbSet<Canal> Canal { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<DataCost> DataCost { get; set; }
        public DbSet<Format> Format { get; set; }
        public DbSet<GrpPharma> GrpPharma { get; set; }
        public DbSet<Laboratory> Laboratory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Specialist> Specialist { get; set; }

        
        public class ModelConfiguration : DbConfiguration
        {
            public ModelConfiguration()
            {
                SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
            }
        }
    }
}
