using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    class GrpPharmaMapping : EntityTypeConfiguration<GrpPharma>
    {

        public GrpPharmaMapping(string schema)
        {
            HasKey(e => new { e.IdGrpPharma });
            ToTable("GRP_PHARMA", schema);
            Property(e => e.IdGrpPharma).HasColumnName("ID_GRP_PHARMA");
            Property(e => e.GrpPharmaLabel).HasColumnName("GRP_PHARMA");
        }

    }
}
