using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Mau
{
    public class OrderClientProductMapping : EntityTypeConfiguration<OrderClientProduct>
    {
        public OrderClientProductMapping(string schema)
        {
            HasKey(e => e.Id);
            ToTable("ORDER_CLIENT_PRODUCT", schema);
            Property(t => t.Id).HasColumnName("ID_ORDER_CLIENT_PRODUCT");
            Property(_ => _.ListMedia).HasColumnName("LIST_MEDIA");
        }
    }
}
