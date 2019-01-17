using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.AdExpress.Web.Core.Turkey.Utilities
{
    public class ProductDetailLevel : Core.Utilities.ProductDetailLevel
    {

        public ProductDetailLevel() : base() {}

        /// <summary>
        /// Get Default Generic Detail Level Ids
        /// </summary>
        /// <param name="currentModule">current Module</param>
        /// <returns>Array List etail Level Ids</returns>
        public override ArrayList GetDefaultGenericDetailLevelIds(long currentModule)
        {
            #region Niveau de détail media (Generic)
            ArrayList levels = new ArrayList();
            switch (currentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                    // Famille
                    levels.Add(11);
                    levels.Add(8);
                    levels.Add(9);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                    levels.Add(15);
                    levels.Add(16);
                    levels.Add(8);
                    break;
                default:
                    levels.Add(8);
                    break;
            }
            return levels;
            #endregion
        }
    }
}
