using Facebook.Service.Core.DomainModels.MauSchema;
using System.Data.Entity.ModelConfiguration;

namespace Facebook.DataAccess.Mapping.Mau
{
    public class OrderClientMediaMapping : EntityTypeConfiguration<OrderClientMedia>
    {
        public OrderClientMediaMapping(string schema)
        {
            HasKey(e => e.Id);
            ToTable("ORDER_CLIENT_MEDIA", schema);
            Property(t => t.Id).HasColumnName("ID_ORDER_CLIENT_MEDIA");
            Property(_ => _.ListMedia).HasColumnName("LIST_MEDIA");
        }
    }
}
