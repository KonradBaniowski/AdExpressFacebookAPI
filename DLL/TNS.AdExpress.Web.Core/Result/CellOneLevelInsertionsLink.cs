using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellOneLevelInsertionsLink:CellOneLevelCreativesLink {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        //public CellOneLevelInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel)
        //    : base(cellLevel,webSession,genericDetailLevel) {
        //    //_link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
        //    //_link = "javascript:OpenInsertion('{0}','{1},{2}','','-1','{3}');";
        //        _link = "javascript:OpenInsertion('{0}','{1}','{2}','{3}','{4}');";

        //}
        public CellOneLevelInsertionsLink(CellLevel cellLevel, WebSession webSession, GenericDetailLevel genericDetailLevel)
    : base(cellLevel, webSession, genericDetailLevel)
        {
            _link = "{0},{1},{2},{3},{4}";
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule du niveau qui a été cliqué</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        //public CellOneLevelInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
        //    : base(cellLevel,webSession,genericDetailLevel) {
        //    //_link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
        //    //_link = "javascript:OpenInsertion('{0}','{1},{2}','','-1','{3}');";
        //        _link = "javascript:OpenInsertion('{0}','{1}','{2}','{3}','{4}');";

        //}
        public CellOneLevelInsertionsLink(CellLevel cellLevel, WebSession webSession, GenericDetailLevel genericDetailLevel, string zoomDate, int universId)
    : base(cellLevel, webSession, genericDetailLevel)
        {
            _link = "{0},{1},{2},{3},{4}";
        }
        #endregion

        #region Implémentation de CellImageLink
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
