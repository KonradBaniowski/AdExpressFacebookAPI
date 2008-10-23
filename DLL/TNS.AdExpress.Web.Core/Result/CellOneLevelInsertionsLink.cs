using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellOneLevelInsertionsLink:CellAdExpressInsertionsLink {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        public CellOneLevelInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
            : base(cellLevel,webSession,genericDetailLevel) {
            _linkRules=new OneLevelShowLinkRules(cellLevel,genericDetailLevel);
            //_link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
            _link = "javascript:OpenInsertion('{0}','{1},{2}','','-1','{3}');";
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellOneLevelInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel) {
            _linkRules=new OneLevelShowLinkRules(cellLevel,genericDetailLevel);
            //_link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
            _link = "javascript:OpenInsertion('{0}','{1},{2}','','-1','{3}');";
        }
        #endregion

        #region Implémentation de CellImageLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if(_linkRules.ShowLink()) {
                return (string.Format(_link,_webSession.IdSession,_cellLevel.Id,_cellLevel.Level, _webSession.CurrentModule));
            }
            return ("");
        }
        #endregion
    }
}
