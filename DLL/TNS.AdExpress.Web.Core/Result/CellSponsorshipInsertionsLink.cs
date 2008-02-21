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
    /// <summary>
    /// Cellule image ayant un lien vers le détail des insertions du parrainage
    /// </summary>
    public class CellSponsorshipInsertionsLink:CellAdExpressInsertionsLink {
        
        #region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        public CellSponsorshipInsertionsLink(CellLevel cellLevel, WebSession webSession, GenericDetailLevel genericDetailLevel)
            : base(cellLevel, webSession, genericDetailLevel) {
            _linkRules=new SponsorshipShowLinkRules(cellLevel,webSession,genericDetailLevel);
			_link = "javascript:OpenCreation('{0}','{1},-1,1','');";
		}
		#endregion

        #region Implémentation de CellImageLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if(_linkRules.ShowLink()) {
                return (string.Format(_link,_webSession.IdSession,_linkRules.GetHierarchy()));
            }
            return ("");
        }
        #endregion

    }
}
