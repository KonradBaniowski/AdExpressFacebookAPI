using KM.AdExpress.Health.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Mapping
{
    public class DataCostMapping : EntityTypeConfiguration<DataCost>
    {
        public DataCostMapping(string schema)
        {
            ToTable("DATA_COST", schema);
            Property(e => e.IdCanal).HasColumnName("ID_CANAL");
            Property(e => e.IdCategory).HasColumnName("ID_CATEGORY");
            Property(e => e.IdFormat).HasColumnName("ID_CONDITIONNEMENT");
            Property(e => e.IdGrpPharma).HasColumnName("ID_GRP_PHARMA");
            Property(e => e.IdLaboratory).HasColumnName("ID_LABORATOIRE");
            Property(e => e.IdProduct).HasColumnName("ID_PRODUCT");
            Property(e => e.IdSpecialist).HasColumnName("ID_MEDECIN");
            Property(e => e.Date).HasColumnName("DATE_CANAL");
            Property(e => e.Euro).HasColumnName("EXPENDITURE");


            HasKey(e => new { e.IdCanal, e.IdCategory, e.IdFormat, e.IdGrpPharma, e.IdProduct, e.IdSpecialist, e.IdLaboratory, e.Date });
            HasRequired(e => e.Canal).WithMany().HasForeignKey(z => new { z.IdCanal });
            HasRequired(e => e.Category).WithMany().HasForeignKey(z => new { z.IdCategory });
            HasRequired(e => e.Format).WithMany().HasForeignKey(z => new { z.IdFormat });
            HasRequired(e => e.GrpPharma).WithMany().HasForeignKey(z => new { z.IdGrpPharma });
            HasRequired(e => e.Product).WithMany().HasForeignKey(z => new { z.IdProduct });
            HasRequired(e => e.Specialist).WithMany().HasForeignKey(z => new { z.IdSpecialist });
            HasRequired(e => e.Laboratory).WithMany().HasForeignKey(z => new { z.IdLaboratory });

        }
    }
}
