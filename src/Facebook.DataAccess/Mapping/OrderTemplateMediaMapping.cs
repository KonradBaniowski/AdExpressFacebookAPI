using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping
{
    public class OrderTemplateMediaMapping : EntityTypeConfiguration<OrderTemplateMedia>
    {
        public OrderTemplateMediaMapping(string schema)
        {
            HasKey(e => e.IdOrderTemplateMedia);
            ToTable("ORDER_TEMPLATE_MEDIA", schema);
            Property(e => e.IdOrderTemplateMedia).HasColumnName("ID_ORDER_TEMPLATE_MEDIA");
            Property(e => e.IdTypeMedia).HasColumnName("ID_TYPE_MEDIA");
            Property(e => e.IdTemplate).HasColumnName("ID_TEMPLATE").IsOptional();
            Property(_ => _.ListMedia).HasColumnName("LIST_MEDIA");

            HasRequired(e => e.TypeMedia).WithMany().HasForeignKey(e => e.IdTypeMedia);
        }
    }
}
