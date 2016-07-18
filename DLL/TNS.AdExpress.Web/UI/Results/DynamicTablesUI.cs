//#region Information
//// Auteur: G. Ragneau
//// Date de création: 24/09/2004 
//// Date de modification: 
////		08/11/2004	A.obermeyer 
////		16/02/2005  A.obermeyer		bug couleur ds tableau 3 4
////		18/02/2005					rajout Marque en personnalisation
////		10/05/2005	K.Shehzad		Chagement d'en tête Excel
////		12/08/2005	G. Facon		Nom de fonction et gestion des exceptions
////		24/10/2005	B. Masson		Mise en place KEuros
//#endregion

//using System;
//using System.Text;

//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using TNS.AdExpress.Web.Rules.Results;
//using WebExceptions = TNS.AdExpress.Web.Exceptions;
//using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
//using WebFunctions = TNS.AdExpress.Web.Functions;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
//using WebCst = TNS.AdExpress.Constantes.Web;
//using ClassifCst = TNS.AdExpress.Constantes.Classification;
//using DBClassif = TNS.AdExpress.Constantes.Classification.DB;
//using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;
//using TNS.AdExpress.Domain.Exceptions;

//namespace TNS.AdExpress.Web.UI.Results {
//    /// <summary>
//    /// Classe UI du module tableau dynamique (analyse sectorielle)
//    /// </summary>
//    public class DynamicTablesUI {

//        #region UI

//        #region Aiguillage en fonction du tableau dynamique
//        /// <summary>
//        /// Lance l'affichage correspondant au tableau demandé
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="excel">Format Excel</param>
//        /// <returns>UI du tableau</returns>
//        public static string GetDynamicTableUI(WebSession webSession, bool excel) {

//            object[,] data;

//            try {
//                data = DynamicTablesRules.GetDataTable(webSession);
//            }
//            catch (DeliveryFrequencyException) { return UnvalidFrequencyDelivery(webSession); }
//            catch (System.Exception err) {
//                throw (new WebExceptions.DynamicTablesUIException("Impossible d'obtenir les données", err));
//            }
//            if (data == null || data.Length == 0) {
//                return NoData(webSession);
//            }

//            StringBuilder html = new StringBuilder(6000000);

//            try {
//                #region Legende
//                //si l'univers d'element reference n'est pas vide ou
//                //		(si l'univers competiteur n'est pas null
//                //			retourner le resultat de (nbElt>0)
//                //		sinon retourner faux)
//                //si le resultat de l'expression n'est pas vide, on affiche la legende
//                string tempString = "";
//                bool withAdvertisers = false;
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) withAdvertisers = true;
//                }
//                else if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1) && webSession.SecondaryProductUniverses[1].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) withAdvertisers = true;
//                }
//                //if(webSession.ReferenceUniversAdvertiser.Nodes.Count > 0
//                //    || (webSession.CompetitorUniversAdvertiser[0]!=null)
//                //    ?((TreeNode)webSession.CompetitorUniversAdvertiser[0]).Nodes.Count>0
//                //    :false){
//                if (withAdvertisers) {
//                    if (!excel) {
//                        html.Append("<table cellPadding=0 cellSpacing=10px border=0 class=\"txtNoir11Bold\"><tr>");
//                        html.Append("<td class=Green>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//                        html.Append("<td>" + GestionWeb.GetWebWord(1230, webSession.SiteLanguage) + "</td>");
//                        html.Append("<td class=Red>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//                        html.Append("<td>" + GestionWeb.GetWebWord(1231, webSession.SiteLanguage) + "</td>");
//                        html.Append("<td>" + GestionWeb.GetWebWord(1232, webSession.SiteLanguage) + "</td>");
//                        html.Append("</tr></table>");
//                    }
//                }
//                #endregion

//                switch (webSession.PreformatedTable) {
//                    case TblFormatCst.PreformatedTables.media_X_Year:
//                    case TblFormatCst.PreformatedTables.product_X_Year:
//                        GetDynamicTableUI_1_2(webSession, html, data, excel);
//                        break;
//                    case TblFormatCst.PreformatedTables.productMedia_X_Year:
//                    case TblFormatCst.PreformatedTables.mediaProduct_X_Year:
//                        GetDynamicTableUI_3_4_10_11(webSession, html, data, excel, false);
//                        break;
//                    case TblFormatCst.PreformatedTables.productMedia_X_YearMensual:
//                    case TblFormatCst.PreformatedTables.mediaProduct_X_YearMensual:
//                        GetDynamicTableUI_3_4_10_11(webSession, html, data, excel, true);
//                        break;
//                    case TblFormatCst.PreformatedTables.productYear_X_Media:
//                        GetDynamicTableUI_5(webSession, html, data, excel);
//                        break;
//                    case TblFormatCst.PreformatedTables.mediaYear_X_Mensual:
//                    case TblFormatCst.PreformatedTables.mediaYear_X_Cumul:
//                    case TblFormatCst.PreformatedTables.productYear_X_Cumul:
//                    case TblFormatCst.PreformatedTables.productYear_X_Mensual:
//                        GetDynamicTableUI_6_7_8_9(webSession, html, data, excel);
//                        break;
//                    default:
//                        throw (new WebExceptions.DynamicTablesUIException("Le cas du tableau " + webSession.PreformatedTable.ToString() + " n'est pas un cas traité."));
//                }
//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.DynamicTablesUIException("Impossible de générer la sortie", err));
//            }
//            return html.ToString();
//        }
//        #endregion

//        #region Tableau de types 1 et 2
//        /// <summary>
//        /// Tableau de types 1 et 2
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="html">Chaine HTML</param>
//        /// <param name="data">Données</param>
//        /// <param name="excel">True si sortie Excel, false sinon</param>
//        public static void GetDynamicTableUI_1_2(WebSession webSession, StringBuilder html, object[,] data, bool excel) {

//            #region Variables
//            //compteurs			
//            int i, j;

//            bool display = true;

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            if (webSession.PreformatedTable == TblFormatCst.PreformatedTables.product_X_Year
//                && !(webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.group
//                || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupSegment
//                )
//            ) {
//                PERSO_COLUMN = 1;
//            }

//            //index de colonnes
//            int FIRST_DATA_INDEX = 0;
//            do {
//                FIRST_DATA_INDEX++;
//            }
//            while (FIRST_DATA_INDEX < data.GetLength(1) && data[0, FIRST_DATA_INDEX] == null);

//            bool evolution = webSession.Evolution && webSession.ComparativeStudy;
//            bool percentage = (webSession.PDM && webSession.PreformatedTable == TblFormatCst.PreformatedTables.media_X_Year)
//                || (webSession.PDV && webSession.PreformatedTable == TblFormatCst.PreformatedTables.product_X_Year);

//            int L3_DATA_INDEX = FIRST_DATA_INDEX - 3;
//            int L4_DATA_INDEX = FIRST_DATA_INDEX - 2;
//            int L5_DATA_INDEX = FIRST_DATA_INDEX - 1;

//            //style Css
//            string HEADER_CSS = "";
//            string L0_CSS = "";
//            string L3_CSS = "";
//            string L4_CSS = "";
//            string L4_COMPET_CSS = "";
//            string L4_REF_CSS = "";
//            string L5_CSS = "";
//            string L5_COMPET_CSS = "";
//            string L5_REF_CSS = "";

//            if (!excel) { //Css html
//                HEADER_CSS = "astd0";
//                L0_CSS = "asl0";
//                L3_CSS = "asl3";
//                L4_CSS = FIRST_DATA_INDEX < 3 ? "asl3" : "asl4";
//                L4_COMPET_CSS = "asl4c";
//                L4_REF_CSS = "asl4r";
//                L5_CSS = FIRST_DATA_INDEX < 2 ? "asl4" : "asl5"; //données
//                L5_COMPET_CSS = FIRST_DATA_INDEX < 2 ? "asl4c" : "asl5bc"; //données
//                L5_REF_CSS = FIRST_DATA_INDEX < 2 ? "asl4r" : "asl5br"; //données
//                if (webSession.PreformatedTable.ToString().StartsWith("media")) {
//                    L4_CSS = FIRST_DATA_INDEX < 4 ? "asl3" : "asl4";
//                    L5_CSS = FIRST_DATA_INDEX < 3 ? "asl4" : "asl5"; //données
//                }
//            }
//            else { //Css Excel
//                HEADER_CSS = "astd0x";
//                L0_CSS = "asl0";
//                L3_CSS = "asl3x";
//                L4_CSS = FIRST_DATA_INDEX < 3 ? "asl3x" : "asl4x";
//                L4_COMPET_CSS = "asl4cx";
//                L4_REF_CSS = "asl4rx";
//                L5_CSS = FIRST_DATA_INDEX < 2 ? "asl4x" : "asl5x"; //données
//                L5_COMPET_CSS = FIRST_DATA_INDEX < 2 ? "asl4cx" : "asl5bcx"; //données
//                L5_REF_CSS = FIRST_DATA_INDEX < 2 ? "asl4rx" : "asl5brx"; //données
//                if (webSession.PreformatedTable.ToString().StartsWith("media")) {
//                    L4_CSS = FIRST_DATA_INDEX < 4 ? "asl3x" : "asl4x";
//                    L5_CSS = FIRST_DATA_INDEX < 3 ? "asl4x" : "asl5x"; //données
//                }
//            }

//            string lineCssClass = "";

//            #endregion

//            #region ouverture du tableau
//            html.Append("<table cellPadding=0 cellSpacing=1px border=0>");
//            #endregion

//            #region entêtes tableau
//            html.Append("<tr class=" + HEADER_CSS + ">");
//            if (webSession.PreformatedTable != TblFormatCst.PreformatedTables.media_X_Year)
//                html.Append("<td>" + GestionWeb.GetWebWord(1164, webSession.SiteLanguage) + "</td>");
//            else
//                html.Append("<td>" + GestionWeb.GetWebWord(1357, webSession.SiteLanguage) + "</td>");
//            html.Append("<td>" + FctUtilities.Dates.getPeriodLabel(webSession, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.currentYear) + "</td>");
//            //PDM
//            if (webSession.PDM && webSession.PreformatedTable == TblFormatCst.PreformatedTables.media_X_Year)
//                html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
//            //PDV
//            if (webSession.PDV && webSession.PreformatedTable == TblFormatCst.PreformatedTables.product_X_Year)
//                html.Append("<td>" + GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
//            //N-1
//            if (webSession.ComparativeStudy) {
//                html.Append("<td>" + FctUtilities.Dates.getPeriodLabel(webSession, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousYear) + "</td>");
//                //PDV N-1
//                if (webSession.PDV && webSession.PreformatedTable == TblFormatCst.PreformatedTables.product_X_Year)
//                    html.Append("<td>" + GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + (int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
//                //PDM N-1
//                if (webSession.PDM && webSession.PreformatedTable == TblFormatCst.PreformatedTables.media_X_Year)
//                    html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + (int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
//                //Evol
//                if (webSession.Evolution)
//                    html.Append("<td>" + GestionWeb.GetWebWord(1168, webSession.SiteLanguage) + "</td>");
//            }
//            html.Append("</tr>");
//            #endregion

//            #region Corps du tableau
//            //ligne totale
//            html.Append("<tr class=" + (lineCssClass = L0_CSS) + "><td align=left nowrap>" + data[0, 0] + "</td>");
//            for (j = FIRST_DATA_INDEX; j < data.GetLength(1) - PERSO_COLUMN; j++) {
//                if (data[0, j] != null)
//                    NumericDataAppend(webSession, html, double.Parse(data[0, j].ToString()), evolution, percentage,
//                        data.GetUpperBound(1) - PERSO_COLUMN, j, FIRST_DATA_INDEX, excel);
//                else
//                    html.Append("<td></td>");
//            }
//            html.Append("</tr>");
//            //lignes suivantes
//            StringBuilder headersHtml = new StringBuilder(300);
//            for (i = 1; i < data.GetLength(0); i++) {
//                for (j = 0; j < data.GetLength(1) - PERSO_COLUMN; j++) {

//                    if (j == L3_DATA_INDEX) {
//                        if (data[i, j] != null) {
//                            html.Append("<tr class=" + (lineCssClass = L3_CSS) + "><td align=left nowrap>" + data[i, j] + "</td>");
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else if (j == L4_DATA_INDEX) {
//                        if (PERSO_COLUMN < 1)
//                            display = true;
//                        else if (webSession.PreformatedProductDetail.ToString().StartsWith(TblFormatCst.PreformatedProductDetails.advertiser.ToString())
//                            && webSession.PersonalizedElementsOnly
//                            && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none)
//                            display = false;
//                        else
//                            display = true;
//                        if (display && data[i, j] != null) {
//                            lineCssClass = L4_CSS;
//                            if (PERSO_COLUMN > 0) {
//                                if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                    lineCssClass = L4_COMPET_CSS;
//                                else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                    lineCssClass = L4_REF_CSS;
//                            }
//                            headersHtml.Length = 0;
//                            headersHtml.Append("<tr class=" + lineCssClass + "><td align=left nowrap>" + data[i, j] + "</td>");
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else if (j == L5_DATA_INDEX) {
//                        if (PERSO_COLUMN < 1) {
//                            display = true;
//                        }
//                        else if (webSession.PersonalizedElementsOnly && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none)
//                            display = false;
//                        else
//                            display = true;
//                        if (display && data[i, j] != null) {
//                            html.Append(headersHtml.ToString());
//                            headersHtml.Length = 0;
//                            lineCssClass = L5_CSS;
//                            if (PERSO_COLUMN > 0) {
//                                if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                    lineCssClass = L5_COMPET_CSS;
//                                else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                    lineCssClass = L5_REF_CSS;
//                            }
//                            html.Append("<tr class=" + lineCssClass + "><td align=left nowrap>" + data[i, j] + "</td>");
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else {
//                        if (display) {
//                            if (data[i, j] != null) {
//                                if (headersHtml.Length < 1) {
//                                    NumericDataAppend(webSession, html, double.Parse(data[i, j].ToString()), evolution,
//                                        percentage, data.GetUpperBound(1) - PERSO_COLUMN, j, FIRST_DATA_INDEX, excel);
//                                }
//                                else {
//                                    NumericDataAppend(webSession, headersHtml, double.Parse(data[i, j].ToString()), evolution,
//                                        percentage, data.GetUpperBound(1) - PERSO_COLUMN, j, FIRST_DATA_INDEX, excel);
//                                }
//                            }
//                            else {
//                                if (headersHtml.Length < 1) {
//                                    html.Append("<td></td>"); //<td>- </td>
//                                }
//                                else {
//                                    headersHtml.Append("<td></td>"); //<td>- </td>
//                                }
//                            }
//                        }
//                    }
//                }
//                html.Append("</tr>");
//            }
//            #endregion

//            #region fermeture des balise du tableau
//            html.Append("</table>");
//            #endregion

//        }
//        #endregion

//        #region Tableau de types 3,4,10,11
//        /// <summary>
//        /// Tableau de types 3 et 4, 10 et 11
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="html">Chaine HTML</param>
//        /// <param name="data">Données</param>
//        /// <param name="extendedToMonths"></param>
//        /// <param name="excel">True si sortie Excel, false sinon</param>
//        public static void GetDynamicTableUI_3_4_10_11(WebSession webSession, StringBuilder html, object[,] data, bool excel, bool extendedToMonths) {

//            #region Variables
//            int i = 0;
//            int j = 0;

//            //index de colonnes
//            //année N
//            int N_COLUMN = 0;
//            do {
//                N_COLUMN++;
//            }
//            while (N_COLUMN < data.GetLength(1) && data[0, N_COLUMN] == null);

//            //PDV
//            int PDV_COLUMN = -1;
//            if (webSession.PDV)
//                PDV_COLUMN = N_COLUMN + 1;

//            //PDM
//            int PDM_COLUMN = -1;
//            if (webSession.PDM)
//                PDM_COLUMN = Math.Max(PDV_COLUMN, N_COLUMN) + 1;

//            //N-1
//            int N_1_COLUMN = -1;
//            int EVOL_COLUMN = -1;
//            int PDM_N_1_COLUMN = -1;
//            int PDV_N_1_COLUMN = -1;
//            if (webSession.ComparativeStudy) {
//                N_1_COLUMN = Math.Max(PDV_COLUMN, Math.Max(PDM_COLUMN, N_COLUMN)) + 1;
//                //PDV N-1
//                if (webSession.PDV)
//                    PDV_N_1_COLUMN = N_1_COLUMN + 1;
//                //PDM N-1
//                if (webSession.PDM)
//                    PDM_N_1_COLUMN = Math.Max(PDV_N_1_COLUMN, N_1_COLUMN) + 1;
//                //Evol
//                if (webSession.Evolution)
//                    EVOL_COLUMN = Math.Max(N_1_COLUMN, Math.Max(PDV_N_1_COLUMN, PDM_N_1_COLUMN)) + 1;
//            }

//            //mois de n
//            int MONTH_COLUMN = -1;
//            int LAST_MONTH_COLUMN = -2;
//            string absolutePeriodEnd = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (extendedToMonths) {
//                MONTH_COLUMN = Math.Max(EVOL_COLUMN,
//                    Math.Max(PDV_N_1_COLUMN,
//                    Math.Max(PDM_N_1_COLUMN,
//                    Math.Max(N_1_COLUMN,
//                    Math.Max(PDV_COLUMN,
//                    Math.Max(PDM_COLUMN, N_COLUMN)
//                    ))))) + 1;
//                LAST_MONTH_COLUMN = MONTH_COLUMN + (int.Parse(absolutePeriodEnd.Substring(4, 2)) - int.Parse(webSession.PeriodBeginningDate.Substring(4, 2)));
//            }

//            //type d'annonceur
//            int ADVERTISER_COLUMN = data.GetLength(1) - 1;

//            //nomenclature de la ligne
//            int CLASSIF_TYPE_COLUMN = ADVERTISER_COLUMN - 1;


//            int MEDIA_LEVEL_NUMBER;
//            switch (webSession.PreformatedMediaDetail) {
//                case TblFormatCst.PreformatedMediaDetails.vehicle:
//                    MEDIA_LEVEL_NUMBER = 1;
//                    break;
//                case TblFormatCst.PreformatedMediaDetails.vehicleCategory:
//                case TblFormatCst.PreformatedMediaDetails.vehicleMedia:
//                    MEDIA_LEVEL_NUMBER = 2;
//                    break;
//                default:
//                    MEDIA_LEVEL_NUMBER = 3;
//                    break;
//            }

//            int PRODUCT_LEVEL_NUMBER;
//            switch (webSession.PreformatedProductDetail) {
//                case TblFormatCst.PreformatedProductDetails.advertiser:
//                case TblFormatCst.PreformatedProductDetails.brand:
//                case TblFormatCst.PreformatedProductDetails.group:
//                case TblFormatCst.PreformatedProductDetails.product:
//                    PRODUCT_LEVEL_NUMBER = 1;
//                    break;
//                default:
//                    PRODUCT_LEVEL_NUMBER = 2;
//                    break;
//            }

//            //nomenclatures et CSS
//            ClassifCst.Branch.type MAIN_CLASSIF_TYPE;

//            int MAIN_LEVEL_L1 = 0;
//            int MAIN_LEVEL_L2 = 1;
//            int MAIN_LEVEL_L3 = 2;
//            int SCD_LEVEL_L1 = 0;
//            int SCD_LEVEL_L2 = 1;
//            int SCD_LEVEL_L3 = 2;

//            string MAIN_L1_CSS = "";
//            string MAIN_L2_CSS = "";
//            string MAIN_L3_CSS = "";
//            string SCD_L1_CSS = "";
//            string SCD_L2_CSS = "";
//            string SCD_L3_CSS = "";

//            string MAIN_L1_REF_CSS = "";
//            string MAIN_L2_REF_CSS = "";
//            string MAIN_L3_REF_CSS = "";
//            string SCD_L1_REF_CSS = "";
//            string SCD_L2_REF_CSS = "";
//            string SCD_L3_REF_CSS = "";

//            string MAIN_L1_CON_CSS = "";
//            string MAIN_L2_CON_CSS = "";
//            string MAIN_L3_CON_CSS = "";
//            string SCD_L1_CON_CSS = "";
//            string SCD_L2_CON_CSS = "";
//            string SCD_L3_CON_CSS = "";

//            string HEADER_CSS = "astd0";
//            if (webSession.PreformatedTable != TblFormatCst.PreformatedTables.mediaProduct_X_Year
//                && webSession.PreformatedTable != TblFormatCst.PreformatedTables.mediaProduct_X_YearMensual) {
//                if (!excel) { //css html
//                    //tableau produit =+> support
//                    MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.product;
//                    MAIN_L1_CSS = "asl0";
//                    MAIN_L1_REF_CSS = "asl0r";
//                    MAIN_L1_CON_CSS = "asl0c";
//                    MAIN_L2_CSS = "asl3";
//                    MAIN_L2_REF_CSS = "asl3r";
//                    MAIN_L2_CON_CSS = "asl3c";
//                    MAIN_L3_CSS = "asl3b";
//                    MAIN_L3_CON_CSS = "asl3bc";
//                    MAIN_L3_REF_CSS = "asl3br";
//                    SCD_L1_CSS = "asl2";
//                    SCD_L2_CSS = "asl5";
//                    SCD_L3_CSS = "asl5b";
//                    MAIN_LEVEL_L1 = 0;
//                    MAIN_LEVEL_L2 = 1;
//                    MAIN_LEVEL_L3 = (PRODUCT_LEVEL_NUMBER > 1) ? 2 : -1;
//                    SCD_LEVEL_L1 = 0;
//                    //niveau inférieur si le détail le precise ou si on est en pluri
//                    SCD_LEVEL_L2 = (MEDIA_LEVEL_NUMBER > 1) ? 1 : -1;
//                    SCD_LEVEL_L3 = (MEDIA_LEVEL_NUMBER > 2) ? 2 : -1; ;
//                }
//                else { // css excel
//                    HEADER_CSS = "astd0x";
//                    //tableau produit =+> support
//                    MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.product;
//                    MAIN_L1_CSS = "asl0";
//                    MAIN_L1_REF_CSS = "asl0rx";
//                    MAIN_L1_CON_CSS = "asl0cx";
//                    MAIN_L2_CSS = "asl3x";
//                    MAIN_L2_REF_CSS = "asl3rx";
//                    MAIN_L2_CON_CSS = "asl3cx";
//                    MAIN_L3_CSS = "asl3bx";
//                    MAIN_L3_CON_CSS = "asl3bcx";
//                    MAIN_L3_REF_CSS = "asl3brx";
//                    SCD_L1_CSS = "asl2x";
//                    SCD_L2_CSS = "asl5x";
//                    SCD_L3_CSS = "asl5bx";
//                    MAIN_LEVEL_L1 = 0;
//                    MAIN_LEVEL_L2 = 1;
//                    MAIN_LEVEL_L3 = (PRODUCT_LEVEL_NUMBER > 1) ? 2 : -1;
//                    SCD_LEVEL_L1 = 0;
//                    //niveau inférieur si le détail le precise ou si on est en pluri
//                    SCD_LEVEL_L2 = (MEDIA_LEVEL_NUMBER > 1) ? 1 : -1;
//                    SCD_LEVEL_L3 = (MEDIA_LEVEL_NUMBER > 2) ? 2 : -1; ;
//                }
//            }
//            else {
//                if (!excel) { //css html
//                    //tableau support =+> produit
//                    MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.media;
//                    MAIN_L1_CSS = "asl0";
//                    MAIN_L1_REF_CSS = "asl0r";
//                    MAIN_L1_CON_CSS = "asl0c";
//                    MAIN_L2_CSS = (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassif.Vehicles.names.plurimedia.GetHashCode())
//                        ? "asl2" : "asl5";
//                    MAIN_L3_CSS = "asl5b";
//                    SCD_L1_CSS = "asl3";
//                    SCD_L1_REF_CSS = "asl3r";
//                    SCD_L1_CON_CSS = "asl3c";
//                    SCD_L2_REF_CSS = "asl3br";
//                    SCD_L2_CON_CSS = "asl3bc";
//                    SCD_L2_CSS = "asl3b";
//                    SCD_L3_CSS = "asl5";
//                    SCD_L3_REF_CSS = "asl5r";
//                    SCD_L3_CON_CSS = "asl5b";
//                    MAIN_LEVEL_L1 = 0;
//                    MAIN_LEVEL_L2 = (MEDIA_LEVEL_NUMBER > 1 ||
//                        ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassif.Vehicles.names.plurimedia.GetHashCode()
//                        ) ? 1 : -1;
//                    MAIN_LEVEL_L3 = (MEDIA_LEVEL_NUMBER > 2) ? 2 : -1;
//                    SCD_LEVEL_L1 = 0;
//                    SCD_LEVEL_L2 = (PRODUCT_LEVEL_NUMBER > 1) ? 1 : -1;
//                    SCD_LEVEL_L3 = -1;
//                }
//                else { //css excel
//                    HEADER_CSS = "astd0x";
//                    //tableau support =+> produit
//                    MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.media;
//                    //SCD_CLASSIF_TYPE = ClassifCst.Branch.type.product;
//                    MAIN_L1_CSS = "asl0";
//                    MAIN_L1_REF_CSS = "asl0rx";
//                    MAIN_L1_CON_CSS = "asl0cx";
//                    MAIN_L2_CSS = (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassif.Vehicles.names.plurimedia.GetHashCode())
//                        ? "asl2x" : "asl5x";
//                    MAIN_L3_CSS = "asl5bx";
//                    SCD_L1_CSS = "asl3x";
//                    SCD_L1_REF_CSS = "asl3rx";
//                    SCD_L1_CON_CSS = "asl3cx";
//                    SCD_L2_REF_CSS = "asl3brx";
//                    SCD_L2_CON_CSS = "asl3bcx";
//                    SCD_L2_CSS = "asl3bx";
//                    SCD_L3_CSS = "asl5x";
//                    SCD_L3_REF_CSS = "asl5rx";
//                    SCD_L3_CON_CSS = "asl5bx";
//                    MAIN_LEVEL_L1 = 0;
//                    MAIN_LEVEL_L2 = (MEDIA_LEVEL_NUMBER > 1 ||
//                        ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassif.Vehicles.names.plurimedia.GetHashCode()
//                        ) ? 1 : -1;
//                    MAIN_LEVEL_L3 = (MEDIA_LEVEL_NUMBER > 2) ? 2 : -1;
//                    SCD_LEVEL_L1 = 0;
//                    SCD_LEVEL_L2 = (PRODUCT_LEVEL_NUMBER > 1) ? 1 : -1;
//                    SCD_LEVEL_L3 = -1;
//                }
//            }

//            #endregion

//            #region ouverture tableau
//            html.Append("<table>");
//            #endregion

//            #region entetes
//            html.Append("<tr class=" + HEADER_CSS + ">");
//            if (webSession.PreformatedTable != TblFormatCst.PreformatedTables.media_X_Year)
//                html.Append("<td colspan=2>" + GestionWeb.GetWebWord(1164, webSession.SiteLanguage) + "</td>");
//            else
//                html.Append("<td>" + GestionWeb.GetWebWord(1357, webSession.SiteLanguage) + "</td>");
//            html.Append("<td>" + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
//            //PDV
//            if (webSession.PDV)
//                html.Append("<td>" + GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + " " + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");//GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + 
//            //PDM
//            if (webSession.PDM)
//                html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + " " + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
//            //N-1
//            if (webSession.ComparativeStudy) {
//                html.Append("<td>" + (int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
//                //PDV N-1
//                if (webSession.PDV)
//                    html.Append("<td>" + GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + " " + (int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
//                //PDM N-1
//                if (webSession.PDM)
//                    html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + " " + (int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
//                //Evol
//                if (webSession.Evolution)
//                    html.Append("<td>" + GestionWeb.GetWebWord(1168, webSession.SiteLanguage) + "</td>");
//            }
//            //Months
//            if (extendedToMonths) {
//                for (int w = 0; w <= (int.Parse(absolutePeriodEnd.Substring(4, 2)) - int.Parse(webSession.PeriodBeginningDate.Substring(4, 2))); w++) {
//                    html.Append("<td>" + WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).AddMonths(w).ToString("MMMM") + "</td>");
//                }
//            }
//            html.Append("</tr>");
//            #endregion

//            #region Ajout des données
//            string offset = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
//            string mainOffset = "&nbsp;&nbsp;&nbsp;";
//            string LINE_CSS;
//            double tmpData = 0.0;
//            int father_level = 0;
//            bool displayFather = true;
//            bool dysplayChild = true;

//            StringBuilder outputHtml = html;

//            bool newLine = false;

//            //varaibles utilisées pour l affichage correct des elts personaliser
//            //buffer utilisé pour rediriger la sortie du code html vers un buffer temporaire
//            StringBuilder bufferHtml = new StringBuilder(50000);
//            //Contenant temporaire poue une entete de la nomenclature secondaire
//            StringBuilder scndHeaderHtml = new StringBuilder(50000);
//            bool addScndData = false;

//            while (i < data.GetLength(0)) {

//                newLine = false;

//                if (data[i, 0] != null) {
//                    if (data[i, 0].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
//                        break;
//                }

//                if (MAIN_CLASSIF_TYPE == (ClassifCst.Branch.type)data[i, CLASSIF_TYPE_COLUMN]) {

//                    displayFather = true;

//                    for (j = 0; j < data.GetLength(1); j++) {

//                        if (data[i, j] != null) {
//                            if (j == MAIN_LEVEL_L1) {
//                                LINE_CSS = MAIN_L1_CSS;
//                                newLine = true;
//                                outputHtml.Append("<tr class=" + LINE_CSS + " style=\"FONT-WEIGHT: bold\">");
//                                outputHtml.Append("<td colspan=2 align=left nowrap>" + data[i, j].ToString() + "</td>");
//                                father_level = j;
//                                j = N_COLUMN - 1;
//                            }
//                            else if (j == MAIN_LEVEL_L2) {
//                                if (WebCst.AdvertiserPersonalisation.Type.none != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN]) {
//                                    if (WebCst.AdvertiserPersonalisation.Type.competitor != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN])
//                                        LINE_CSS = MAIN_L2_REF_CSS;
//                                    else
//                                        LINE_CSS = MAIN_L2_CON_CSS;
//                                }
//                                else {
//                                    LINE_CSS = MAIN_L2_CSS;
//                                    if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.product && webSession.PersonalizedElementsOnly) {
//                                        if (webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupBrand ||
//                                            webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupProduct ||
//                                            webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupAdvertiser
//                                            || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.segmentAdvertiser
//                                            //toto
//                                            || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.product
//                                            || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.segmentBrand
//                                            || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.segmentProduct) {
//                                            bufferHtml.Length = 0;
//                                            outputHtml = bufferHtml;
//                                        }
//                                        //elet concurrent ou referent
//                                        if (webSession.PreformatedProductDetail.ToString().StartsWith(TblFormatCst.PreformatedProductDetails.advertiser.ToString())
//                                            || webSession.PreformatedProductDetail.ToString().StartsWith(TblFormatCst.PreformatedProductDetails.brand.ToString())
//                                            )
//                                            displayFather = false;
//                                    }
//                                    else if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media && webSession.PersonalizedElementsOnly
//                                        && webSession.PreformatedProductDetail != TblFormatCst.PreformatedProductDetails.groupSegment) {
//                                        bufferHtml.Length = 0;
//                                        outputHtml = bufferHtml;
//                                    }
//                                }
//                                if (displayFather) {
//                                    newLine = true;
//                                    outputHtml.Append("<tr class=" + LINE_CSS + " style=\"FONT-WEIGHT: bold\">");
//                                    outputHtml.Append("<td colspan=2 align=left nowrap>" + data[i, j].ToString() + "</td>");
//                                    father_level = j;
//                                    j = N_COLUMN - 1;
//                                }
//                            }
//                            else if (j == MAIN_LEVEL_L3) {
//                                //affichage
//                                if (displayFather) {
//                                    if (WebCst.AdvertiserPersonalisation.Type.none != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN]) {
//                                        outputHtml = html;
//                                        html.Append(bufferHtml.ToString());
//                                        bufferHtml.Length = 0;
//                                        if (WebCst.AdvertiserPersonalisation.Type.competitor != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN])
//                                            LINE_CSS = MAIN_L3_REF_CSS;
//                                        else
//                                            LINE_CSS = MAIN_L3_CON_CSS;
//                                    }
//                                    else {
//                                        LINE_CSS = MAIN_L3_CSS;
//                                        //elet concurrent ou referent
//                                        if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.product && webSession.PersonalizedElementsOnly
//                                            && webSession.PreformatedProductDetail != TblFormatCst.PreformatedProductDetails.groupSegment)
//                                            displayFather = false;
//                                    }
//                                    if (displayFather) {
//                                        newLine = true;
//                                        outputHtml.Append("<tr class=" + LINE_CSS + " style=\"FONT-WEIGHT: bold\">");
//                                        outputHtml.Append("<td colspan=2 align=left nowrap>" + mainOffset + data[i, j].ToString() + "</td>");
//                                        father_level = j;
//                                        j = N_COLUMN - 1;
//                                    }
//                                }
//                            }
//                            else if ((j == N_COLUMN || j == N_1_COLUMN || (j >= MONTH_COLUMN && j <= LAST_MONTH_COLUMN)) && displayFather) {
//                                //année
//                                tmpData = double.Parse(data[i, j].ToString());
//                                outputHtml.Append("<td nowrap>" + ((!double.IsNaN(tmpData) && tmpData != 0) ? WebFunctions.Units.ConvertUnitValueToString(tmpData.ToString(), webSession.Unit) : "") + "</td>");
//                            }
//                            else if ((j == PDV_COLUMN || j == PDM_COLUMN || j == PDM_N_1_COLUMN || j == PDV_N_1_COLUMN) && displayFather) {
//                                //pourcentage
//                                tmpData = double.Parse(data[i, j].ToString());
//                                outputHtml.Append("<td nowrap>" + ((tmpData == 0 || Double.IsNaN(tmpData) || Double.IsInfinity(tmpData)) ? "" : WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %") + "</td>");
//                            }
//                            else if (j == EVOL_COLUMN && displayFather) {
//                                if (!excel) {
//                                    //evol
//                                    tmpData = double.Parse(data[i, j].ToString());
//                                    if (tmpData > 0) //hausse
//                                        outputHtml.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/g.gif></td>");
//                                    else if (tmpData < 0) //baisse
//                                        outputHtml.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/r.gif></td>");
//                                    else if (!Double.IsNaN(tmpData)) // 0 exactement
//                                        outputHtml.Append("<td nowrap>0 %<img src=/I/o.gif></td>");
//                                    else
//                                        outputHtml.Append("<td></td>"); //<td>- </td>
//                                }
//                                else if (excel) {
//                                    //evol
//                                    tmpData = double.Parse(data[i, j].ToString());
//                                    if (tmpData > 0) //hausse
//                                        outputHtml.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                                    else if (tmpData < 0) //baisse
//                                        outputHtml.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                                    else if (!Double.IsNaN(tmpData)) // 0 exactement
//                                        outputHtml.Append("<td nowrap> 0 %</td>");
//                                    else
//                                        outputHtml.Append("<td></td>");
//                                }
//                            }
//                        }
//                    }
//                    if (newLine)
//                        outputHtml.Append("</tr>");
//                }
//                else {
//                    if (displayFather) {
//                        StringBuilder dataOutput = outputHtml;
//                        dysplayChild = true;
//                        addScndData = false;
//                        for (j = 0; j < data.GetLength(1); j++) {

//                            if (data[i, j] != null) {

//                                if (j - father_level == SCD_LEVEL_L1) {
//                                    scndHeaderHtml.Length = 0;
//                                    if (WebCst.AdvertiserPersonalisation.Type.none != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN]) {
//                                        dataOutput = outputHtml = html;
//                                        outputHtml.Append(bufferHtml.ToString());
//                                        bufferHtml.Length = 0;
//                                        if (WebCst.AdvertiserPersonalisation.Type.competitor != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN])
//                                            LINE_CSS = (father_level != 0) ? SCD_L1_REF_CSS : MAIN_L1_REF_CSS;
//                                        else
//                                            LINE_CSS = (father_level != 0) ? SCD_L1_CON_CSS : MAIN_L1_CON_CSS;
//                                    }
//                                    else {
//                                        LINE_CSS = (father_level != 0) ? SCD_L1_CSS : MAIN_L1_CSS;
//                                        if (webSession.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media
//                                            && webSession.PreformatedProductDetail != TblFormatCst.PreformatedProductDetails.groupSegment) {
//                                            dysplayChild = false;
//                                            newLine = true;
//                                            scndHeaderHtml.Append("<tr Font-Italic=True class=" + LINE_CSS + " style=\"FONT-WEIGHT: normal\">");
//                                            scndHeaderHtml.Append("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>" + offset + data[i, j].ToString() + "</td>");
//                                            addScndData = true;
//                                            dataOutput = scndHeaderHtml;
//                                            j = N_COLUMN - 1;
//                                        }
//                                    }

//                                    if (dysplayChild) {
//                                        newLine = true;
//                                        outputHtml.Append("<tr Font-Italic=True class=" + LINE_CSS + " style=\"FONT-WEIGHT: normal\">");
//                                        outputHtml.Append("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>" + offset + data[i, j].ToString() + "</td>");
//                                        j = N_COLUMN - 1;
//                                    }
//                                }
//                                else if (j - father_level == SCD_LEVEL_L2) {

//                                    if (WebCst.AdvertiserPersonalisation.Type.none != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN]) {
//                                        dataOutput = outputHtml = html;
//                                        outputHtml.Append(bufferHtml.ToString());
//                                        bufferHtml.Length = 0;
//                                        if (WebCst.AdvertiserPersonalisation.Type.competitor != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN])
//                                            LINE_CSS = (father_level != 0) ? SCD_L2_REF_CSS : MAIN_L1_REF_CSS;
//                                        else
//                                            LINE_CSS = (father_level != 0) ? SCD_L2_CON_CSS : MAIN_L1_CON_CSS;
//                                    }
//                                    else {
//                                        LINE_CSS = (father_level != 0) ? SCD_L2_CSS : MAIN_L1_CSS;
//                                        if (webSession.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media
//                                            && webSession.PreformatedProductDetail != TblFormatCst.PreformatedProductDetails.groupSegment)
//                                            dysplayChild = false;
//                                    }

//                                    if (dysplayChild) {
//                                        newLine = true;
//                                        outputHtml.Append(scndHeaderHtml.ToString());///////////////////
//                                        scndHeaderHtml.Length = 0;////////////
//                                        outputHtml.Append("<tr class=" + LINE_CSS + " style=\"FONT-WEIGHT: normal\">");
//                                        outputHtml.Append("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>" + offset + offset + data[i, j].ToString() + "</td>");
//                                        j = N_COLUMN - 1;
//                                    }
//                                }
//                                else if (j - father_level == SCD_LEVEL_L3) {

//                                    if (WebCst.AdvertiserPersonalisation.Type.none != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN]) {
//                                        if (WebCst.AdvertiserPersonalisation.Type.competitor != (WebCst.AdvertiserPersonalisation.Type)data[i, ADVERTISER_COLUMN])
//                                            LINE_CSS = (father_level != 0) ? SCD_L3_REF_CSS : MAIN_L1_REF_CSS;
//                                        else
//                                            LINE_CSS = (father_level != 0) ? SCD_L3_CON_CSS : MAIN_L1_CON_CSS;
//                                    }
//                                    else {
//                                        LINE_CSS = (father_level != 0) ? SCD_L3_CSS : MAIN_L1_CSS;
//                                        if (webSession.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media)
//                                            dysplayChild = false;
//                                    }
//                                    if (dysplayChild) {
//                                        newLine = true;
//                                        outputHtml.Append(scndHeaderHtml.ToString());///////////////////
//                                        scndHeaderHtml.Length = 0;/////////////
//                                        outputHtml.Append("<tr Font-Italic=True class=" + LINE_CSS + " style=\"FONT-WEIGHT: normal\">");
//                                        outputHtml.Append("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>" + offset + offset + offset + data[i, j].ToString() + "</td>");
//                                        j = N_COLUMN - 1;
//                                    }
//                                }
//                                else if ((j == N_COLUMN || j == N_1_COLUMN || (j >= MONTH_COLUMN && j <= LAST_MONTH_COLUMN)) && (dysplayChild || addScndData)) {
//                                    //année
//                                    tmpData = double.Parse(data[i, j].ToString());
//                                    dataOutput.Append("<td nowrap>" + ((!double.IsNaN(tmpData) && tmpData != 0) ? WebFunctions.Units.ConvertUnitValueToString(tmpData.ToString(), webSession.Unit) : "") + "</td>");
//                                }
//                                else if ((j == PDV_COLUMN || j == PDM_COLUMN || j == PDM_N_1_COLUMN || j == PDV_N_1_COLUMN) && displayFather && (dysplayChild || addScndData)) {
//                                    //pourcentage
//                                    tmpData = double.Parse(data[i, j].ToString());
//                                    dataOutput.Append("<td nowrap>" + ((tmpData == 0 || Double.IsNaN(tmpData) || Double.IsInfinity(tmpData)) ? "" : WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %") + "</td>");
//                                }
//                                else if (j == EVOL_COLUMN && (dysplayChild || addScndData)) {
//                                    if (!excel) {
//                                        //evol
//                                        tmpData = double.Parse(data[i, j].ToString());
//                                        if (tmpData > 0) //hausse
//                                            dataOutput.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/g.gif></td>");
//                                        else if (tmpData < 0) //baisse
//                                            dataOutput.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/r.gif></td>");
//                                        else if (!Double.IsNaN(tmpData)) // 0 exactement
//                                            dataOutput.Append("<td nowrap>0 %<img src=/I/o.gif></td>");
//                                        else
//                                            dataOutput.Append("<td></td>");
//                                    }
//                                    else if (excel) {
//                                        //evol
//                                        tmpData = double.Parse(data[i, j].ToString());
//                                        if (tmpData > 0) //hausse
//                                            dataOutput.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                                        else if (tmpData < 0) //baisse
//                                            dataOutput.Append("<td nowrap>" + ((!Double.IsInfinity(tmpData)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(tmpData.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                                        else if (!Double.IsNaN(tmpData)) // 0 exactement
//                                            dataOutput.Append("<td nowrap> 0 %</td>");
//                                        else
//                                            dataOutput.Append("<td></td>");
//                                    }
//                                }
//                            }

//                        }
//                        if (newLine)
//                            dataOutput.Append("</tr>");
//                    }
//                }

//                i++;
//            }
//            #endregion

//            #region Fermeture tableau
//            html.Append("</table>");
//            #endregion

//        }
//        #endregion

//        #region Tableau	de type 5
//        /// <summary>
//        /// Tableau de type 5
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="html">Chaine HTML</param>
//        /// <param name="data">Données</param>
//        /// <param name="excel">True si sortie Excel, false sinon</param>
//        public static void GetDynamicTableUI_5(WebSession webSession, StringBuilder html, object[,] data, bool excel) {

//            #region Variables
//            //compteurs			
//            int i, j;

//            bool display = true;
//            // Savoir si l'on se trouve sur une ligne totale
//            bool totalLine = false;

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            if (!(webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.group
//                || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupSegment
//                )
//                ) {
//                PERSO_COLUMN = 1;
//            }

//            //html stuff
//            int beginningIndex;
//            int lastMainHeaderIndex;
//            string tmpHtml = "";

//            //variables extraites des données de session
//            int NB_OPTION = 0;
//            int NB_YEAR = 1;
//            if (webSession.ComparativeStudy) {
//                NB_YEAR++;
//                if (webSession.Evolution) NB_OPTION++;
//            }
//            if (webSession.PDM) {
//                NB_OPTION += NB_YEAR;
//            }
//            if (webSession.PDV) {
//                NB_OPTION += NB_YEAR;
//            }

//            //Indexe de lignes
//            int FIRST_DATA_LINE = 0;
//            do {
//                FIRST_DATA_LINE++;
//            }
//            while (FIRST_DATA_LINE < data.GetLength(0) && data[FIRST_DATA_LINE, 0] == null);

//            //index de colonnes
//            int FIRST_DATA_INDEX = 0;
//            do {
//                FIRST_DATA_INDEX++;
//            }
//            while (FIRST_DATA_INDEX < data.GetLength(1) && data[0, FIRST_DATA_INDEX] == null);


//            int L1_DATA_INDEX = FIRST_DATA_INDEX - 2;
//            int L2_DATA_INDEX = FIRST_DATA_INDEX - 1;

//            //style Css
//            string HEADER_CSS = "";
//            string L0_CSS = "";
//            string Light_L0_CSS = "";
//            string L1_CSS = "";
//            string L1_COMPET_CSS = "";
//            string L1_REF_CSS = "";
//            string Light_L1_CSS = "";
//            string Light_L1_COMPET_CSS = "";
//            string Light_L1_REF_CSS = "";
//            string L2_CSS = "";
//            string L2_COMPET_CSS = "";
//            string L2_REF_CSS = "";
//            string Light_L2_CSS = "";
//            string Light_L2_COMPET_CSS = "";
//            string Light_L2_REF_CSS = "";

//            if (!excel) { //Css html
//                HEADER_CSS = "astd0";
//                L0_CSS = "asl0";
//                Light_L0_CSS = "asl0";

//                L1_CSS = "asl3";
//                L1_COMPET_CSS = "asl3c";
//                L1_REF_CSS = "asl3r";

//                Light_L1_CSS = "asl3b";
//                Light_L1_COMPET_CSS = "asl3bc";
//                Light_L1_REF_CSS = "asl3br";

//                L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5" : "asl3";
//                L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5c" : "asl3c";
//                L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5r" : "asl3r";

//                Light_L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5b" : "asl3b";
//                Light_L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5bc" : "asl3bc";
//                Light_L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5br" : "asl3br";
//            }
//            else { //Css Excel
//                HEADER_CSS = "astd0x";
//                L0_CSS = "asl0";
//                Light_L0_CSS = "asl0x";

//                L1_CSS = "asl3x";
//                L1_COMPET_CSS = "asl3cx";
//                L1_REF_CSS = "asl3rx";

//                Light_L1_CSS = "asl3bx";
//                Light_L1_COMPET_CSS = "asl3bcx";
//                Light_L1_REF_CSS = "asl3brx";

//                L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5x" : "asl3x";
//                L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5cx" : "asl3cx";
//                L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5rx" : "asl3rx";

//                Light_L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5bx" : "asl3bx";
//                Light_L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5bcx" : "asl3bcx";
//                Light_L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5brx" : "asl3brx";
//            }

//            string lineCssClass = "";

//            #endregion

//            #region ouverture du tableau
//            html.Append("<table cellPadding=0 cellSpacing=1px border=0>");
//            #endregion

//            #region entêtes tableau
//            beginningIndex = html.Length;
//            if (FIRST_DATA_LINE > 1) {
//                //deux lignes d'entêtes
//                tmpHtml += "<tr class=" + HEADER_CSS + ">";
//                html.Append("<tr class=" + HEADER_CSS + ">");
//                tmpHtml += "<td rowSpan=2>" + GestionWeb.GetWebWord(1164, webSession.SiteLanguage) + "</td>";
//                lastMainHeaderIndex = FIRST_DATA_INDEX;
//                for (i = FIRST_DATA_INDEX; i < data.GetLength(1) - PERSO_COLUMN; i++) {
//                    if (data[1, i] != null)
//                        html.Append("<td nowrap>" + data[1, i].ToString() + "</td>");
//                    if (data[0, i] != null && i != FIRST_DATA_INDEX) {
//                        tmpHtml += "<td";
//                        if (lastMainHeaderIndex == FIRST_DATA_INDEX) tmpHtml += " rowSpan=2";
//                        tmpHtml += " colSpan=" + (i - lastMainHeaderIndex) + ">" + data[0, lastMainHeaderIndex] + "</td>";
//                        lastMainHeaderIndex = i;
//                    }
//                }
//                html.Append("</tr>");
//                tmpHtml += "<td colSpan=" + (i - lastMainHeaderIndex) + ">" + data[0, lastMainHeaderIndex] + "</td></tr>";
//                html.Insert(beginningIndex, tmpHtml);
//            }
//            else {
//                html.Append("<tr class=" + HEADER_CSS + ">");
//                html.Append("<td>" + GestionWeb.GetWebWord(1164, webSession.SiteLanguage) + "</td>");
//                for (i = FIRST_DATA_INDEX; i < data.GetLength(1) - PERSO_COLUMN; i++) {
//                    html.Append("<td>" + data[0, i].ToString() + "</td>");
//                }
//                html.Append("</tr>");
//            }
//            #endregion

//            #region Corps du tableau
//            //ligne totale
//            lineCssClass = L0_CSS;
//            // Ajout de 1 pour ne pas afficher la 1ère ligne
//            for (i = FIRST_DATA_LINE + 1; i <= FIRST_DATA_LINE + NB_YEAR + NB_OPTION; i++) {

//                html.Append("<tr class=" + lineCssClass + "><td");
//                if (i < 1 + FIRST_DATA_LINE) {
//                    html.Append(" align=left");
//                }
//                html.Append(" nowrap>" + data[i, 0] + "</td>");

//                for (j = FIRST_DATA_INDEX; j < data.GetLength(1) - PERSO_COLUMN; j++) {
//                    if (data[i, j] != null)
//                        NumericDataAppend(webSession, html, double.Parse(data[i, j].ToString()),
//                            IsMultiYearLine(i, FIRST_DATA_LINE, NB_YEAR, NB_OPTION), data[i, 0].ToString(), excel, false);
//                    else
//                        html.Append("<td>-</td>");

//                }
//                html.Append("</tr>");
//                lineCssClass = Light_L0_CSS;
//            }

//            //lignes suivantes
//            string align = "";
//            int labelIndex = 0;
//            StringBuilder bufferHtml = new StringBuilder(5000);
//            StringBuilder output = html;
//            bool newLine = false;
//            for (i = FIRST_DATA_LINE + NB_YEAR + NB_OPTION + 1; i < data.GetLength(0); i++) {
//                newLine = false;
//                for (j = 0; j < data.GetLength(1) - PERSO_COLUMN; j++) {

//                    if (j == L1_DATA_INDEX && data[i, j] != null) {
//                        if (PERSO_COLUMN < 1)
//                            display = true;
//                        else if (webSession.PersonalizedElementsOnly
//                            && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none
//                            && webSession.PreformatedProductDetail.ToString().StartsWith(TblFormatCst.PreformatedProductDetails.advertiser.ToString()))
//                            display = false;
//                        else {
//                            if (IsMultiYearLine(i, FIRST_DATA_LINE, NB_YEAR, NB_OPTION))
//                                bufferHtml.Length = 0;
//                            output = bufferHtml;
//                            display = true;
//                        }
//                        if (display) {
//                            if (!IsMultiYearLine(i, FIRST_DATA_LINE, NB_YEAR, NB_OPTION)) {
//                                totalLine = false;
//                                lineCssClass = Light_L1_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = Light_L1_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = Light_L1_REF_CSS;
//                                }
//                                align = "";
//                            }
//                            else {
//                                align = " align=left ";
//                                totalLine = true;
//                                lineCssClass = L1_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = L1_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = L1_REF_CSS;
//                                }
//                            }
//                            newLine = true;
//                            output.Append("<tr class=" + lineCssClass + "><td" + align + " nowrap>" + data[i, j] + "</td>");

//                            labelIndex = j;
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else if (j == L2_DATA_INDEX) {
//                        if (PERSO_COLUMN < 1)
//                            display = true;
//                        else if (webSession.PersonalizedElementsOnly && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none)
//                            display = false;
//                        else {
//                            display = true;
//                        }
//                        if (display && data[i, j] != null) {
//                            html.Append(bufferHtml.ToString());
//                            bufferHtml.Length = 0;
//                            output = html;
//                            if (!IsMultiYearLine(i, FIRST_DATA_LINE, NB_YEAR, NB_OPTION)) {
//                                totalLine = false;
//                                align = "";
//                                lineCssClass = Light_L2_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = Light_L2_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = Light_L2_REF_CSS;
//                                }
//                            }
//                            else {
//                                totalLine = true;
//                                align = " align=left ";
//                                lineCssClass = L2_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = L2_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = L2_REF_CSS;
//                                }
//                            }
//                            newLine = true;
//                            output.Append("<tr class=" + lineCssClass + "><td" + align + " nowrap>" + data[i, j] + "</td>");
//                            labelIndex = j;
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else {
//                        if (display) {
//                            if (data[i, j] != null) {
//                                NumericDataAppend(webSession, output, double.Parse(data[i, j].ToString()),
//                                    IsMultiYearLine(i, FIRST_DATA_LINE, NB_YEAR, NB_OPTION), data[i, labelIndex].ToString(), excel, totalLine);

//                            }
//                            else if (j >= FIRST_DATA_INDEX)
//                                output.Append("<td>-</td>");
//                        }

//                    }
//                }
//                if (newLine)
//                    output.Append("</tr>");
//            }
//            #endregion

//            #region fermeture des balise du tableau
//            html.Append("</table>");
//            #endregion

//        }
//        #endregion

//        #region Tableau de type 6, 7, 8, 9
//        /// <summary>
//        /// Tableau de types 6, 7, 8, 9
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="html">Chaine HTML</param>
//        /// <param name="data">Données</param>
//        /// <param name="excel">True si sortie Excel, false sinon</param>
//        public static void GetDynamicTableUI_6_7_8_9(WebSession webSession, StringBuilder html, object[,] data, bool excel) {

//            #region Variables

//            bool display = true;
//            // Savoir si l'on se trouve sur une ligne totale
//            bool totalLine = false;

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            if (!(webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.group
//                || webSession.PreformatedProductDetail == TblFormatCst.PreformatedProductDetails.groupSegment
//                || webSession.PreformatedTable == TblFormatCst.PreformatedTables.mediaYear_X_Mensual
//                || webSession.PreformatedTable == TblFormatCst.PreformatedTables.mediaYear_X_Cumul)
//                ) {
//                PERSO_COLUMN = 1;
//            }

//            //compteurs			
//            int i, j;

//            //Indexe de lignes

//            //variables extraites des données de session
//            int NB_OPTION = 0;
//            int NB_YEAR = 1;
//            if (webSession.ComparativeStudy) {
//                NB_YEAR++;
//                if (webSession.Evolution) {
//                    NB_OPTION++;
//                }
//            }

//            //nombre d'options PDM, PDV, Evol
//            if (webSession.PDM && webSession.PreformatedTable.ToString().StartsWith("media")) {
//                NB_OPTION += (webSession.ComparativeStudy) ? 2 : 1;
//            }

//            if (webSession.PDV && webSession.PreformatedTable.ToString().StartsWith("product")) {
//                NB_OPTION += (webSession.ComparativeStudy) ? 2 : 1;
//            }

//            //index de colonnes
//            int FIRST_DATA_INDEX = 0;
//            do {
//                FIRST_DATA_INDEX++;
//            }
//            while (FIRST_DATA_INDEX < data.GetLength(1) && data[0, FIRST_DATA_INDEX] == null);


//            int L1_DATA_INDEX = FIRST_DATA_INDEX - 2;
//            int L2_DATA_INDEX = FIRST_DATA_INDEX - 1;

//            //style Css
//            string HEADER_CSS = "";
//            string L0_CSS = "";
//            string LIGHT_L0_CSS = "";
//            string L1_CSS = "";
//            string L1_COMPET_CSS = "";
//            string L1_REF_CSS = "";
//            string LIGHT_L1_CSS = "";
//            string LIGHT_L1_COMPET_CSS = "";
//            string LIGHT_L1_REF_CSS = "";
//            string L2_CSS = "";
//            string L2_COMPET_CSS = "";
//            string L2_REF_CSS = "";
//            string LIGHT_L2_CSS = "";
//            string LIGHT_L2_COMPET_CSS = "";
//            string LIGHT_L2_REF_CSS = "";
//            string lineCssClass = "";

//            if (!excel) { //Css html
//                HEADER_CSS = "astd0";
//                L0_CSS = "asl0";
//                LIGHT_L0_CSS = "asl0";

//                L1_CSS = "asl3";
//                L1_COMPET_CSS = "asl3c";
//                L1_REF_CSS = "asl3r";

//                LIGHT_L1_CSS = "asl3b";
//                LIGHT_L1_COMPET_CSS = "asl3bc";
//                LIGHT_L1_REF_CSS = "asl3br";

//                L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5" : "asl3";
//                L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5c" : "asl3c";
//                L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5r" : "asl3r";

//                LIGHT_L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5b" : "asl3b";
//                LIGHT_L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5bc" : "asl3bc";
//                LIGHT_L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5br" : "asl3br";
//                lineCssClass = "";

//                if (webSession.PreformatedTable.ToString().StartsWith("media")) {
//                    L2_CSS = L1_DATA_INDEX > 0 ? "asl5" : "asl3";
//                    LIGHT_L2_CSS = L1_DATA_INDEX > 0 ? "asl5b" : "asl3b";
//                }
//            }
//            else { //Css Excel
//                HEADER_CSS = "astd0x";
//                L0_CSS = "asl0";
//                LIGHT_L0_CSS = "asl0";

//                L1_CSS = "asl3x";
//                L1_COMPET_CSS = "asl3cx";
//                L1_REF_CSS = "asl3rx";

//                LIGHT_L1_CSS = "asl3bx";
//                LIGHT_L1_COMPET_CSS = "asl3bcx";
//                LIGHT_L1_REF_CSS = "asl3brx";

//                L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5x" : "asl3x";
//                L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5cx" : "asl3cx";
//                L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5rx" : "asl3rx";

//                LIGHT_L2_CSS = (L1_DATA_INDEX >= 0) ? "asl5bx" : "asl3bx";
//                LIGHT_L2_COMPET_CSS = (L1_DATA_INDEX >= 0) ? "asl5bcx" : "asl3bcx";
//                LIGHT_L2_REF_CSS = (L1_DATA_INDEX >= 0) ? "asl5brx" : "asl3brx";
//                lineCssClass = "";

//                if (webSession.PreformatedTable.ToString().StartsWith("media")) {
//                    L2_CSS = L1_DATA_INDEX > 0 ? "asl5x" : "asl3x";
//                    LIGHT_L2_CSS = L1_DATA_INDEX > 0 ? "asl5bx" : "asl3bx";
//                }
//            }
//            #endregion

//            #region ouverture du tableau
//            html.Append("<table cellPadding=0 cellSpacing=1px border=0>");
//            //html.Append("<table>");
//            #endregion

//            #region entêtes tableau
//            html.Append("<tr class=" + HEADER_CSS + ">");
//            if (webSession.PreformatedTable != TblFormatCst.PreformatedTables.mediaYear_X_Cumul
//                && webSession.PreformatedTable != TblFormatCst.PreformatedTables.mediaYear_X_Mensual)
//                html.Append("<td>" + GestionWeb.GetWebWord(1164, webSession.SiteLanguage) + "</td>");
//            else
//                html.Append("<td>" + GestionWeb.GetWebWord(1357, webSession.SiteLanguage) + "</td>");
//            j = FIRST_DATA_INDEX;
//            if (webSession.PreformatedTable.ToString().EndsWith("Mensual")) {
//                html.Append("<td>TOTAL</td>");
//                j++;
//            }
//            for (i = j; i < data.GetLength(1) - PERSO_COLUMN; i++) {
//                html.Append("<td>" + WebFunctions.Dates.GetPeriodBeginningDate((int.Parse(webSession.PeriodBeginningDate) + i - j).ToString(), webSession.PeriodType).ToString("MMMM") + "</td>");
//            }
//            html.Append("</tr>");

//            #endregion

//            #region Corps du tableau
//            //ligne totale
//            lineCssClass = L0_CSS;
//            for (i = 1; i <= NB_YEAR + NB_OPTION; i++) {
//                html.Append("<tr class=" + lineCssClass + "><td");
//                if (i < 1) {
//                    html.Append(" align=left");
//                }
//                html.Append(" nowrap>" + data[i, 0] + "</td>");
//                for (j = FIRST_DATA_INDEX; j < data.GetLength(1) - PERSO_COLUMN; j++) {
//                    if (data[i, j] != null) {
//                        NumericDataAppend(webSession, html, double.Parse(data[i, j].ToString()),
//                            IsMultiYearLine(i, 0, NB_YEAR, NB_OPTION), data[i, 0].ToString(), excel, false);
//                    }
//                    else {
//                        html.Append("<td nowrap></td>");
//                    }
//                }
//                html.Append("</tr>");
//                lineCssClass = LIGHT_L0_CSS;
//            }
//            //lignes suivantes
//            string align = "";
//            int labelIndex = 0;
//            StringBuilder output = html;
//            StringBuilder bufferHtml = new StringBuilder(5000);
//            bool newLine = false;
//            for (i = 1 + NB_YEAR + NB_OPTION; i < data.GetLength(0); i++) {
//                newLine = false;
//                for (j = 0; j < data.GetLength(1) - PERSO_COLUMN; j++) {

//                    if (j == L1_DATA_INDEX && data[i, j] != null) {
//                        if (PERSO_COLUMN < 1)
//                            display = true;
//                        else if (webSession.PersonalizedElementsOnly
//                            && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none
//                            && webSession.PreformatedProductDetail.ToString().StartsWith(TblFormatCst.PreformatedProductDetails.advertiser.ToString()))
//                            display = false;
//                        else {
//                            if (IsMultiYearLine(i, 0, NB_YEAR, NB_OPTION))
//                                bufferHtml.Length = 0;
//                            output = bufferHtml;
//                            display = true;
//                        }

//                        if (display) {
//                            labelIndex = j;
//                            if (!IsMultiYearLine(i, 0, NB_YEAR, NB_OPTION)) {
//                                totalLine = false;
//                                align = "";
//                                lineCssClass = LIGHT_L1_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = LIGHT_L1_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = LIGHT_L1_REF_CSS;
//                                }
//                            }
//                            else {
//                                totalLine = true;
//                                align = " align=left ";
//                                lineCssClass = L1_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = L1_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = L1_REF_CSS;
//                                }
//                            }
//                            newLine = true;
//                            output.Append("<tr class=" + (lineCssClass) + "><td" + align + " nowrap>" + data[i, j] + "</td>");
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else if (j == L2_DATA_INDEX) {
//                        if (PERSO_COLUMN < 1)
//                            display = true;
//                        else if (webSession.PersonalizedElementsOnly && (WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.none)
//                            display = false;
//                        else
//                            display = true;
//                        if (display && data[i, j] != null) {
//                            html.Append(bufferHtml.ToString());
//                            bufferHtml.Length = 0;
//                            output = html;
//                            labelIndex = j;
//                            if (!IsMultiYearLine(i, 0, NB_YEAR, NB_OPTION)) {
//                                totalLine = false;
//                                align = "";
//                                lineCssClass = LIGHT_L2_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = LIGHT_L2_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = LIGHT_L2_REF_CSS;
//                                }
//                            }
//                            else {
//                                totalLine = true;
//                                align = " align=left ";
//                                lineCssClass = L2_CSS;
//                                if (PERSO_COLUMN > 0) {
//                                    if ((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == WebCst.AdvertiserPersonalisation.Type.competitor)
//                                        lineCssClass = L2_COMPET_CSS;
//                                    else if (((WebCst.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == WebCst.AdvertiserPersonalisation.Type.reference)
//                                        lineCssClass = L2_REF_CSS;
//                                }
//                            }
//                            newLine = true;
//                            output.Append("<tr class=" + (lineCssClass) + "><td" + align + " nowrap>" + data[i, j] + "</td>");
//                            j = FIRST_DATA_INDEX - 1;
//                        }
//                    }
//                    else {
//                        if (display) {
//                            if (data[i, j] != null) {
//                                NumericDataAppend(webSession, output, double.Parse(data[i, j].ToString()),
//                                    IsMultiYearLine(i, 0, NB_YEAR, NB_OPTION), data[i, labelIndex].ToString(), excel, totalLine);
//                            }
//                            else if (j > L2_DATA_INDEX)
//                                output.Append("<td></td>"); //<td>- </td>
//                        }

//                    }
//                }
//                if (newLine)
//                    output.Append("</tr>");
//            }
//            #endregion

//            #region fermeture des balise du tableau
//            html.Append("</table>");
//            #endregion

//        }
//        #endregion

//        #region Pas de données
//        /// <summary>
//        /// Pas de données
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML</returns>
//        private static string NoData(WebSession webSession) {
//            return "<div align=\"center\" class=\"txtViolet11Bold\"><br><br>" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "<br><br><br></div>";
//        }
//        #endregion

//        #region Fréquence de livraison des données invalide
//        /// <summary>
//        /// Fréquence de livraison des données invalide
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML</returns>
//        private static string UnvalidFrequencyDelivery(WebSession webSession) {
//            return "<div align=\"center\" class=\"txtViolet11Bold\"><br><br>" + GestionWeb.GetWebWord(1234, webSession.SiteLanguage) + "<br><br><br></div>";
//        }
//        #endregion



//        #region Sortie Excel
//        /// <summary>
//        /// Sortie Excel
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code Source pour Excel</returns>
//        public static string GetDynamicTableExcelUI(WebSession webSession) {
//            System.Text.StringBuilder t = new System.Text.StringBuilder(6000000);
//            try {

//                #region Rappel des paramètres
//                t.Append(ExcelFunction.GetLogo(webSession));
//                t.Append(ExcelFunction.GetExcelHeader(webSession, true, true, false, GestionWeb.GetWebWord(1055, webSession.SiteLanguage)));
//                #endregion

//                #region Résultat
//                t.Append(TNS.FrameWork.Convertion.ToHtmlString(GetDynamicTableUI(webSession, true)));
//                #endregion

//            }
//            catch (System.Exception err) {
//                throw (new WebExceptions.DynamicTablesUIException("Impossible de générer le résultat", err));
//            }
//            t.Append(ExcelFunction.GetFooter(webSession));
//            return t.ToString();
//        }
//        #endregion

//        #endregion

//        #region Méthodes privées

//        #region Afficher une valeur numérique
//        /// <summary>
//        /// Afficher une valeur numérique
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="html">Code Html à remplire</param>
//        /// <param name="data">Données</param>
//        /// <param name="multiYearLine">True si année multiple</param>
//        /// <param name="lineLabel">Ligne du libellé</param>
//        /// <param name="excel">True si Excel</param>
//        /// <param name="totalLine">Ligne Total</param>
//        private static void NumericDataAppend(WebSession webSession, StringBuilder html, double data, bool multiYearLine, string lineLabel, bool excel, bool totalLine) {

//            if (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(1168, webSession.SiteLanguage))) {
//                if (!excel) {
//                    //Evolution
//                    if (data > 0) //hausse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/g.gif></td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/g.gif></td>");
//                    else if (data < 0) //baisse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/r.gif></td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/r.gif></td>");
//                    else if (!Double.IsNaN(data)) // 0 exactement
//                        html.Append("<td nowrap>0 %<img src=/I/o.gif></td>");
//                    else
//                        html.Append("<td nowrap></td>"); //<td nowrap>- </td>
//                }
//                else if (excel) {
//                    //Evolution
//                    if (data > 0) //hausse
//                        //html.Append("<td nowrap> " + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "</td>");
//                        html.Append("<td nowrap> " + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                    else if (data < 0) //baisse
//                        //html.Append("<td nowrap> " + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "</td>");
//                        html.Append("<td nowrap> " + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                    else if (!Double.IsNaN(data)) // 0 exactement
//                        html.Append("<td nowrap> 0 %</td>");
//                    else
//                        html.Append("<td nowrap></td>"); //<td nowrap>- </td>
//                }
//            }
//            else if (data != 0 &&
//                (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(1166, webSession.SiteLanguage)))
//                || (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(806, webSession.SiteLanguage)))) {
//                //PDV ou PDM
//                //html.Append("<td nowrap>" + ( (data==0||Double.IsNaN(data)||Double.IsInfinity(data)) ? "- " : data.ToString("# ### ##0.##")+" %" ) + "</td>");
//                html.Append("<td nowrap>" + ((data == 0 || Double.IsNaN(data) || Double.IsInfinity(data)) ? "" : WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %") + "</td>");
//            }
//            else {
//                //!PDV et !PDM et !evol
//                if (!totalLine) {
//                    //html.Append("<td nowrap>" + ( (!double.IsNaN(data) && data!=0) ? data.ToString("### ### ### ### ##0") : "- " ) + "</td>");
//                    html.Append("<td nowrap>" + ((!double.IsNaN(data) && data != 0) ? WebFunctions.Units.ConvertUnitValueToString(data.ToString(), webSession.Unit) : "") + "</td>");
//                }
//                else {
//                    html.Append("<td nowrap></td>");
//                }
//            }
//        }
//        #endregion

//        #region Afficher une valeur numérique
//        /// <summary>
//        /// Afficher une valeur numérique
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="html">Html à remplire</param>
//        /// <param name="data">Données</param>
//        /// <param name="evolution">True si evolution</param>
//        /// <param name="percentage">True si pourcentage</param>
//        /// <param name="dataLineUpperbound">?Bonne Question?</param>
//        /// <param name="currenCol">Colonne courrante</param>
//        /// <param name="first_data_column">Première colonne</param>
//        /// <param name="excel">True si EXcel, False sinon</param>
//        private static void NumericDataAppend(WebSession webSession, StringBuilder html, double data, bool evolution, bool percentage, int dataLineUpperbound, int currenCol, int first_data_column, bool excel) {

//            if (evolution && currenCol == dataLineUpperbound) {
//                if (!excel) {
//                    //Evolution
//                    if (data > 0) //hausse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/g.gif></td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/g.gif></td>");
//                    else if (data < 0) //baisse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/r.gif></td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "<img src=/I/r.gif></td>");
//                    else if (!Double.IsNaN(data)) // 0 exactement
//                        html.Append("<td nowrap> 0 %<img src=/I/o.gif></td>");
//                    else
//                        html.Append("<td></td>"); //<td>- </td>
//                }
//                else if (excel) {
//                    //Evolution
//                    if (data > 0) //hausse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "</td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                    else if (data < 0) //baisse
//                        //html.Append("<td nowrap>" + ( (!Double.IsInfinity(data)) ? data.ToString("# ### ##0.##")+" %" : "" ) + "</td>");
//                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %" : "") + "</td>");
//                    else if (!Double.IsNaN(data)) // 0 exactement
//                        html.Append("<td nowrap> 0 %</td>");
//                    else
//                        html.Append("<td></td>"); //<td>- </td>
//                }
//            }
//            else if (data != 0 && percentage && ((currenCol - first_data_column) % 2) == 1) {
//                //PDV ou PDM
//                //html.Append("<td nowrap>" + ( (data==0||Double.IsNaN(data)||Double.IsInfinity(data)) ? "- " : data.ToString("# ### ##0.##")+" %" ) + "</td>");
//                html.Append("<td nowrap>" + ((data == 0 || Double.IsNaN(data) || Double.IsInfinity(data)) ? "" : WebFunctions.Units.ConvertUnitValueAndPdmToString(data.ToString(), webSession.Unit, true) + " %") + "</td>");
//            }
//            else {
//                //!PDV et !PDM et !evol
//                //html.Append("<td nowrap>" + ( (!double.IsNaN(data) && data!=0) ? data.ToString("### ### ### ### ##0") : "- " ) + "</td>");
//                html.Append("<td nowrap>" + ((!double.IsNaN(data) && data != 0) ? WebFunctions.Units.ConvertUnitValueToString(data.ToString(), webSession.Unit) : "") + "</td>");

//            }

//        }
//        #endregion

//        #region Test si une ligne est une ligne multiAnnée ou non
//        /// <summary>
//        /// Test si une ligne est une ligne multiAnnée ou non
//        /// </summary>
//        /// <param name="line">Ligne</param>
//        /// <param name="first_data_line">Première ligne</param>
//        /// <param name="nb_year">Nombre d'année</param>
//        /// <param name="nb_option">Numéro de l'option</param>
//        /// <returns>True si une ligne est une ligne multiAnnée, False sinon</returns>
//        private static bool IsMultiYearLine(int line, int first_data_line, int nb_year, int nb_option) {
//            return (line - first_data_line) % (1 + nb_year + nb_option) == 0;
//        }
//        #endregion

//        #endregion

//    }
//}
