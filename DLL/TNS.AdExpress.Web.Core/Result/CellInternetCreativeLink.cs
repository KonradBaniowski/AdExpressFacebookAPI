#region Informations
// Auteur: D. Mussuma
// Date de création: 05/10/2010
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;


namespace TNS.AdExpress.Web.Core.Result
{
    
    /// <summary>
    /// Cell with link to Internet creatives
    /// </summary>
    public class CellInternetCreativeLink : CellAdExpressImageLink {
    

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        public CellInternetCreativeLink(string creatives, WebSession webSession)
        {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _link = "javascript:OpenInternetCreation('{0}');";
            _creatives = creatives;
            _webSession = webSession;
            _imagePath = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Images/Common/Picto_internet.gif";
        }
        #endregion

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if (_creatives.Length > 0)
                return (string.Format(_link, _creatives));
            else
                return "";
        }
        #endregion
    }
}
