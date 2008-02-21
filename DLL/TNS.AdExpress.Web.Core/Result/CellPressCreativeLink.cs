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
    /// cellule contenant le lien vers les créations de la Presse
    /// </summary>
    public class CellPressCreativeLink : CellAdExpressImageLink {

        #region Variables
        /// <summary>
        /// Liste des chemins vers les créations
        /// </summary>
        protected string _creatives;

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        public CellPressCreativeLink(string creatives) {
            _imagePath = "/Images/Common/picto_plus.gif";
            _link = "javascript:openPressCreation('{0}');";
            _creatives = creatives;
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
