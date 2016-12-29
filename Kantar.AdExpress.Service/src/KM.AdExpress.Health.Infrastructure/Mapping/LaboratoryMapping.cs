using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    class LaboratoryMapping : EntityTypeConfiguration<Laboratory>
    {

        public LaboratoryMapping(string schema)
        {
            HasKey(e => new { e.IdLaboratory });
            ToTable("LABORATOIRE", schema);
            Property(e => e.IdLaboratory).HasColumnName("ID_LABORATOIRE");
            Property(e => e.LaboratoryLabel).HasColumnName("LABORATOIRE");
        }

    }
}
