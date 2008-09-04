//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de création: 20/10/2004 
//// Date de modification: 20/10/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions
////	25/10/2005	D. V. Mussuma	Intégration unité Keuros	
	
//#endregion

//#region Namespace
//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;
//using System.Globalization;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using TNS.FrameWork.Date;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using TNS.AdExpress.Domain.Translation;
//using DateFunctions = TNS.FrameWork.Date;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using TNS.AdExpress.Web.Exceptions;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
//using TNS.AdExpress.Domain.Web;
//#endregion

//namespace TNS.AdExpress.Web.Rules.Results
//{
//    /// <summary>
//    /// Classe métier de traitement des données issues de la base pour 
//    /// les Nouveautés (analyse sectorielle).
//    /// </summary>
//    public class IndicatorNoveltyRules
//    {
//        /// <summary>
//        /// Crée le tableau de résultats qui permettra de détecter les réels nouveaux produits
//        /// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="elementType">référence  ou annonceur</param>
//        /// <returns>Tableau de nouveaux produits ou annonceurs</returns>
//        public static object[,] GetFormattedTable(WebSession webSession,ConstResults.Novelty.ElementType elementType){
//            #region Variables 
//            //indexe ligne tableau
//            int i=0;
//            //groupes de données nouveaux produits ou annonceurs
//            DataSet dsNovelty=null;
//            //table de données nouveaux produits ou annonceurs
//            DataTable dtNovelty=null;
//            //Groupe de données pour total univers
//            DataSet dsTotalUnivers = null;
//            //Table de données pour total univers
//            DataTable dtTotalUnivers = null;	
//            //nombre de mois d'inactivité 	
//            int nbMonthInactivityPeriod=0;
//            //tableau de résultats
//            object[,] tab = null;
//            //Dernier mois actif
//            string LatestActiveMonths="";
//            //Derniers mois actifs
//            string[] LatestActiveMonthsArr=null;
//            //Y a t'il un mois actif
//            bool activeMonthFounded=false;
//            //Investissment total pour le mois courant
//            string TotalcurrentMonthInvest="";
//            //mois courant
//            string currentMonth="";
//            //double TotalUnivers=(double)0.0;
//            Int64 Total = 0;
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            #endregion
			
//            #region periode etudiée
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region chargement des données
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.universTotal ){				
//                    //Données univers
//                    if(ConstResults.Novelty.ElementType.advertiser==elementType)
//                    dsTotalUnivers = IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,false);	
//                    else dsTotalUnivers = IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,false);	
//                    if( dsTotalUnivers!=null && dsTotalUnivers.Tables[0].Rows.Count > 0){
//                        dtTotalUnivers = dsTotalUnivers.Tables[0];
//                    }	
//            }
//            //Groupes de données contenant la liste des annonceurs ou des références,les investissements mensuels de la période précédente,
//            //l'investissement du mois courant.
//            if(ConstResults.Novelty.ElementType.product == elementType)
//            dsNovelty=IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,true);			
//            else if(ConstResults.Novelty.ElementType.advertiser == elementType) dsNovelty=IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,true);				
//            #endregion

//            #region construction du tableau de résultats
//            if(dsNovelty!=null && dsNovelty.Tables[0]!=null && dsNovelty.Tables[0].Rows.Count >0){
//            dtNovelty = dsNovelty.Tables[0];
//            //création et initialisation du tableau du tableau 
//                tab = new object[dsNovelty.Tables[0].Rows.Count,ConstResults.Novelty.MAX_COLUMN_LENGTH];
//                foreach(DataRow currentRow in dtNovelty.Rows){
//                    //données annonceur(s) ou références
//                    if(ConstResults.Novelty.ElementType.advertiser==elementType || ConstResults.Novelty.ElementType.product==elementType){
//                        //colonnes Libellés et ID références
//                        if(ConstResults.Novelty.ElementType.product==elementType){
//                            tab[i,ConstResults.Novelty.ID_ELEMENT_COLUMN_INDEX]=currentRow["id_product"].ToString();
//                            tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]=currentRow["product"].ToString();							
//                        }
//                        //colonnes Libellés et ID annonceurs
//                        if(ConstResults.Novelty.ElementType.advertiser==elementType){
//                            tab[i,ConstResults.Novelty.ID_ELEMENT_COLUMN_INDEX]=currentRow["id_advertiser"].ToString();
//                            tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]=currentRow["advertiser"].ToString();
//                        }	
//                        //colonne ID annonceur utiliser pour personnaliser les éléments de références ou concurrents
//                        tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]=currentRow["id_advertiser"].ToString();
//                        #region colonnes dernier mois actif année N-1
//                        //Dernier mois actif année précédente
//                        try{
//                            /*Determine le dernier mois actif de la période précédente ( si étude comparative).
//                             * Pour cela,  on parcours  la liste des mois en partant du dernier, et on recupère celui dont l'invetissement
//                             * est supérieur à zéro.
//                             * */
//                            //Liste des mois de la période précédente
//                            LatestActiveMonths = IndicatorDataAccess.StringMonthsComparativeStudy(webSession);
//                            if(WebFunctions.CheckedText.IsStringEmpty(LatestActiveMonths) && webSession.ComparativeStudy){								
//                                LatestActiveMonthsArr=LatestActiveMonths.Split(',');
//                                if(LatestActiveMonthsArr!=null && LatestActiveMonthsArr.Length>0){
//                                    for(int j=LatestActiveMonthsArr.Length-1;j>=0;j--){
//                                        //on parcours  la liste des mois en partant du dernier, et on recupère celui dont l'invetissement est supérieur à zéro
//                                        if( double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString()) >(double)0.0){
////											tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]=double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString())/(double)1000.0;
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]=double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString());
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]= WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month.ToString();
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]=DateFunctions.MonthString.GetCharacters(WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month,cultureInfo,0)+" "
//                                            +WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Year;
//                                            //Periode d'inactivité
//                                            try{
//                                                //mois courant
//                                                currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//                                                //nombre mois d'inactivité  = (Mois actif - 1) +(12 - Dernier mois actif année N-1)												
//                                                nbMonthInactivityPeriod = (WebFunctions.Dates.GetDateFromAlias(currentMonth).Month -1) + ( 12 - WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month );
//                                                if(nbMonthInactivityPeriod > 0)
//                                                    tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= nbMonthInactivityPeriod.ToString();
//                                                else{
//                                                    tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= "";  
//                                                }
//                                                activeMonthFounded=true;
//                                            }catch(Exception per){
//                                                throw( new Exceptions.IndicateurNoveltyRulesException("Impossible de déterminer la période d'inactivité : ",per));
//                                            }
//                                            break;
//                                        }										
//                                    }
//                                    if(!activeMonthFounded){
//                                        tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]="";
//                                        tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]="";
//                                        tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= "";
//                                        tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]="";
//                                    }
//                                    activeMonthFounded=false;
//                                }
//                                else{
//                                    tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]="";
//                                    tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]="";
//                                    tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= "";
//                                    tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]="";
//                                }								
//                            }else{
//                                tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]="";
//                                tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]="";
//                                tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= "";
//                                tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]="";
//                            }
//                        }catch(Exception actEr){
//                            throw (new Exceptions.IndicateurNoveltyRulesException("Imposible de déterminer le Dernier mois actif pour l'année N-1 :",actEr) );
//                        }
//                        #endregion

//                        #region Mois actif année N
//                        //Investissement Mois actif année N
//                        try{
//                            //Dernier mois actif année N
//                            currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//                            if(WebFunctions.CheckedText.IsStringEmpty(currentMonth) && double.Parse(currentRow[currentMonth.Trim()].ToString()) >(double)0.0){								
////								tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]= double.Parse(currentRow[currentMonth.Trim()].ToString()) /(double)1000.0;		
//                                tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]= double.Parse(currentRow[currentMonth.Trim()].ToString());
								
//                            }else{
//                                tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]="";
//                            }
//                        }catch(Exception investErr){
//                            throw (new Exceptions.IndicateurNoveltyRulesException(GestionWeb.GetWebWord(1360,webSession.SiteLanguage )+ " : ",investErr));
//                        }
//                        #region SOV
//                        //SOV en fonction du total univers ou marché ou famille pour mois actif année N.
//                        //Le SOV exprime, en %, investissement du nouveau Produit sur le mois en cours / Investissement du Total (Marché, famille, univers) sur le mois en cours.
//                        switch(webSession.ComparaisonCriterion){
//                            case WebConstantes.CustomerSessions.ComparisonCriterion.universTotal :
//                                //SOV sur total univers
//                                if(WebFunctions.CheckedText.IsStringEmpty(currentMonth)){
//                                    if(dtTotalUnivers!=null && dtTotalUnivers.Columns.Contains(currentMonth)){									
//                                        TotalcurrentMonthInvest = dtTotalUnivers.Compute("sum("+currentMonth+")","").ToString();
//                                        if(WebFunctions.CheckedText.IsStringEmpty(TotalcurrentMonthInvest))
//                                        tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] = (double.Parse(currentRow[currentMonth.Trim()].ToString())*(double)100.0)/double.Parse(TotalcurrentMonthInvest);
//                                        else tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] ="";
//                                    }
//                                }
//                                else tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] ="";
//                                break;
//                            case WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal :
//                            case WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal :
//                                //SOV sur total famille ou marché
//                                if(WebFunctions.CheckedText.IsStringEmpty(currentMonth)){
//                                    Total = IndicatorDataAccess.getTotalForMonth(webSession,ConstResults.PalmaresRecap.typeYearSelected.currentYear);
//                                    if(Total>(double)0.0){									
//                                        tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] = (double.Parse(currentRow[currentMonth.Trim()].ToString())*(double)100.0)/Total;									
//                                    }
//                                    else tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] ="";
//                                }
//                                else tab[i,ConstResults.Novelty.SOV_COLUMN_INDEX] ="";
//                                break;
//                            default : 
//                                throw (new Exceptions.IndicateurNoveltyRulesException(GestionWeb.GetWebWord(1361,webSession.SiteLanguage)));
//                        }
						
//                        #endregion
						  
//                        #endregion

//                    }						
//                    else {
//                        throw (new Exceptions.IndicateurNoveltyRulesException(GestionWeb.GetWebWord(1362,webSession.SiteLanguage)));
//                    }
//                    i++;
//                }
//            }			
//            #endregion
//            // Il n'y a pas de données
//            if(tab==null)return(new object[0,0]);	
			
//            return tab;
//        }
//    }
//}
