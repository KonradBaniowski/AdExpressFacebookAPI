using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class OneLevelShowLinkRules:DefaultShowLinkRules {

        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        public OneLevelShowLinkRules(CellLevel cellLevel,GenericDetailLevel genericDetailLevel)
            :base(cellLevel,genericDetailLevel){
        }
        #endregion

        ///// <summary>
        ///// Obtient la hierarchie de la nomenclature sur 4 niveaux
        ///// </summary>
        ///// <remarks>
        ///// La valeur long.MinValue correspond à un niveau dont il ne faut pas tenir compte
        ///// </remarks>
        ///// <example>
        ///// hierarchie: "1,235,366665,long.MinValue" ou "long.MinValue,long.MinValue,3336,long.MinValue"
        ///// </example>
        ///// <returns>Chaîne représentant la hierarchie</returns>
        //public override string GetHierarchy() {
        //    //string[] IdList =  { "-1","-1","-1","-1" };
        //    string[] IdList = { long.MinValue.ToString(), long.MinValue.ToString(), long.MinValue.ToString(), long.MinValue.ToString() };
        //    string sep=",";
        //    IdList[_cellLevel.Level-1]=_cellLevel.Id.ToString();
        //    return (String.Join(sep,IdList));
        //}
    }
}
