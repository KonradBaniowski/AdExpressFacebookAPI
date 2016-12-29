using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class CategoryMapping : EntityTypeConfiguration<Category>
    {

        public CategoryMapping(string schema)
        {
            HasKey(e => new { e.IdCategory });
            ToTable("CATEGORY", schema);
            Property(e => e.IdCategory).HasColumnName("ID_CATEGORY");
            Property(e => e.CategoryLabel).HasColumnName("CATEGORY");
        }

    }
}
