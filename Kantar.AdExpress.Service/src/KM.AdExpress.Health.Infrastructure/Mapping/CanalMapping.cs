using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class CanalMapping : EntityTypeConfiguration<Canal>
    {

        public CanalMapping(string schema)
        {
            HasKey(e => new { e.IdCanal });
            ToTable("CANAL", schema);
            Property(e => e.IdCanal).HasColumnName("ID_CANAL");
            Property(e => e.CanalLabel).HasColumnName("CANAL");
        }

    }
}
