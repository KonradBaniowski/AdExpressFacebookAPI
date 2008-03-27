#region Informations
// Auteur: Y. R'kaina
// Date de création: 20/08/2007
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Result {

    /// <summary>
    /// cellule contenant le lien vers les créations TV
    /// </summary>
    public class CellTvCreativeLink : CellAdExpressImageLink {

        #region Variables
        /// <summary>
        /// Id solgan
        /// </summary>
        protected Int64 _creative;
        /// <summary>
        /// Id du vehicle (Tv ou others)
        /// </summary>
        protected int _vehicleId;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        public CellTvCreativeLink(Int64 creative, WebSession webSession, int vehicleId) {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _link = "javascript:openDownload('{0}','{1}','{2}');";
            _creative = creative;
            _webSession = webSession;
            _vehicleId = vehicleId;
            _imagePath = "/App_Themes/"+WebApplicationParameters.Themes[_webSession.SiteLanguage].Name+"/Images/Common/Picto_pellicule.gif";
        }
        #endregion

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if (_creative != -1)
                return (string.Format(_link, _creative, _webSession.IdSession, _vehicleId));
            else
                return "";
        }
        #endregion

    }
}
