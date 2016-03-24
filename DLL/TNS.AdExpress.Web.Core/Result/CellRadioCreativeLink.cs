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
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Core.Result {

    /// <summary>
    /// cellule contenant le lien vers les créations Radio
    /// </summary>
    public class CellRadioCreativeLink : CellAdExpressImageLink {

        #region Variables
        /// <summary>
        /// Id solgan
        /// </summary>
        protected string _creative;
        /// <summary>
        /// Media type ID
        /// </summary>
        protected long _vehicleId;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        //public CellRadioCreativeLink(string creative, WebSession webSession, long vehicleId)
        //{
        //    if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
        //    _link = "javascript:openDownload('{0}','{1}','{2}');";
        //    _creative = creative;
        //    _webSession = webSession;
        //    _imagePath = "/App_Themes/"+WebApplicationParameters.Themes[webSession.SiteLanguage].Name+"/Images/Common/Picto_Radio.gif";
        //    _vehicleId = vehicleId;
        //}


        public CellRadioCreativeLink(string creative, WebSession webSession, long vehicleId)
        {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _link = "{0},{1},{2}";
            _creative = creative;
            _webSession = webSession;
            _imagePath = "/App_Themes/" + WebApplicationParameters.Themes[webSession.SiteLanguage].Name + "/Images/Common/Picto_Radio.gif";
            _vehicleId = vehicleId;
        }

        #endregion

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if (_creative.Length > 0)
            {
                return (string.Format(_link, _creative, _webSession.IdSession, _vehicleId));
            }
            else
                return "";
        }
        #endregion

    }
}
