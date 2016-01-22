using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class SponsorshipShowLinkRules:DefaultShowLinkRules {


        #region Variables
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _webSession;
        #endregion


        #region Constructor
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        public SponsorshipShowLinkRules(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
            : base(cellLevel,genericDetailLevel) {
            if(webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _webSession = webSession;
        }
        #endregion

        #region IShowLinkRules Membres
        /// <summary>
        /// Indique si le lien doit être montrée dans la Cellule
        /// </summary>
        /// <returns>True s'il doit être montré, false sinon</returns>
        public override bool ShowLink() {
            if(_cellLevel.Level < 1) return (false);
            if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                return (true);
            // Autrement on utilise les règles de base
            return (base.ShowLink());
        }

        #endregion
    }
}
