using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Mau
{
    public class TemplateAssignmentMapping : EntityTypeConfiguration<TemplateAssignment>
    {
        public TemplateAssignmentMapping(string schema)
        {
            HasKey(e => new { e.IdTemplate, e.IdProject, e.IdLogin});
            ToTable("TEMPLATE_ASSIGNMENT", schema);
            Property(e => e.IdLogin).HasColumnName("ID_LOGIN");
            Property(e => e.IdProject).HasColumnName("ID_PROJECT");
            Property(e => e.IdTemplate).HasColumnName("ID_TEMPLATE");
            Property(e => e.Activation).HasColumnName("ACTIVATION");

            HasRequired(e => e.Template).WithMany().HasForeignKey(e => e.IdTemplate);
        }
    }
}
