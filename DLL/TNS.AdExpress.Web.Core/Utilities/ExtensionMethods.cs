using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Core.Utilities
{

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        public static List<DetailLevelItemInformation.Levels> ConvertToDetailLevelItemInformation(this Universe universe)
        {
        
            var levels = new List<DetailLevelItemInformation.Levels>();
            foreach (long univeseLevel in universe.GetLevelListId())
            {
                switch (univeseLevel)
                {

                    case TNSClassificationLevels.SECTOR:
                        levels.Add(DetailLevelItemInformation.Levels.sector); break;
                    case TNSClassificationLevels.SUB_SECTOR: 
                        levels.Add(DetailLevelItemInformation.Levels.subSector); break;
                    case TNSClassificationLevels.GROUP_:
                        levels.Add(DetailLevelItemInformation.Levels.group); break;
                    case TNSClassificationLevels.SEGMENT:
                        levels.Add(DetailLevelItemInformation.Levels.segment); break;
                    case TNSClassificationLevels.HOLDING_COMPANY:
                        levels.Add(DetailLevelItemInformation.Levels.holdingCompany); break;
                    case TNSClassificationLevels.ADVERTISER:
                        levels.Add(DetailLevelItemInformation.Levels.advertiser); break;
                    case TNSClassificationLevels.BRAND: 
                        levels.Add(DetailLevelItemInformation.Levels.brand); break;
                    case TNSClassificationLevels.PRODUCT: 
                        levels.Add(DetailLevelItemInformation.Levels.product); break;
                    default:
                        throw (new Exception("Unknown classification identifier"));
                }

            }
            return levels;
        }

    }
}
