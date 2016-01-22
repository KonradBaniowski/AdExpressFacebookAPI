using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data.DataProvider;
using KM.AdExpress.AlertPreRoll.Domain;

namespace KM.AdExpress.AlertPreRoll.DAL
{
    public class PreRollDAL
    {
          protected readonly object _instance = new object();
        public PreRollDbConnectionFactory PreRollDb { get; set; }
        private static Int64 _idLanguage=33;
        private const int ID_GROUP_FORMAT_BANNERS_PRE_ROLL = 2;

        public PreRollDAL(String connexionString, String providerName)
        {
            PreRollDb = new PreRollDbConnectionFactory(connexionString, new GenericDataProvider(providerName));
           
        }

        public List<PreRollItem> Select(PreRollDb db, DateTime start, DateTime end)
        {
            List<PreRollItem> preRollItems;

            var strQuery = new StringBuilder();

            strQuery.Append(" select  IdProduct , Product,IdAdvertiser, Advertiser");
            strQuery.Append(" ,IdSector,Sector,IdSubSector, SubSector,IdGroup,Group_");
            strQuery.Append(" , IdSegment,Segment,IdCategory,CATEGORY, IdMedia,Media");
            strQuery.Append(" ,nvl(id_slogan,0) as Version, date_media_num as DateMediaNum");
            strQuery.Append(" ,sum(Occurence) as Occurence,associated_file as Url");
            strQuery.Append("  from ((");
            strQuery.Append("  select");
            strQuery.Append(" ap.id_product as IdProduct,ap.product as Product,ap.id_advertiser as IdAdvertiser,ap.advertiser  as Advertiser");
            strQuery.Append(" ,ap.id_sector as IdSector,ap.sector  as Sector,ap.ID_SUBSECTOR as IdSubSector, SUBSECTOR as SubSector");
            strQuery.Append(" ,ap.ID_GROUP_ as IdGroup, GROUP_  as Group_, ap.ID_SEGMENT as IdSegment, ap.SEGMENT as Segment");
            strQuery.Append(" ,am.ID_CATEGORY as IdCategory, am.CATEGORY as Category, am.ID_MEDIA as IdMedia, MEDIA as Media");
            strQuery.Append(" ,nvl(hashcode,0) as id_slogan  , DATE_MEDIA_NUM,sum(insertion) as Occurence, associated_file");
            strQuery.Append(" from ADEXPR03.ALL_PRODUCT_33 ap, ADEXPR03.ALL_MEDIA_33 am, adexpr03.data_evaliant_4M wp");
            strQuery.Append("  where  ap.id_product=wp.id_product");
            strQuery.Append("  and am.id_media = wp.id_media and am.id_vehicle = wp.id_vehicle");
            strQuery.Append("  and am.id_category = wp.id_category");
            strQuery.AppendFormat(" and ((date_media_num>={0} and date_media_num<={1}))",start.ToString("yyyyMMdd"),end.ToString("yyyyMMdd"));
            strQuery.Append(" and wp.id_product not in ( 50000,50001,180000 )");
            strQuery.AppendFormat("  and wp.ID_group_format_banners in ({0})", ID_GROUP_FORMAT_BANNERS_PRE_ROLL);
            strQuery.Append(" Group by ap.id_product,ap.product,ap.id_advertiser,ap.advertiser,ap.id_sector,ap.sector");
            strQuery.Append("  ,ap.ID_SUBSECTOR, SUBSECTOR, ap.ID_GROUP_ , GROUP_  , ap.ID_SEGMENT , SEGMENT");
            strQuery.Append(" ,am.ID_CATEGORY, am.CATEGORY, am.ID_MEDIA, MEDIA");
            strQuery.Append(" ,hashcode, date_media_num,associated_file");
            strQuery.Append(" ))");
            strQuery.Append(" group by IdProduct,Product,IdAdvertiser,Advertiser,IdSector,sector");
            strQuery.Append(" ,IdSubSector, SubSector,IdGroup,Group_");
            strQuery.Append(" , IdSegment,Segment,IdCategory,CATEGORY, IdMedia,Media");
            strQuery.Append("  ,id_slogan, date_media_num ,associated_file");
            strQuery.Append(" order by Product,IdProduct,Advertiser,IdAdvertiser,sector,IdSector");
            strQuery.Append("   , SubSector,IdSubSector,Group_  ,IdGroup");
            strQuery.Append(" , IdSegment,Segment,IdCategory,CATEGORY, IdMedia,Media");
            strQuery.Append(" ,id_slogan, date_media_num");
            


            var dbCmd = db.SetCommand(strQuery.ToString());
            return db.ExecuteList<PreRollItem>();
        }
    }
}
