//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de création: 06/12/2004 
//// Date de modification: 06/12/2004 
//// 22/06/2005 par B.Masson - Sortie Excel des données brutes
//#endregion

//using System;
//using System.Web.UI;

//using TNS.AdExpress.Web.Core.Sessions;
//using WebExceptions = TNS.AdExpress.Web.Exceptions;
//using WebRules = TNS.AdExpress.Web.Rules;
//using WebUI = TNS.AdExpress.Web.UI;
//using WebFunctions = TNS.AdExpress.Web.Functions;
//using TNS.AdExpress.Web.UI.Results;
//using TNS.AdExpress.Domain.Translation;
//using TNS.AdExpress.Domain.Web.Navigation;
//using ClassificationCst = TNS.AdExpress.Constantes.Classification;
//using TNS.FrameWork.WebResultUI;


//namespace TNS.AdExpress.Web.BusinessFacade.Results {
//    /// <summary>
//    /// Construction d'un résultat pour Alerte Portefeuille ou Portefeuille d'un support.
//    /// </summary>
//    public class PortofolioSystem {

//        #region HTML

//        #region Alerte portefeuille d'un support
//        /// <summary>
//        /// Accès à la construction du tableau de l'alerte portefeuille
//        /// </summary>
//        /// <param name="page">Page</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static string GetAlertHtml(Page page, WebSession webSession) {


//            #region Module sélectionné
//            Module currentModuleDescription;
//            try {
//                currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible d'obtenir le module sélectionné", err));
//            }
//            #endregion

//            #region Paramétrage des dates
//            int dateBegin;
//            int dateEnd;
//            try {
//                dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
//                dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de formater les dates", err));
//            }
//            #endregion

//            try {

//                switch (webSession.CurrentTab) {

//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.Synthesis(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);

//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.GetHTMLNoveltyUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);

//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.DetailMediaUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
//                        if (webSession.Graphics) {
//                            ((TNS.AdExpress.Web.UI.Results.PortofolioChartUI)page.FindControl("portofolioChart")).StructureChart(webSession, true);
//                            if (!((TNS.AdExpress.Web.UI.Results.PortofolioChartUI)page.FindControl("portofolioChart")).Visible) {
//                                return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
//                                    + "</div>");
//                            }
//                            else {
//                                return string.Empty;
//                            }

//                        }
//                        else {
//                            return WebUI.Results.PortofolioUI.GetHTMLStructureUI(page, webSession, false);
//                        }
//                    default:
//                        return "";

//                }

//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de calculer le résultat HTML d'un alerte de portefeuille", err));
//            }
//        }
//        #endregion

//        #region Portefeuille d'un support
//        /// <summary>
//        /// Accès à la construction du tableau Portefeuille d'un support.
//        /// </summary>
//        /// <param name="page">Page</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static string GetHtml(Page page, WebSession webSession) {


//            #region Module sélectionné
//            Module currentModuleDescription;
//            try {
//                currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible d'obtenir le module sélectionné", err));
//            }
//            #endregion

//            #region Paramétrage des dates
//            int dateBegin;
//            int dateEnd;
//            try {
//                dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
//                dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de formater les dates", err));
//            }
//            #endregion

//            try {

//                switch (webSession.CurrentTab) {
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.SynthesisAnalysis(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.GetHTMLNoveltyUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
//                        return TNS.AdExpress.Web.UI.Results.PortofolioUI.DetailMediaUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PERFORMANCES:
//                        //id Média
//                        string idVehicle = webSession.GetSelection(webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
//                        //Résultat
//                        if ((ClassificationCst.DB.Vehicles.names)int.Parse(idVehicle.ToString()) != ClassificationCst.DB.Vehicles.names.press
//                            && (ClassificationCst.DB.Vehicles.names)int.Parse(idVehicle.ToString()) != ClassificationCst.DB.Vehicles.names.internationalPress)
//                            return TNS.AdExpress.Web.UI.Results.PortofolioUI.Synthesis(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), false);
//                        else return WebUI.Results.PortofolioUI.GetHTMLPerformancesUI(page, webSession, false);
//                    default:
//                        return "";
//                }

//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de calculer le résultat HTML d'un alerte de portefeuille", err));
//            }
//        }
//        /// <summary>
//        /// Accès à la construction des resulttable Portefeuille d'un support.
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Résult Table</returns>
//        public static ResultTable GetResultTable(WebSession webSession) {

//            try {

//                switch (webSession.CurrentTab) {

//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
//                        return WebRules.Results.PortofolioRules.GetResultTable(webSession);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
//                        return WebRules.Results.PortofolioRules.GetCalendar(webSession);
//                    default:
//                        return null;
//                }

//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de calculer le résultat d'une analyse de portefeuille", err));
//            }
//        }
//        #endregion


//        #endregion

//        #region Sortie Excel
//        /// <summary>
//        /// Accès à la construction du tableau de l'alerte portefeuille
//        /// </summary>
//        /// <param name="page">Page</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML du tableau de l'analyse dynamique</returns>
//        public static string GetExcel(Page page, WebSession webSession) {


//            #region Module sélectionné
//            Module currentModuleDescription;
//            try {
//                currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible d'obtenir le module sélectionné", err));
//            }
//            #endregion

//            #region Paramétrage des dates
//            int dateBegin;
//            int dateEnd;
//            try {
//                dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
//                dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de formater les dates", err));
//            }
//            #endregion

//            try {
//                switch (webSession.CurrentTab) {

//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
//                        return PortofolioUI.SynthesisExcel(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString(), currentModuleDescription.ModuleType);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
//                        return PortofolioUI.GetExcelNoveltyUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString());
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
//                        return PortofolioUI.GetExcelDetailMediaUI(webSession, ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID, ((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID, dateBegin.ToString(), dateEnd.ToString());
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
//                        return PortofolioUI.GetExcelStructureUI(page, webSession, true);
//                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PERFORMANCES:
//                        return PortofolioUI.GetExcelPerformancesUI(page, webSession, true);
//                    default:
//                        return "";

//                }

//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.PortofolioSystemException("Impossible de calculer le résultat HTML d'un alerte de portefeuille", err));
//            }
//        }
//        #endregion
//    }
//}
