using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellAdExpressInsertionsLink:CellAdExpressDetailLink {

        #region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        public CellAdExpressInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
            : base(cellLevel, webSession, genericDetailLevel,"",-1) {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de détail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellAdExpressInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
        }
        #endregion
    }
}
