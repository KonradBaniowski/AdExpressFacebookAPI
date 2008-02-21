using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// R�gle de base � appliquer � un cellule pointant vers les cr�ations ou le d�tail des insertions
    /// </summary>
    public class DefaultShowLinkRules:ILinkRules {

        #region Variables
        /// <summary>
        /// Cellule de niveau
        /// </summary>
        protected CellLevel _cellLevel;

        /// <summary>
        /// Niveau de d�tail g�n�rique
        /// </summary>
        protected GenericDetailLevel _genericDetailLevel;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule de niveau cliqu�</param>
        /// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
        public DefaultShowLinkRules(CellLevel cellLevel,GenericDetailLevel genericDetailLevel) {
            if(cellLevel == null) throw (new ArgumentNullException("L'objet cellLevel est null"));
            if(genericDetailLevel == null) throw (new ArgumentNullException("L'objet genericDetailLevel est null"));
            _cellLevel = cellLevel;
            _genericDetailLevel=genericDetailLevel;
        }
        #endregion

        #region IShowLinkRules Membres
        /// <summary>
        /// Obtient la hierarchie de la nomenclature sur 4 niveaux
        /// </summary>
        /// <remarks>
        /// La valeur -1 correspond � un niveau dont il ne faut pas tenir compte
        /// </remarks>
        /// <example>
        /// hierarchie: "1,235,366665,-1"
        /// </example>
        /// <returns>Cha�ne repr�sentant la hierarchie</returns>
        public virtual string GetHierarchy() {
            string[] IdList =  { "-1","-1","-1","-1" };
            string sep = ",";
            CellLevel localCellLevel = _cellLevel;

            while(localCellLevel != null && localCellLevel.Level > 0) {
                IdList[localCellLevel.Level - 1] = localCellLevel.Id.ToString();
                localCellLevel = localCellLevel.ParentLevel;
            }
            return String.Join(sep,IdList);
        }

        /// <summary>
        /// Indique si le lien doit �tre montr�e dans la Cellule
        /// </summary>
        /// <returns>True s'il doit �tre montr�, false sinon</returns>
        public virtual bool ShowLink() {
            if(_cellLevel.Level > 0) {
                switch(_genericDetailLevel.GetDetailLevelItemInformation(_cellLevel.Level)) {
                    case TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.product:
                    case TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.advertiser:
                        return true;
                    default: return false;
                }
            }
            return (false);
        }

        #endregion
    }
}
