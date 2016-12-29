using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class SpecialistMapping : EntityTypeConfiguration<Specialist>
    {
        public SpecialistMapping(string schema)
        {
            HasKey(e => new { e.IdSpecialist });
            ToTable("MEDECIN", schema);
            Property(e => e.IdSpecialist).HasColumnName("ID_MEDECIN");
            Property(e => e.SpecialistLabel).HasColumnName("MEDECIN");
        }
    }
}
