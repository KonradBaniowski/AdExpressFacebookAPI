#region Informations
// Auteur: G. Facon 
// Date de création: 23/11/2004 
// Date de modification: 21/06/2005
//	Excel des données brutes
//	11/08/2005	G. Facon		New Exception Management
//	01/12/2006	G. Facon		Résultats Génériques
#endregion

#region Using
using System;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebRules=TNS.AdExpress.Web.Rules;
using WebUI=TNS.AdExpress.Web.UI;
using TNS.FrameWork.WebResultUI;
#endregion

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Construction d'un résultat analyse ou alerte de potentiels
	/// </summary>
	public class MarketShareSystem{

		#region HTML
		/// <summary>
		/// Accès à la construction du tableau de l'analyse dynamique
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static ResultTable GetResultTable(WebSession webSession){
			try{
				switch(webSession.CurrentTab){
					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.PORTEFEUILLE:
						return(WebRules.Results.MarketShareRules.GetPortofolioFormattedTable(webSession));
					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.FORCES:
						return(WebRules.Results.MarketShareRules.GetStrenghsOrOpportunitiesFormattedTable(webSession,true));
					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.POTENTIELS:
						return(WebRules.Results.MarketShareRules.GetStrenghsOrOpportunitiesFormattedTable(webSession,false));
				}
				return(null);
			}
			catch(System.Exception err){
				throw(new WebExceptions.MarketShareSystemException("Impossible de construire le tableau de résultat d'un module potentiel",err));
			}
		}
		#endregion

		#region Excel
		/// <summary>
		/// Accès à la construction du tableau de l'analyse dynamique
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static string GetExcel(Page page,WebSession webSession){
			object[,]tab=null;
			try{
//				switch(webSession.CurrentTab){
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.PORTEFEUILLE:
//						tab=WebRules.Results.MarketShareRules.GetPortofolioFormattedTable(webSession);
//						break;
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.FORCES:
//						tab=WebRules.Results.MarketShareRules.GetStrenghsFormattedTable(webSession);
//						break;
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.POTENTIELS:
//						tab=WebRules.Results.MarketShareRules.GetOpportunitiesFormattedTable(webSession);
//						break;								
//				}
				if(tab==null){
					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
						+"</div>");
				}
				return(WebUI.Results.MarketShareUI.GetExcel(page,tab,webSession));		
			}
			catch(System.Exception err){
				throw(new WebExceptions.MarketShareSystemException("Impossible de construire le tableau de résultat d'un module potentiel",err));
			}
		}
		#endregion

		#region Excel des données brutes
		/// <summary>
		/// Accès à la construction du tableau de l'analyse dynamique
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static string GetRawExcel(Page page,WebSession webSession){
			object[,]tab=null;
			try{
                //switch(webSession.CurrentTab){
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.PORTEFEUILLE:
//						tab=WebRules.Results.MarketShareRules.GetPortofolioFormattedTable(webSession);
//						break;
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.FORCES:
//						tab=WebRules.Results.MarketShareRules.GetStrenghsFormattedTable(webSession);
//						break;
//					case TNS.AdExpress.Constantes.FrameWork.Results.MarketShare.POTENTIELS:
//						tab=WebRules.Results.MarketShareRules.GetOpportunitiesFormattedTable(webSession);
//						break;								
                //}
				if(tab==null){
					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
						+"</div>");
				}
				return(WebUI.Results.MarketShareUI.GetRawExcel(page,tab,webSession));		
			}
			catch(System.Exception err){
				throw(new WebExceptions.MarketShareSystemException("Impossible de construire le tableau de résultat d'un module potentiel",err));
			}
		}
		#endregion
	}
}
