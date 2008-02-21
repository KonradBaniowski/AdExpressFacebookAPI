#region Informations
// Auteur: G. Facon
// Date de cr�ation: 21/08/2007
// Date de modification:
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Core.Result{


	/// <summary>
	/// Description r�sum�e de CellCreative.
	/// </summary>
    public class CellInsertionsLink:CellAdExpressInsertionsLink {

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de d�tail</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
		public CellInsertionsLink( CellLevel cellLevel, WebSession webSession,GenericDetailLevel genericDetailLevel)
            :base(cellLevel,webSession,genericDetailLevel){
            _linkRules=new DefaultShowLinkRules(cellLevel,genericDetailLevel);
            _link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de d�tail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de d�tail g�n�rique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
        public CellInsertionsLink(CellLevel cellLevel,WebSession webSession,GenericDetailLevel genericDetailLevel,string zoomDate,int universId)
            : base(cellLevel,webSession,genericDetailLevel,zoomDate,universId) {
            _linkRules=new DefaultShowLinkRules(cellLevel,genericDetailLevel);
            _link="javascript:OpenCreationCompetitorAlert('{0}','{1},{2}','');";
        }
		#endregion

        #region Impl�mentation de CellImageLink
        /// <summary>
        /// Obtient l'adresse du lien
        /// </summary>
        /// <returns>Adresse du lien</returns>
        public override string GetLink() {
            if(_linkRules.ShowLink()) {
                return (string.Format(_link,_webSession.IdSession,_cellLevel.Id,_cellLevel.Level));
            }
            return ("");
        }
        #endregion

    }
}
