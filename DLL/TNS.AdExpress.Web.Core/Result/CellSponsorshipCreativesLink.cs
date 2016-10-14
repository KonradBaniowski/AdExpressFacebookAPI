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

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink()
        {
            if (ShowLink())
            {
                _link = "ids={0}&zoomDate={1}&idUnivers={2}&moduleId={3}";
                if (_moduleId < 0) _moduleId = _webSession.CurrentModule;
                object[] args = { _linkRules.GetHierarchy(), _zoomDate, _universId, _moduleId };
                return (string.Format(_link, args));
            }
            return ("");
        }
        #endregion

    }
}
