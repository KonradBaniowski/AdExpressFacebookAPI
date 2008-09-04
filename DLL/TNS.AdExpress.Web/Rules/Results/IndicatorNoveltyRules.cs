//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de cr�ation: 20/10/2004 
//// Date de modification: 20/10/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions
////	25/10/2005	D. V. Mussuma	Int�gration unit� Keuros	
	
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
//    /// Classe m�tier de traitement des donn�es issues de la base pour 
//    /// les Nouveaut�s (analyse sectorielle).
//    /// </summary>
//    public class IndicatorNoveltyRules
//    {
//        /// <summary>
//        /// Cr�e le tableau de r�sultats qui permettra de d�tecter les r�els nouveaux produits
//        /// ou annonceurs des d�marrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le d�but de l'ann�.
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="elementType">r�f�rence  ou annonceur</param>
//        /// <returns>Tableau de nouveaux produits ou annonceurs</returns>
//        public static object[,] GetFormattedTable(WebSession webSession,ConstResults.Novelty.ElementType elementType){
//            #region Variables 
//            //indexe ligne tableau
//            int i=0;
//            //groupes de donn�es nouveaux produits ou annonceurs
//            DataSet dsNovelty=null;
//            //table de donn�es nouveaux produits ou annonceurs
//            DataTable dtNovelty=null;
//            //Groupe de donn�es pour total univers
//            DataSet dsTotalUnivers = null;
//            //Table de donn�es pour total univers
//            DataTable dtTotalUnivers = null;	
//            //nombre de mois d'inactivit� 	
//            int nbMonthInactivityPeriod=0;
//            //tableau de r�sultats
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
			
//            #region periode etudi�e
//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region chargement des donn�es
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.universTotal ){				
//                    //Donn�es univers
//                    if(ConstResults.Novelty.ElementType.advertiser==elementType)
//                    dsTotalUnivers = IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,false);	
//                    else dsTotalUnivers = IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,false);	
//                    if( dsTotalUnivers!=null && dsTotalUnivers.Tables[0].Rows.Count > 0){
//                        dtTotalUnivers = dsTotalUnivers.Tables[0];
//                    }	
//            }
//            //Groupes de donn�es contenant la liste des annonceurs ou des r�f�rences,les investissements mensuels de la p�riode pr�c�dente,
//            //l'investissement du mois courant.
//            if(ConstResults.Novelty.ElementType.product == elementType)
//            dsNovelty=IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,true);			
//            else if(ConstResults.Novelty.ElementType.advertiser == elementType) dsNovelty=IndicatorDataAccess.GetNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,true);				
//            #endregion

//            #region construction du tableau de r�sultats
//            if(dsNovelty!=null && dsNovelty.Tables[0]!=null && dsNovelty.Tables[0].Rows.Count >0){
//            dtNovelty = dsNovelty.Tables[0];
//            //cr�ation et initialisation du tableau du tableau 
//                tab = new object[dsNovelty.Tables[0].Rows.Count,ConstResults.Novelty.MAX_COLUMN_LENGTH];
//                foreach(DataRow currentRow in dtNovelty.Rows){
//                    //donn�es annonceur(s) ou r�f�rences
//                    if(ConstResults.Novelty.ElementType.advertiser==elementType || ConstResults.Novelty.ElementType.product==elementType){
//                        //colonnes Libell�s et ID r�f�rences
//                        if(ConstResults.Novelty.ElementType.product==elementType){
//                            tab[i,ConstResults.Novelty.ID_ELEMENT_COLUMN_INDEX]=currentRow["id_product"].ToString();
//                            tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]=currentRow["product"].ToString();							
//                        }
//                        //colonnes Libell�s et ID annonceurs
//                        if(ConstResults.Novelty.ElementType.advertiser==elementType){
//                            tab[i,ConstResults.Novelty.ID_ELEMENT_COLUMN_INDEX]=currentRow["id_advertiser"].ToString();
//                            tab[i,ConstResults.Novelty.ELEMENT_COLUMN_INDEX]=currentRow["advertiser"].ToString();
//                        }	
//                        //colonne ID annonceur utiliser pour personnaliser les �l�ments de r�f�rences ou concurrents
//                        tab[i,ConstResults.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]=currentRow["id_advertiser"].ToString();
//                        #region colonnes dernier mois actif ann�e N-1
//                        //Dernier mois actif ann�e pr�c�dente
//                        try{
//                            /*Determine le dernier mois actif de la p�riode pr�c�dente ( si �tude comparative).
//                             * Pour cela,  on parcours  la liste des mois en partant du dernier, et on recup�re celui dont l'invetissement
//                             * est sup�rieur � z�ro.
//                             * */
//                            //Liste des mois de la p�riode pr�c�dente
//                            LatestActiveMonths = IndicatorDataAccess.StringMonthsComparativeStudy(webSession);
//                            if(WebFunctions.CheckedText.IsStringEmpty(LatestActiveMonths) && webSession.ComparativeStudy){								
//                                LatestActiveMonthsArr=LatestActiveMonths.Split(',');
//                                if(LatestActiveMonthsArr!=null && LatestActiveMonthsArr.Length>0){
//                                    for(int j=LatestActiveMonthsArr.Length-1;j>=0;j--){
//                                        //on parcours  la liste des mois en partant du dernier, et on recup�re celui dont l'invetissement est sup�rieur � z�ro
//                                        if( double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString()) >(double)0.0){
////											tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]=double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString())/(double)1000.0;
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]=double.Parse(currentRow[LatestActiveMonthsArr[j].ToString().Trim()].ToString());
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]= WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month.ToString();
//                                            tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]=DateFunctions.MonthString.GetCharacters(WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month,cultureInfo,0)+" "
//                                            +WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Year;
//                                            //Periode d'inactivit�
//                                            try{
//                                                //mois courant
//                                                currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//                                                //nombre mois d'inactivit�  = (Mois actif - 1) +(12 - Dernier mois actif ann�e N-1)												
//                                                nbMonthInactivityPeriod = (WebFunctions.Dates.GetDateFromAlias(currentMonth).Month -1) + ( 12 - WebFunctions.Dates.GetDateFromAlias(LatestActiveMonthsArr[j].ToString().Trim()).Month );
//                                                if(nbMonthInactivityPeriod > 0)
//                                                    tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= nbMonthInactivityPeriod.ToString();
//                                                else{
//                                                    tab[i,ConstResults.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]= "";  
//                                                }
//                                                activeMonthFounded=true;
//                                            }catch(Exception per){
//                                                throw( new Exceptions.IndicateurNoveltyRulesException("Impossible de d�terminer la p�riode d'inactivit� : ",per));
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
//                            throw (new Exceptions.IndicateurNoveltyRulesException("Imposible de d�terminer le Dernier mois actif pour l'ann�e N-1 :",actEr) );
//                        }
//                        #endregion

//                        #region Mois actif ann�e N
//                        //Investissement Mois actif ann�e N
//                        try{
//                            //Dernier mois actif ann�e N
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
//                        //SOV en fonction du total univers ou march� ou famille pour mois actif ann�e N.
//                        //Le SOV exprime, en %, investissement du nouveau Produit sur le mois en cours / Investissement du Total (March�, famille, univers) sur le mois en cours.
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
//                                //SOV sur total famille ou march�
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
//            // Il n'y a pas de donn�es
//            if(tab==null)return(new object[0,0]);	
			
//            return tab;
//        }
//    }
//}
