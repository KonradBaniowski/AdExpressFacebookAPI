using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellOneLevelCreativesLink:TNS.AdExpress.Web.Core.Result.CellAdExpressCreativesLink {

        #region Constructeurs
        /// <summary>
		/// Constructeur
		/// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a �t� cliqu�</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellOneLevelCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
            _linkRules=new OneLevelShowLinkRules(cellLevel,genericDetailLevel);
		}

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a �t� cliqu�</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellOneLevelCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
            : base(cellLevel,webSession,genericDetailLevel,"",-1) {
            _linkRules=new OneLevelShowLinkRules(cellLevel,genericDetailLevel);
        }
        #endregion

        #region Impl�mentation de CellImageLink
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