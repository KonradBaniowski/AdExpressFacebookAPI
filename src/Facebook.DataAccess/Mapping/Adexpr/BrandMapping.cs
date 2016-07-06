using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class BrandMapping : EntityTypeConfiguration<Brand>
    {
        public BrandMapping(string schema)
        {
            HasKey(e => new { e.Id, e.IdLanguage });
            ToTable("BRAND", schema);
            Property(e => e.Id).HasColumnName("ID_BRAND");
            Property(e => e.IdLanguage).HasColumnName("ID_LANGUAGE");
            Property(e => e.BrandLabel).HasColumnName("BRAND");
            Property(e => e.Activation).HasColumnName("ACTIVATION");
        }
    }
}
