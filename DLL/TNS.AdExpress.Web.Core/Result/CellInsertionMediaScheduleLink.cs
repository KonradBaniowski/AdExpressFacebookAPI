using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// Cellule créant un lien vers le plan media à partir du détail insertion du portefeuille
    /// </summary>
    public class CellInsertionMediaScheduleLink:CellAdExpressImageLink {

        #region Variables
        /// <summary>
        /// Identifiant du produit
        /// </summary>
        private Int64 _productId;
        /// <summary>
        /// Identifiant du media
        /// </summary>
        private Int64 _vehicleId;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="webSession">Identifiant du produit</param>
        /// <param name="productId">Identifiant du produit</param>
        /// <param name="vehicleId">Identifiant du media</param>
        public CellInsertionMediaScheduleLink(WebSession webSession,Int64 productId,long vehicleId) {
            if(webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _webSession=webSession;
            _productId=productId;
            _vehicleId=vehicleId;
            _imagePath = "/App_Themes/"+WebApplicationParameters.Themes[_webSession.SiteLanguage].Name+"/Images/Common/picto_plus.gif";
            _link="javascript:OpenMediaPlanAlert('{0}','{1}','{2}');";
        }
        #endregion

        #region Implémentation de CellImageLink
        /// <summary>
        /// Obtient le lien de l'image
        /// </summary>
        /// <returns>Lien de l'image</returns>
        public override string GetLink() {
            return (string.Format(_link,_webSession.IdSession,_productId.ToString(),_vehicleId.ToString()));
        }
        #endregion
    }
}
