//#region Informations
//// Auteur:
//// Date de cr�ation: 
//// Date de modification: 
////		19/11/2004
////		12/08/2005	G. Facon	Gestion des exceptions 
//#endregion

//using System;
//using System.Web.UI;
//using TNS.AdExpress.Web.Core.Sessions;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;

//namespace TNS.AdExpress.Web.UI.Results{

//    /// <summary>
//    /// UI alerte de potentiels
//    /// </summary>
//    public class MarketShareUI{
 
//        /// <summary>
//        /// G�n�re le code Html pour l'alerte de potentiels
//        /// </summary>
//        /// <returns>Code Html</returns>
//        public static string GetHtml(Page page,object[,] tab,WebSession webSession){
//            try{
//                return (TNS.AdExpress.Web.UI.Results.CompetitorUI.GetHtml(page,tab,webSession));
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareUIException("Impossible de G�n�rer le code HTML",err));
//            }
//        }

//        /// <summary>
//        /// G�n�re le code Excel pour l'alerte de potentiels
//        /// </summary>
//        /// <returns>Code Excel</returns>
//        public static string GetExcel(Page page,object[,] tab,WebSession webSession){
//            try{
//                return (TNS.AdExpress.Web.UI.Results.CompetitorUI.GetExcel(page,tab,webSession));
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareUIException("Impossible de G�n�rer le code Excel",err));
//            }
//        }

//        /// <summary>
//        /// G�n�re le code Excel des donn�es brutes pour l'alerte de potentiels
//        /// </summary>
//        /// <returns>Code Raw Excel</returns>
//        public static string GetRawExcel(Page page,object[,] tab,WebSession webSession){
//            try{
//                return (TNS.AdExpress.Web.UI.Results.CompetitorUI.GetRawExcel(page,tab,webSession));
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MarketShareUIException("Impossible de G�n�rer le code Raw Excel",err));
//            }
//        }
//    }
//}
