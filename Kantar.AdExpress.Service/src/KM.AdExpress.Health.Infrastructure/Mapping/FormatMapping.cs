using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class FormatMapping : EntityTypeConfiguration<Format>
    {
        public FormatMapping(string schema)
        {
            HasKey(e => new { e.IdFormat });
            ToTable("CONDITIONNEMENT", schema);
            Property(e => e.IdFormat).HasColumnName("ID_CONDITIONNEMENT");
            Property(e => e.FormatLabel).HasColumnName("CONDITIONNEMENT");
        }
    }
}
