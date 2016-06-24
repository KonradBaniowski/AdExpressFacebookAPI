using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class AdvertiserMapping: EntityTypeConfiguration<Advertiser>
    {
        public AdvertiserMapping(string schema)
        {
            HasKey(e => new { e.IdAdvertiser, e.IdLanguage });
            ToTable("ADVERTISER", schema);
            Property(e => e.IdAdvertiser).HasColumnName("ID_ADVERTISER");
            Property(e => e.IdLanguage).HasColumnName("ID_LANGUAGE");
            Property(e => e.IdHoldingcompany).HasColumnName("ID_HOLDING_COMPANY");
            Property(e => e.AdvertiserLabel).HasColumnName("ADVERTISER");
            Property(e => e.Activation).HasColumnName("ACTIVATION");
        }
    }
}
