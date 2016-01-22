//#region Informations
//// Auteur: G. Facon 
//// Date de création: 23/11/2004 
//// Date de modification: 23/11/2004 
////	G. Facon,	11/08/2005,	New Exception Management
//#endregion

//using System;
//using System.Web.UI;

//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using WebRules=TNS.AdExpress.Web.Rules;
//using WebUI=TNS.AdExpress.Web.UI;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using TNS.FrameWork.WebResultUI;

//namespace TNS.AdExpress.Web.BusinessFacade.Results{
//    /// <summary>
//    /// Construction d'un résultat analyse ou alerte concurrentielle
//    /// </summary>
//    public class CompetitorSystem{

//        #region HTML
//        /// <summary>
//        /// Accès à la construction du tableau de l'analyse dynamique
//        /// </summary>	
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static ResultTable GetHtml(WebSession webSession){
			
			
//            try{
//                switch(webSession.CurrentTab){
//                    case FrameWorkResultConstantes.CompetitorMarketShare.SYNTHESIS :

////						tab=WebRules.Results.CompetitorRules.GetSynthesisData(webSession);
////						if(tab==null){
////							return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
////								+"</div>");
////						}
////						return(WebUI.Results.CompetitorUI.GetSynthesisHtml(page,tab,webSession,false));						
//                        return(WebRules.Results.CompetitorRules.GetSynthesisData(webSession));
//                    case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.FORCES:
//                        return (WebRules.Results.MarketShareRules.GetStrenghsOrOpportunitiesFormattedTable(webSession, true));
//                    case TNS.AdExpress.Constantes.FrameWork.Results.CompetitorMarketShare.POTENTIELS:
//                        return (WebRules.Results.MarketShareRules.GetStrenghsOrOpportunitiesFormattedTable(webSession, false));
//                    default:
//                        return(WebRules.Results.CompetitorRules.GetData(webSession));
////						tab=WebRules.Results.CompetitorRules.GetData(webSession);
////						if(tab==null){
////							return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
////								+"</div>");
////						}
////						return(WebUI.Results.CompetitorUI.GetHtml(page,tab,webSession));
//                }
				
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorSystemException("Impossible de calculer le résultat HTML d'un module concurrentiel",err));
//            }
//        }
//        #endregion

//        #region Excel
//        /// <summary>
//        /// Accès à la construction du tableau de l'analyse dynamique
//        /// </summary>
//        /// <param name="page">Page</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static ResultTable GetExcel(Page page,WebSession webSession){
			
//            try{
//                switch(webSession.CurrentTab){
//                    case FrameWorkResultConstantes.CompetitorAlert.SYNTHESIS :
////						tab=WebRules.Results.CompetitorRules.GetSynthesisData(webSession);
////						if(tab==null){
////							return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
////								+"</div>");
////						}
////						return(WebUI.Results.CompetitorUI.GetSynthesisExcel(page,tab,webSession));						
//                    default:
//                        return(WebRules.Results.CompetitorRules.GetData(webSession));
////						tab=WebRules.Results.CompetitorRules.GetData(webSession);
////						if(tab==null){
////							return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
////								+"</div>");
////						}
////						return(WebUI.Results.CompetitorUI.GetExcel(page,tab,webSession));
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorSystemException("Impossible de calculer le résultat Excel d'un module concurrentiel",err));
//            }
//        }
//        #endregion

//        #region Excel des données brutes
//        /// <summary>
//        /// Accès à la construction du tableau de l'analyse dynamique
//        /// </summary>
//        /// <param name="page">Page</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static string GetRawExcel(Page page,WebSession webSession){
////			object[,]tab=null;
////			try{
////				tab=WebRules.Results.CompetitorRules.GetData(webSession);
////				if(tab==null){
////					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
////				}
////				return(WebUI.Results.CompetitorUI.GetRawExcel(page,tab,webSession));
////			}
////			catch(System.Exception err){
////				throw(new WebExceptions.CompetitorSystemException("Impossible de calculer le résultat Excel d'un module concurrentiel",err));
////			}
//            throw(new System.NotImplementedException());
//        }
//        #endregion

//    }
//}
