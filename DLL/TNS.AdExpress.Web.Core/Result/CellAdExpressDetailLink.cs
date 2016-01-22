#region Informations
// Auteur: G. Facon
// Date de création: 21/08/2007
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// Cellule AdExpress pour les liens vers le détail des créations ou inertions
    /// </summary>
    public class CellAdExpressDetailLink : CellAdExpressImageLink {

        #region Variables
        protected ILinkRules _linkRules=null;
        /// <summary>
        /// Niveau de détail générique
        /// </summary>
        protected GenericDetailLevel _genericDetailLevel;
        /// <summary>
        /// Cellule du niveau qui a été cliqué
        /// </summary>
        protected CellLevel _cellLevel;
        /// <summary>
        /// Identifiant de l'univers
        /// </summary>
        protected int _universId=-1;
        /// <summary>
        /// Date du zoom
        /// </summary>
        protected string _zoomDate="";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        protected CellAdExpressDetailLink(){}

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de détail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        protected CellAdExpressDetailLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId) {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            if (cellLevel == null) throw (new ArgumentNullException("L'objet cellLevel est null"));
            if (genericDetailLevel == null) throw (new ArgumentNullException("L'objet genericDetailLevel est null"));
            if(zoomDate!=null) _zoomDate=zoomDate;
            _universId=universId;
            _webSession = webSession;
            _imagePath = "/App_Themes/"+WebApplicationParameters.Themes[_webSession.SiteLanguage].Name+"/Images/Common/picto_plus.gif";
            _genericDetailLevel = genericDetailLevel;
            _cellLevel = cellLevel;
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Indique s'il faut afficher le lien vers les créations
        /// </summary>
        /// <returns>True s'il faut afficher le lien vers les créations</returns>
        protected bool ShowLink() {
            if(_linkRules==null) throw (new ArgumentNullException("Rules are not defined"));
            return (_linkRules.ShowLink());
        }
        #endregion

        #region Implémentation de CellImageLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            throw (new NotImplementedException());
        }
        #endregion

    }
}
