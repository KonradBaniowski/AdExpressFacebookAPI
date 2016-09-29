#region Informations
// Auteur: G. Facon
// Date de cr�ation: 21/08/2007
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    /// <summary>
    /// Cellule image ayant un lien vers le d�tail des insertions du parrainage
    /// </summary>
    public class CellSponsorshipInsertionsLink:CellAdExpressInsertionsLink {

        #region Variables
        /// <summary>
        /// Module a utiliser
        /// </summary>
        protected Int64 _moduleId = -1;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de d�tail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
        public CellSponsorshipInsertionsLink(CellLevel cellLevel, WebSession webSession, GenericDetailLevel genericDetailLevel)
            : base(cellLevel, webSession, genericDetailLevel) {
            _linkRules=new SponsorshipShowLinkRules(cellLevel,webSession,genericDetailLevel);
            _link = "javascript:OpenInsertion('{0}','{1},1','','-1','{2}');";
        }
        #endregion

        //#region Impl�mentation de CellImageLink
        ///// <summary>
        ///// Obtient l'adresse du lien
        ///// </summary>
        ///// <returns>Adresse du lien</returns>
        //public override string GetLink() {
        //    if(_linkRules.ShowLink()) {
        //        return (string.Format(_link, _webSession.IdSession, _linkRules.GetHierarchy(), _webSession.CurrentModule));            
        //    }
        //    return ("");
        //}
        //#endregion

        #region Impl�mentation de GetLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink()
        {
            if (ShowLink())
            {
                _link = "ids={0}&zoomDate={1}&idUnivers={2}&moduleId={3}";
                if (_moduleId < 0) _moduleId = _webSession.CurrentModule;
                object[] args = { _linkRules.GetHierarchy(), _zoomDate, _universId, _moduleId };
                return (string.Format(_link, args));
            }
            return ("");
        }
        #endregion

    }
}
