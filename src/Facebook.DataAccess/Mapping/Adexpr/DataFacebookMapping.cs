using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class DataFacebookMapping : EntityTypeConfiguration<DataFacebook>
    {
        public DataFacebookMapping(string schema)
        {
            HasKey(e => new { e.IdMedia, e.DateMediaNum, e.IdDataFacebook });
            ToTable("DATA_FACEBOOK", schema);
            Property(e => e.IdMedia).HasColumnName("ID_MEDIA");
            Property(e => e.DateMediaNum).HasColumnName("DATE_MEDIA_NUM");
            Property(e => e.IdDataFacebook).HasColumnName("ID_DATA_FACEBOOK");
            Property(e => e.IdLanguageData).HasColumnName("ID_LANGUAGE_DATA_I");
            Property(e => e.Expenditure).HasColumnName("EXPENDITURE");
            Property(e => e.ExpenditureLocal).HasColumnName("EXPENDITURE_LOCAL");
            Property(e => e.NumberFan).HasColumnName("NUMBER_FAN");
            Property(e => e.NumberLocalFan).HasColumnName("NUMBER_LOCAL_FAN");
            Property(e => e.NumberPost).HasColumnName("NUMBER_POST");
            Property(e => e.NumberAction).HasColumnName("NUMBER_ACTION");
            Property(e => e.NumberLike).HasColumnName("NUMBER_LIKE");
            Property(e => e.NumberComment).HasColumnName("NUMBER_COMMENT");
            Property(e => e.NumberShare).HasColumnName("NUMBER_SHARE");
            Property(e => e.IdPageFacebook).HasColumnName("ID_PAGE_FACEBOOK");
            Property(e => e.PageName).HasColumnName("PAGE_NAME");
            Property(e => e.IdPage).HasColumnName("ID_PAGE");
            Property(e => e.IdProduct).HasColumnName("ID_PRODUCT");
            Property(e => e.Url).HasColumnName("URL");
            Property(e => e.GlobalType).HasColumnName("GLOBAL_TYPE");
            Property(e => e.MainCategory).HasColumnName("MAIN_CATEGORY");
            Property(e => e.PictureProfileUrl).HasColumnName("PICTURE_PROFILE_URL");
            Property(e => e.VerifiedPage).HasColumnName("VERIFIED_PAGE");
            Property(e => e.IdSegment).HasColumnName("ID_SEGMENT");
            Property(e => e.IdGroup).HasColumnName("ID_GROUP_");
            Property(e => e.IdSubSector).HasColumnName("ID_SUBSECTOR");
            Property(e => e.IdSector).HasColumnName("ID_SECTOR");
            Property(e => e.IdAdvertiser).HasColumnName("ID_ADVERTISER");
            Property(e => e.IdHoldingCompany).HasColumnName("ID_HOLDING_COMPANY");
            Property(e => e.IdBrand).HasColumnName("ID_BRAND");
            Property(e => e.IdBasicMedia).HasColumnName("ID_BASIC_MEDIA");
            Property(e => e.IdCategory).HasColumnName("ID_CATEGORY");
            Property(e => e.IdInterestCenter).HasColumnName("ID_INTEREST_CENTER");
            Property(e => e.IdVehicle).HasColumnName("ID_VEHICLE");
            Property(e => e.IdPeriodicity).HasColumnName("ID_PERIODICITY");
            Property(e => e.IdMediaSeller).HasColumnName("ID_MEDIA_SELLER");
            Property(e => e.IdTitle).HasColumnName("ID_TITLE");
            Property(e => e.IdCountry).HasColumnName("ID_COUNTRY");
            Property(e => e.IdMediaGroup).HasColumnName("ID_MEDIA_GROUP");
            Property(e => e.IdGroupAdvertisingAgency).HasColumnName("ID_GROUP_ADVERTISING_AGENCY");
            Property(e => e.IdAdvertisingAgency).HasColumnName("ID_ADVERTISING_AGENCY");

            HasRequired(e => e.Advertiser).WithMany().HasForeignKey(z => new { z.IdAdvertiser, z.IdLanguageData });
        }
    }
}