using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Mau
{
    public class TemplateMapping : EntityTypeConfiguration<Template>
    {
        public TemplateMapping(string schema)
        {
            HasKey(e => e.IdTemplate);
            ToTable("TEMPLATE", schema);
            Property(e => e.IdTemplate).HasColumnName("ID_TEMPLATE");
            Property(e => e.IdProject).HasColumnName("ID_PROJECT");
            Property(e => e.Activation).HasColumnName("ACTIVATION");
        }
    }
}
