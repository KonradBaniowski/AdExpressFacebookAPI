using Kantar.AdExpress.Service.Core.Domain.Mau;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.MauMapping
{
    public class ModuleAssignmentMappingMau : EntityTypeConfiguration<ModuleAssignment>
    {
        public ModuleAssignmentMappingMau(string schema)
        {
            HasKey(t => new { t.IdModule, t.IdLogin });
            ToTable("MODULE_ASSIGNMENT", schema);
            Property(t => t.IdModule).HasColumnName("ID_MODULE");
            Property(t => t.IdLogin).HasColumnName("ID_LOGIN");
            Property(t => t.DateDebutModule).HasColumnName("DATE_BEGINNING_MODULE");
            Property(t => t.DateFinModule).HasColumnName("DATE_END_MODULE");
            Property(t => t.Activation).HasColumnName("ACTIVATION");
        }
    }
}
