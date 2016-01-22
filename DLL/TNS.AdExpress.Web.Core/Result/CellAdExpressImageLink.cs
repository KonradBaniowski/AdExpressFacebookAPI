#region Informations
// Auteur: G. Facon
// Date de cr�ation: 21/08/2007
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// Cellule contenant une image avec un lien pour AdExpress
    /// </summary>
    public abstract class CellAdExpressImageLink:CellImageLink {
        

        #region Variables
        /// <summary>
        /// Adresse d'appelle � la cr�ation
        /// </summary>
        protected string _link = null;

        /// <summary>
        /// Chemin de l'image servant au lien
        /// </summary>
        protected string _imagePath = null;

        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _webSession;
        
        /// <summary>
        /// Liste des chemins vers les cr�ations
        /// </summary>
        protected string _creatives;
       
        #endregion

        /// <summary>
        /// Obtient le chemin de l'image contenant le lien
        /// </summary>
        /// <returns>Chemin de l'image contenant le lien</returns>
        public override string GetImagePath() {
            if (_imagePath == null) throw new ArgumentNullException("Parameter _imagePath is null"); 
            return (_imagePath);
        }

        
    }
}
