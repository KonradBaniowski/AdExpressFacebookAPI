#region Informations
// Auteur: G. Facon
// Date de création: 21/08/2007
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellCreativesLink:CellAdExpressCreativesLink {

        #region Constructeurs
        /// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
            _linkRules=new DefaultShowLinkRules(cellLevel,genericDetailLevel);
            _link = "javascript:OpenCreatives('{0}','{1},{2}','');";
		}

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de détail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
            : base(cellLevel,webSession,genericDetailLevel,"",-1) {
            _linkRules=new DefaultShowLinkRules(cellLevel,genericDetailLevel);
            _link = "javascript:OpenCreatives('{0}','{1},{2}','');";
        }
		#endregion

        ///// <summary>
        ///// Obtient l'adresse du lien
        ///// </summary>
        ///// <returns>Adresse du lien</returns>
        //public override string GetLink() {
        //    if (_cellLevel.Level > 0 && ShowLink()) {
        //        return (string.Format(_link, _webSession.IdSession, _cellLevel.Id, _cellLevel.Level));
        //    }
        //    return ("");
        //}
    }
}
