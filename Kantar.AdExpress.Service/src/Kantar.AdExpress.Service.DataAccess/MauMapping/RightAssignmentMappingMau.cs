using Kantar.AdExpress.Service.Core.Domain.Mau;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.MauMapping
{
    public class RightAssignmentMappingMau: EntityTypeConfiguration<RightAssignment>
    {
        public RightAssignmentMappingMau(string schema)
        {
            HasKey(t => new { t.IdProject, t.IdLogin});
            ToTable("RIGHT_ASSIGNMENT", schema);
            Property(t => t.IdProject).HasColumnName("ID_¨PROJECT");
            Property(t => t.IdLogin).HasColumnName("ID_LOGIN");
            Property(t => t.Activation).HasColumnName("ACTIVATION");
        }
    }
}
