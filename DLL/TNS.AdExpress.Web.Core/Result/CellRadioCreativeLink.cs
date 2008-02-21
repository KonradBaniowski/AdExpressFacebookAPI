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
        /// Id de la session
        /// </summary>
        protected string _sessionId;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        public CellRadioCreativeLink(string creative, string sessionId) {
            _imagePath = "/Images/Common/Picto_Radio.gif";
            _link = "javascript:openDownload('{0}','{1}','{2}');";
            _creative = creative;
            _sessionId = sessionId;
        }
        #endregion

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if (_creative.Length > 0)
                return (string.Format(_link, _creative, _sessionId, Vehicles.names.radio.GetHashCode()));
            else
                return "";
        }
        #endregion

    }
}
