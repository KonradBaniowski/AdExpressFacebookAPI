using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// Cellule image ayant un lien vers les créations du parrainage
    /// </summary>
    public class CellSponsorshipCreativesLink:CellAdExpressCreativesLink {

        #region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        public CellSponsorshipCreativesLink(CellLevel cellLevel, WebSession webSession, GenericDetailLevel genericDetailLevel)
            : base(cellLevel, webSession, genericDetailLevel,"",-1) {
            _linkRules=new SponsorshipShowLinkRules(cellLevel,webSession,genericDetailLevel);
		}
		#endregion
    }
}
