using Facebook.Service.Core.DomainModels.AdExprSchema;
using System.Data.Entity.ModelConfiguration;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class ProductMapping : EntityTypeConfiguration<LevelItem>
    {
        public ProductMapping(string schema)
        {
            HasKey(p => p.ProductId);
            ToTable("ALL_PRODUCT_FACEBOOK", schema);
            Property(p => p.ProductId).HasColumnName("ID_PRODUCT");
            Property(P => P.Product).HasColumnName("PRODUCT");
            Property(p => p.SegmentId).HasColumnName("ID_SEGMENT");
            Property(p => p.Segment).HasColumnName("SEGMENT");
            Property(p => p.GroupId).HasColumnName("ID_GROUP_");
            Property(p => p.Group).HasColumnName("GROUP_");
            Property(p => p.SubSectorId).HasColumnName("ID_SUBSECTOR");
            Property(p => p.SubSector).HasColumnName("SUBSECTOR");
            Property(p => p.SectorId).HasColumnName("ID_SECTOR");
            Property(P => P.Sector).HasColumnName("SECTOR");
            Property(p => p.BrandId).HasColumnName("ID_BRAND");
            Property(p => p.Brand).HasColumnName("BRAND");
            Property(p => p.AdvertiserId).HasColumnName("ID_ADVERTISER");
            Property(P => P.Advertiser).HasColumnName("ADVERTISER");
            Property(p => p.HoldingCompanyId).HasColumnName("ID_HOLDING_COMPANY");
            Property(p => p.HoldingCompany).HasColumnName("HOLDING_COMPANY");
            Property(p => p.LanguageId).HasColumnName("ID_LANGUAGE");
        }
    }
}
