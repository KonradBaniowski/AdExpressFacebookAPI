using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class OneLevelRussiaShowLinkRules : OneLevelShowLinkRules {

        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        public OneLevelRussiaShowLinkRules(CellLevel cellLevel, GenericDetailLevel genericDetailLevel)
            :base(cellLevel,genericDetailLevel){
        }
        #endregion

        /// <summary>
        /// Indique si le lien doit être montrée dans la Cellule
        /// </summary>
        /// <returns>True s'il doit être montré, false sinon</returns>
        public override bool ShowLink()
        {
            if (_cellLevel.Level > 0)
            {
                switch (_genericDetailLevel.GetDetailLevelItemInformation(_cellLevel.Level))
                {
                    case DetailLevelItemInformation.Levels.product:
                    case DetailLevelItemInformation.Levels.advertiser:
                    case DetailLevelItemInformation.Levels.segment:
                    case DetailLevelItemInformation.Levels.brand:
                    case DetailLevelItemInformation.Levels.subBrand:
                        return true;
                    default: return false;
                }
            }
            return (false);
        }
    }
}
