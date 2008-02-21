#region Information
// Auteur G. Facon
// Date de cr�ation : 15/11/04
// Date de modification : 
//	D. Mussuma	30/12/2004  Int�gration de WebPage
//	B. Masson	22/06/2005  Excel des donn�es brutes
//	G. Facon	11/08/2005	New Exception Management
#endregion

using System;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebRules=TNS.AdExpress.Web.Rules;
using WebUI=TNS.AdExpress.Web.UI;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Acc�s au r�sultat de l'analyse dynamique
	/// </summary>
	public class DynamicSystem{

		#region HTML
		/// <summary>
		/// Acc�s � la construction du tableau de l'analyse dynamique
		/// </summary>		
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static ResultTable GetResultTable(WebSession webSession){
			
			try{
				//Construction du tableau de donn�es en fonction du r�sultat demand�
				switch(webSession.CurrentTab){
					case FrameWorkResultConstantes.DynamicAnalysis.PORTEFEUILLE:
						return(WebRules.Results.DynamicRules.GetPortofolio(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL:
						return(WebRules.Results.DynamicRules.GetLoyal(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_DECLINE:
						return(WebRules.Results.DynamicRules.GetLoyalDecline(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_RISE:
						return(WebRules.Results.DynamicRules.GetLoyalRise(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.LOST:
						return(WebRules.Results.DynamicRules.GetLost(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.WON:
						return(WebRules.Results.DynamicRules.GetWon(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					case FrameWorkResultConstantes.DynamicAnalysis.SYNTHESIS:
						return(WebRules.Results.DynamicRules.GetSynthesis(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
					default:
						throw(new WebExceptions.DynamicSystemException("L'onglet n'existe pas"));
					
				}
//				// G�n�ration du code HTML
//				if(tab==null){
//					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//						+"</div>");
//				}
//				return(WebUI.Results.DynamicUI.GetHtml(page,tab,webSession));
			}
			catch(System.Exception err){
				throw(new WebExceptions.DynamicSystemException("Impossible d'obtenir le r�sultat",err));
			}
		}
		#endregion

		#region Excel
		/// <summary>
		/// Acc�s � la construction du tableau de l'analyse dynamique
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="page">page</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static string GetExcel(Page page,WebSession webSession){
//			object[,] tab=null;
//			try{
//				//Construction du tableau de donn�es en fonction du r�sultat demand�
//				switch(webSession.CurrentTab){
//					case FrameWorkResultConstantes.DynamicAnalysis.PORTEFEUILLE:
//						tab=WebRules.Results.DynamicRules.GetPortofolio(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL:
//						tab=WebRules.Results.DynamicRules.GetLoyal(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_DECLINE:
//						tab=WebRules.Results.DynamicRules.GetLoyalDecline(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_RISE:
//						tab=WebRules.Results.DynamicRules.GetLoyalRise(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.LOST:
//						tab=WebRules.Results.DynamicRules.GetLost(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.WON:
//						tab=WebRules.Results.DynamicRules.GetWon(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						break;
//					case FrameWorkResultConstantes.DynamicAnalysis.SYNTHESIS:
//						tab=WebRules.Results.DynamicRules.GetSynthesis(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate);
//						return(WebUI.Results.DynamicUI.GetSynthesisExcel(page,tab,webSession));						
//					default:
//						throw(new WebExceptions.DynamicSystemException("L'onglet n'existe pas"));
//					
//				}
//				// G�n�ration du code HTML
//				if(tab==null){
//					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//						+"</div>");
//				}
//				return("<table><tr><td bgcolor=\"#ffffff\">"+WebUI.Results.DynamicUI.GetExcel(tab,webSession)+"</table></td></tr></table>");
//			}
//			catch(System.Exception err){
//				throw(new WebExceptions.DynamicSystemException("Impossible d'obtenir le r�sultat",err));
//			}
			return(null);
		}
		#endregion

		#region Excel des donn�es brutes
		/// <summary>
		/// Acc�s � la construction du tableau de l'analyse dynamique
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau de l'analyse dynamique</returns>
		public static string GetRawExcel(WebSession webSession){
//			object[,] tab=GetData(webSession);
//			try{
//				// G�n�ration du code HTML
//				if(tab==null){
//					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//						+"</div>");
//				}
//				return("<table><tr><td bgcolor=\"#ffffff\">"+WebUI.Results.DynamicUI.GetRawExcel(tab,webSession)+"</table></td></tr></table>");
//			}
//			catch(System.Exception err){
//				throw(new WebExceptions.DynamicSystemException("Impossible d'obtenir le r�sultat",err));
//			}
			return(null);
		}
		#endregion

		#region Methodes internes
		/// <summary>
		/// Obtient les donn�es � traiter en fonction de l'onglet
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>R�sultat</returns>
		private static object[,] GetData(WebSession webSession){
//			try{
//				object[,] tab=null;
//				//Construction du tableau de donn�es en fonction du r�sultat demand�
//				switch(webSession.CurrentTab){
//					case FrameWorkResultConstantes.DynamicAnalysis.PORTEFEUILLE:
//						return(tab=WebRules.Results.DynamicRules.GetPortofolio(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL:
//						return(tab=WebRules.Results.DynamicRules.GetLoyal(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_DECLINE:
//						return(tab=WebRules.Results.DynamicRules.GetLoyalDecline(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_RISE:
//						return(tab=WebRules.Results.DynamicRules.GetLoyalRise(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					case FrameWorkResultConstantes.DynamicAnalysis.LOST:
//						return(tab=WebRules.Results.DynamicRules.GetLost(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					case FrameWorkResultConstantes.DynamicAnalysis.WON:
//						return(WebRules.Results.DynamicRules.GetWon(webSession,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
//					default:
//						throw(new WebExceptions.DynamicSystemException("L'onglet n'existe pas"));
//
//				}
//			}
//			catch(System.Exception err){
//				throw(new WebExceptions.DynamicSystemException("Impossible d'obtenir les donn�es pour calculer le r�sultat: "+err.Message));
//			}
			return(null);				
		}
		#endregion
	}
}
