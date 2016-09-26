using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Result {
    public class CellAdExpressCreativesLink:CellAdExpressDetailLink {

        #region Variables
        /// <summary>
        /// Module a utiliser
        /// </summary>
        protected Int64 _moduleId=-1;
        #endregion

        #region Constructeurs
        /// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        protected CellAdExpressCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
            _link = "javascript:OpenCreatives('{0}','{1}','{2}','{3}','{4}');";
		}

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de détail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        /// <param name="moduleId">Module à utiliser</param>
        protected CellAdExpressCreativesLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId,Int64 moduleId)
            : this(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
            _moduleId=moduleId;      
        }
		#endregion

        #region Implémentation de CellImageLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if(ShowLink()) {
                if(_moduleId<0)_moduleId=_webSession.CurrentModule;
                object[] args={_webSession.IdSession,_linkRules.GetHierarchy(),_zoomDate,_universId,_moduleId};
                //return (string.Format(_link,args));

                return string.Format("ids={0}&zoomDate={1}&idUnivers={2}&moduleId={3}", _linkRules.GetHierarchy(), _zoomDate, _universId, _moduleId);
            }
            return ("");
        }
        #endregion

    }
}
