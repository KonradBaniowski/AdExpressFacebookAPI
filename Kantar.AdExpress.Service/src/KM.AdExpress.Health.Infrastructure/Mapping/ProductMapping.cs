using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class ProductMapping : EntityTypeConfiguration<Product>
    {
        public ProductMapping(string schema)
        {
            HasKey(e => new { e.IdProduct });
            ToTable("PRODUCT", schema);
            Property(e => e.IdProduct).HasColumnName("ID_PRODUCT");
            Property(e => e.ProductLabel).HasColumnName("PRODUCT");
        }
    }
}
