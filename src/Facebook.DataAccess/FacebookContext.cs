using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.MauSchema;
using Oracle.ManagedDataAccess.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace Facebook.DataAccess
{

    public class FacebookContext : DbContext
    {
        const string schemaMau = "MAU01";
        const string schema = "ADEXPR03";
        const string connectionString = "adexpr03";

        public FacebookContext() : base(connectionString)

        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assemblyTypes = GetType().Assembly.GetTypes().ToList();
            //GetType().Assembly.GetTypes().ToList().Where(e => e.FullName.Contains("Mau"))
            assemblyTypes.Where(p => p.Namespace != null 
                && p.Namespace.Contains("Mau")
                && p.BaseType != null
                && p.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList().ForEach(
                p =>
                {
                    dynamic mapping = Activator.CreateInstance(p, schemaMau);
                    modelBuilder.Configurations.Add(mapping);
                });
            var assemblyTypes2 = GetType().Assembly.GetTypes().ToList();

            assemblyTypes2.Where(p => p.Namespace != null
                && p.Namespace.Contains("Adexpr")
                && p.BaseType != null
                && p.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList().ForEach(
                p =>
                {
                    dynamic mapping = Activator.CreateInstance(p, schema);
                    modelBuilder.Configurations.Add(mapping);
                });
        }

        public DbSet<OrderClientMedia> OrderClientMedia { get; set; }
        public DbSet<OrderTemplateMedia> OrderTemplateMedia { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<LevelItem> Products { get; set; }
        public DbSet<DataFacebook> DataFacebook { get; set; }
        public DbSet<Advertiser> Advertiser { get; set; }
    }

    public class ModelConfiguration : DbConfiguration
    {
        public ModelConfiguration()
        {
            SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
        }
    }

}