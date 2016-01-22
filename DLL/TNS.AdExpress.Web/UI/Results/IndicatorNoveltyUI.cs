//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de création: 20/09/2004  
//// Date de modification: 20/10/2004 
////		10/05/2005	K.Shehzad	Chagement d'en tête Excel
////		12/08/2005	G. Facon	Nom de fonction
////	25/10/2005	D. V. Mussuma	Intégration unité Keuros	

//#endregion

//using System.Web;

//using System;
//using System.Collections;
//using System.Web.UI;
//using TNS.AdExpress.Constantes.FrameWork.Results;
//using CstWeb = TNS.AdExpress.Constantes.Web;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using TNS.FrameWork.Date;
//using WebRules=TNS.AdExpress.Web.Rules.Results;
//using DateFunctions = TNS.FrameWork.Date;
//using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Exceptions;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using TNS.FrameWork;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// Interface Utilisateur des Nouveautés (produits ou annonceurs) dans analyse sectorielle
//    /// Cette classe génère suivant le format de sortie le code pour afficher un tableau	
//    /// </summary>
//    public class IndicatorNoveltyUI{
		
//        #region Sortie HTML
//        /// <summary>
//        /// Crée le code HTML pour l'affichage du tableau de résultats qui permettra de détecter les réels nouveaux produits
//        /// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.
//        /// </summary>		
//        /// <param name="webSession">Session du client</param>
//        /// <param name="page">Page qui affiche les nouveautés</param>
//        /// <param name="tab">tableau des résultats</param>
//        /// <param name="elementType">annonceur ou référence</param>
//        /// <param name="excel">sortie Html ou excel</param>
//        /// <returns>Code HTML</returns>
//        //public static string getIndicatorSeasonalityHtmlUI(Page page,object[,] tab,WebSession webSession){
//        public static string GetIndicatorNoveltyHtmlUI(Page page,object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType,bool excel){	
		
//            #region variables 
//            string classe="";
//            string classe2="";
//            string AdvertiserAccessList="";		
//            string CompetitorAdvertiserAccessList="";			
//            ArrayList AdvertiserAccessListArr=null;
//            ArrayList CompetitorAdvertiserAccessListArr=null;
//            string IdElementToPersonnalize="";
//            string pluszero="";
//            DateTime currentMonthDate;
//            string currentMonth="";
//            //bool PreviousYearActiveMonth=false;			
//            #endregion			 

//            #region Constantes
//            const string competitorExcelStyle="p142";
//            const string competitorExcelStyleNB="p143";
			
//            #endregion

//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0 ){
//                if(elementType==ConstResults.Novelty.ElementType.product){
//                    return GestionWeb.GetWebWord(1238,webSession.SiteLanguage);
//                }
//                else{
//                    return GestionWeb.GetWebWord(1239,webSession.SiteLanguage);
//                }
//            }
//            #endregion

//            #region identifiant annonceurs référence et concurrents 
//            AdvertiserAccessList = webSession.GetSelection(webSession.ReferenceUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);		
//            if(webSession.CompetitorUniversAdvertiser[0]!=null){
//                CompetitorAdvertiserAccessList = webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],CustomerRightConstante.type.advertiserAccess);		
//            }
//            #region recuperation éléments références et concurrents			
//            if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){				
//                AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
//            }
//            if(WebFunctions.CheckedText.IsStringEmpty(CompetitorAdvertiserAccessList)){
//                CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
//            }			
//            #endregion
//            #endregion

//            //fin période étudiée
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            //Mois actif
//            currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//            //Date mois actif
//            currentMonthDate = WebFunctions.Dates.GetDateFromAlias(currentMonth);

//            if(currentMonthDate.Month<10)pluszero="0";
//            else pluszero="";

//            #region construction du tableau de nouveautés
//            System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
//            //Debut tableau
//            t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");			
//            //Debut tableau nouveautés			
			
//            #region ligne libellés
//            //Ligne Libellés tableau

//            //Colonne libéllés produits ou annonceurs
//            if(ConstResults.Novelty.ElementType.advertiser==elementType)
//            t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
//            else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
//            //Colonne investissement dernier mois actifs année N-1 (kE)
//            if(webSession.ComparativeStudy) {
//                t.Append("<td nowrap class=\"p2\" >"+GestionWeb.GetWebWord(1218,webSession.SiteLanguage)+"</td>");
//                //Colonne libéllés dernier mois actifs année N-1 (kE)
//                t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1219,webSession.SiteLanguage)+"</td>");
//                //Période d'inactivité
//                t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");
//            }
//            //Colonne mois en cours (KE)
//            t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1221,webSession.SiteLanguage)+pluszero+currentMonthDate.Month+"-"+currentMonthDate.Year+"</td>");
//            //Colonne SOV			
//            t.Append("<td  nowrap  class=\"p2\" colspan=2>"+GestionWeb.GetWebWord(437,webSession.SiteLanguage)+"</td>");				
//            t.Append("\n</tr>");
//            #endregion
			
//            #region lignes tableaux
//                classe2="p7";
//                classe="p9";																
//            for(int i=0;i<tab.GetLength(0);i++){

//                #region personnalisation des éléments de références et concurrents
//                if(tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]!=null) {
//                 IdElementToPersonnalize = tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX].ToString();
//                    if(AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(IdElementToPersonnalize)){
//                        classe2="p15"; 
//                        classe="p151";
						
//                    }
//                    else if(CompetitorAdvertiserAccessListArr !=null && CompetitorAdvertiserAccessListArr.Contains(IdElementToPersonnalize)){
//                        classe2="p14"; 
//                        classe="p141";	
//                        if(excel){
//                            classe2=competitorExcelStyle; 
//                            classe=competitorExcelStyleNB;
//                        }

//                    }else {
//                        classe2="p7";
//                        classe="p9";	
//                    }
//                }
//                #endregion

//                t.Append("<tr>");
//                //Colonne libéllé produit ou annonceur 
//                if(tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe2+">"+tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX].ToString()+"</td>");
//                else t.Append("<td nowrap  class="+classe2+">&nbsp;</td>");
//                //Si etude comparative
//                if(webSession.ComparativeStudy) {
//                    //Colonne investissement dernier mois actifs année N-1 (kE)
//                    if(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]!=null && !tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString().Equals(""))
////						t.Append("<td nowrap  class="+classe+">"+double.Parse(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
//                        t.Append("<td nowrap  class="+classe+">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
//                    else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
//                    //Colonne libéllés dernier mois actifs année N-1 (kE)
//                    if(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe2+">"+Convertion.ToHtmlString(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString())+"</td>");
//                    else t.Append("<td nowrap  class="+classe2+">&nbsp;</td>");
//                    //Colonne Période d'inactivité (nbre de mois)
//                    if(tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe+">"+tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX].ToString()+"</td>");
//                    else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");				
//                }
//                //Colonne mois en cours (KE)
//                if(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]!=null && !tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString().Equals(""))
////					t.Append("<td nowrap  class="+classe+">"+double.Parse(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
//                    t.Append("<td nowrap  class="+classe+">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
//                else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
//                //SOV
//                if(tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX]!=null && !tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX].ToString().Equals("-"))t.Append("<td nowrap  class="+classe+">"+double.Parse(tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.00")+ConstResults.Novelty.PERCENT_SYMBOL+"</td>");
//                else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
//                t.Append("</tr>");
//            }

//            //Fin tableau nouveautés
//            //t.Append("\n</TABLE>");	
//            //Fin tableau
//            t.Append("\n</TD></TR></TABLE>");
//            #endregion
//            return(t.ToString());
//            #endregion
//        }
//        /// <summary>
//        /// Crée le code HTML pour l'affichage du tableau (version graphique) de résultats qui permettra de détecter les réels nouveaux produits
//        /// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.
//        /// </summary>	
//        /// <param name="tab">tableau de résultats </param>
//        /// <param name="webSession">session client</param>
//        /// <param name="elementType">élément annonceur ou référence</param>
//        /// <returns>Code</returns>
//        //public static string GetIndicatorNoveltyGraphicHtmlUI(Page page,object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType){
//        public static string GetIndicatorNoveltyGraphicHtmlUI(object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType){
	
//            #region variables
//            string currentMonth="";
//            bool PreviousYearActiveMonth=false;
//            DateTime currentMonthDate;			
//            string classe="";
//            string classe2="";
//            string classe3="";
//            string pluszero="";
//            string AdvertiserAccessList="";
//            string[] AdvertiserSplitter={","};
//            string CompetitorAdvertiserAccessList="";			
//            ArrayList AdvertiserAccessListArr=null;
//            ArrayList CompetitorAdvertiserAccessListArr=null;
//            string IdElementToPersonnalize="";
//            #endregion

//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0){
//                if(elementType==ConstResults.Novelty.ElementType.product){
//                    return GestionWeb.GetWebWord(1238,webSession.SiteLanguage);
//                }
//                else{
//                    return GestionWeb.GetWebWord(1239,webSession.SiteLanguage);
//                }
//            }
//            #endregion
			
//            if(webSession.ComparativeStudy) {
//                #region identifiant annonceurs référence et concurrents 
//                AdvertiserAccessList = webSession.GetSelection(webSession.ReferenceUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);		
//                if(webSession.CompetitorUniversAdvertiser[0]!=null){
//                    CompetitorAdvertiserAccessList = webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],CustomerRightConstante.type.advertiserAccess);		
//                }
//                #region recuperation éléments références et concurrents			
//                if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){				
//                    AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
//                }
//                if(WebFunctions.CheckedText.IsStringEmpty(CompetitorAdvertiserAccessList)){
//                    CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
//                }			
//                #endregion

//                #endregion

//                #region construction du tableau de nouveautés
//                System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
//                //Debut tableau
//                t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

//                //fin période étudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();
//                DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//                //Mois actif
//                currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//                //Date mois actif
//                currentMonthDate = WebFunctions.Dates.GetDateFromAlias(currentMonth);

//                #region ligne libellés
//                classe2="p7";
//                classe="p9";
//                classe3="p9ind";
//                //Ligne Libellés tableau				
//                    //Colonne libéllés produits ou annonceurs
//                    if(ConstResults.Novelty.ElementType.advertiser==elementType)
//                        t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
//                    else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
//                    //Période d'inactivité
//                    t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");			
//                    //Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

//                    //Cellules mois  de l'année N-1
//                    for (int j=1;j<=12;j++){
//                        if(j<10)pluszero="0";
//                        else pluszero="";
//                        t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+j+"-"+PeriodEndDate.AddYears(-1).Year+"</td>");
//                    }
//                    //Colonne separation année N/N-1
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

//                    //cellules mois de la période N
//                    for(int m=1;m<currentMonthDate.Month;m++){
//                        if(m<10)pluszero="0";
//                        else pluszero="";
//                        t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+m+"-"+PeriodEndDate.Year+"</td>");
	
//                    }
//                    //Colonne separation miis actif année N / et mois inactif année N
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//                    //Colonne mois en cours (KE)
//                    if(currentMonthDate.Month<10)pluszero="0";
//                    else pluszero="";
//                    t.Append("<td  nowrap align=center class=\"p2\">"+GestionWeb.GetWebWord(1221,webSession.SiteLanguage)+"<br>"+pluszero+currentMonthDate.Month+"-"+currentMonthDate.Year+"</td>");
//                    t.Append("\n</tr>");
			
//                #endregion

//                #region lignes produits ou annonceurs
				
//                for(int i=0;i<tab.GetLength(0);i++){
//                    #region personnalisation des éléments de références et concurrents
//                    if(tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]!=null) {
//                        IdElementToPersonnalize = tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX].ToString();
//                        if(AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(IdElementToPersonnalize)){
//                            classe2="p15"; 
//                            classe="p151";
//                        }
//                        else if(CompetitorAdvertiserAccessListArr !=null && CompetitorAdvertiserAccessListArr.Contains(IdElementToPersonnalize)){							
//                            classe2="p14"; 
//                            classe="p141";	
//                        }else{						
//                            classe2="p7";
//                            classe="p9";
//                        }
//                    }
//                    #endregion
//                    t.Append("<tr>");
//                    //Colonne libéllé produit ou annonceur 
//                    if(tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe2+">"+tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX].ToString()+"</td>");
//                    else t.Append("<td nowrap  class="+classe2+">&nbsp;</td>");
//                    //Colonne Période d'inactivité (nbre de mois)
//                    if(tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe+">"+tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX].ToString()+"</td>");
//                    else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
//                    //Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

//                    //Cellules mois  de l'année N-1
//                    for (int k=1;k<=12;k++){
//                        //Colonne libéllés dernier mois actifs année N-1 (kE)
//                        if(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]!=null && !tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals("")){
//                            //PreviousYearActiveMonth = tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString();
//                            if(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals(k.ToString())){							
////								t.Append("<td nowrap  class=pmcategorynb>"+double.Parse(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
//                                t.Append("<td nowrap  class=pmcategorynb>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
//                                PreviousYearActiveMonth=true;
//                            }
//                            else if(PreviousYearActiveMonth) t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
//                            else t.Append("<td nowrap  >&nbsp;</td>");
//                        }
//                        else t.Append("<td nowrap >&nbsp;</td>");
//                    }
//                    //Colonne separation année N/N-1
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//                    //cellules mois de la période N
//                    for(int l=1;l<currentMonthDate.Month;l++){
//                        if(PreviousYearActiveMonth)t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
//                        else t.Append("<td nowrap >&nbsp;</td>");
//                    }
//                    PreviousYearActiveMonth=false;
//                    //Colonne separation mois actif année N / et mois inactif année N
//                    t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

//                    //Colonne mois en cours (KE)
//                    if(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]!=null && !tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString().Equals("-"))
////						t.Append("<td nowrap  class=\"pmcategorynb\">"+double.Parse(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
//                        t.Append("<td nowrap  class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
//                    else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
//                    t.Append("\n</tr>");
//                }
//                #endregion

				
			
//                //Fin tableau
//                t.Append("\n</TD></TR></TABLE>");
//                #region tableau légende
//                t.Append("<table><tr><td nowrap class=pmcategorynb width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1225,webSession.SiteLanguage)+"</td></tr>");
//                t.Append("<tr><td nowrap class="+classe3+" width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1226,webSession.SiteLanguage)+"</td></tr></table>");
//                #endregion
//                #endregion
//                return(t.ToString());
//            }
//            else {
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1224,webSession.SiteLanguage)				
//                    +"</div>");

//            }			
//        }
//        #endregion

//        #region Sortie Excel
//        /// <summary>
//        /// Sortie Excel
//        /// </summary>
//        /// <param name="page">page</param>
//        /// <param name="tab">tab</param>
//        /// <param name="webSession">webSession</param>
//        /// <param name="elementType">Type d'éléments (produit, annonceur)</param>
//        /// <returns>Code</returns>
//        public static string GetIndicatorNoveltyExcelUI(Page page,object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType){
		
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

//            #region Rappel des paramètres
//            // Paramètres du tableau
			
//            //t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,GestionWeb.GetWebWord(1310,webSession.SiteLanguage)));


//            #endregion
				
//            t.Append(GetIndicatorNoveltyHtmlUI(page,tab,webSession,elementType,true));
//            return Convertion.ToHtmlString(t.ToString());

		
//        }

//        #endregion
//    }
//}
