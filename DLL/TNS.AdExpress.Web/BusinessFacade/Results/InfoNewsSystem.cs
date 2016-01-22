#region Informations
// Auteur: B. Masson
// Date de création: 18/11/2004 
// Date de modification: 19/11/2004
//	G. Facon	11/08/2005	New Exception Management 
#endregion

using System;
using System.Web.UI;
using System.Collections;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebRules = TNS.AdExpress.Web.Rules;
using WebCommon = TNS.AdExpress.Web.Common;
using WebUI = TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Accès aux données d'Info/News
	/// </summary>
	public class InfoNewsSystem{
		
		#region Variables
		/// <summary>
		/// Liste de tous les éléments
		/// </summary>
		protected ArrayList _list=new ArrayList();
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="page">Page en cours</param>
		/// <param name="webSession">Session</param>
		public InfoNewsSystem(Page page, WebSession webSession){
			try{
				_list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXNEWS,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXNEWS,webSession)));
			}
			catch(System.Exception){
			}

			try{
				_list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXREPORT,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXREPORT,webSession)));
			}
			catch(System.Exception){
			}

			try{
				_list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXREVIEW,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXREVIEW,webSession)));
			}
			catch(System.Exception){
			}

			try{
				_list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.NOUVEAUTES,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.NOUVEAUTES,webSession)));
			}
			catch(System.Exception){
			}
		}
		#endregion

		#region Sortie HTML
		/// <summary>
		/// Accès à la construction du tableau déroulant d'Infos/News
		/// </summary>
		/// <param name="page">Page en cours</param>
		/// <param name="webSession">Session</param>
		/// <returns>Code HTML du tableau déroulant d'Infos/News</returns>
        public static string GetHtml(Page page, WebSession webSession) {
			ArrayList list=new ArrayList();

			#region Construction de la liste
			try{
				list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXNEWS,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXNEWS,webSession)));
			}
			catch(System.Exception){
			}

			try{
				list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXREPORT,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXREPORT,webSession)));
			}
			catch(System.Exception){
			}

			try{
				list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.ADEXREVIEW,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.ADEXREVIEW,webSession)));
			}
			catch(System.Exception){
			}

			try{
				list.Add(new WebCommon.Results.InfoNewsItem(WebConstantes.ModuleInfosNews.NOUVEAUTES,WebRules.Results.InfoNewsRules.GetData(WebConstantes.ModuleInfosNews.NOUVEAUTES,webSession)));
			}
			catch(System.Exception){
			}
			#endregion

            return (WebUI.InfoNewsUI.GetHtml(page, list));
		}
		#endregion
		
	}
}
