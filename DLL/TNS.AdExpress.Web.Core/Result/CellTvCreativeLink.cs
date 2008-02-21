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
    /// cellule contenant le lien vers les créations TV
    /// </summary>
    public class CellTvCreativeLink : CellAdExpressImageLink {

        #region Variables
        /// <summary>
        /// Id solgan
        /// </summary>
        protected Int64 _creative;
        /// <summary>
        /// Id de la session
        /// </summary>
        protected string _sessionId;
        /// <summary>
        /// Id du vehicle (Tv ou others)
        /// </summary>
        protected int _vehicleId;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        public CellTvCreativeLink(Int64 creative, string sessionId, int vehicleId) {
            _imagePath = "/Images/Common/Picto_pellicule.gif";
            _link = "javascript:openDownload('{0}','{1}','{2}');";
            _creative = creative;
            _sessionId = sessionId;
            _vehicleId = vehicleId;
        }
        #endregion

        #region Implémentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if (_creative != -1)
                return (string.Format(_link, _creative, _sessionId, _vehicleId));
            else
                return "";
        }
        #endregion

    }
}
