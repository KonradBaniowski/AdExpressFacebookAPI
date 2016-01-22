//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de cr�ation: 30/09/2004 
//// Date de modification: 24/11/2004 
////	12/08/2005	A.Dadouch	Nom de fonctions	
//#endregion

//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using TNS.AdExpress.Web.Exceptions;
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
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

//namespace TNS.AdExpress.Web.Rules.Results
//{
//    /// <summary>
//    /// Classe m�tier de traitement des donn�es issues de la base pour 
//    /// l'indicateur saisonnalit� (analyse sectorielle).
//    /// </summary>
//    public class IndicatorSeasonalityRules {	
		
//        #region "Rules" Saisonnalit� pour pr�sentation tableau 
//        /// <summary>
//        /// Donne sur le total support de l'univers,march� ou famille, ou sur les
//        /// annonceurs de r�f�rences ou concurrents :
//        /// -les investissements de l'ann�e N
//        /// -une �volution N vs N-1 (uniquement dans le ca d'une �tude comparative)
//        /// -le nombre de r�f�rence
//        /// -le budget moyen par r�f�rence
//        /// -le 1er annonceur en Ke et SOV (uniquement pour les lignes total produits �ventuels)
//        /// -la 1ere r�f�rence en Ke et SOV (uniquement pour les lignes total produits �ventuels)
//        /// </summary>
//        /// <remarks>Cette m�thode est utilis�e pour la pr�sentation tableau des donn�es.</remarks>
//        /// <param name="webSession">session du client</param>		
//        /// <returns>tableau mensuel comparatif mois par mois</returns>
//        public static object[] GetFormattedTable(WebSession webSession){														
			
//            #region variables
//            //Groupes de donn�es pour annonceurs de r�f�rences ou concurrents
//            DataSet ds=null;
//            //Groupes de donn�es pour total supports de l'univers
//            DataSet dsTotalUniverse=null;
//            //Groupes de donn�es pour total supports de march� ou famille
//            DataSet dsTotal=null;
//            //Table de donn�es pour annonceurs de r�f�rences ou concurrents
//            DataTable dt=null;			
//            //DataTable dtTotalUniverse=null;		
//            //ID ancien annonceur
//            Int64  oldIdAdvertiser=0;
//            //Int64 oldIdProduct=0;
//            //index ancienne ligne du tableau de r�sultats
//            Int64 oldCurrentLine=0;
//            //ancien nomre de lignes total � traiter
//            Int64 oldMaxLinesToTreat=0;	
//            //ligne courante
//            Int64 currentLine=0;
//            //debut boucle
//            Int64 Start=0;
//            //Int64 StartAd=0;
//            //nombre ligne du tableau de r�sultats
//            Int64 nbTabLines=0;
//            //nombre d'annonceurs
//            int nbAdvertiser=0;	
//            // investissement temporaire total des r�f�rences
//            int tempTotalRef=0;	
//            //liste Identifiants temporaires produits 
//            ArrayList temIdProduct= new ArrayList();
//            //liste Identifiants produits 
//            ArrayList IdProductList = new ArrayList();
//            //liste identifiants temporaires r�f�rences par annonceur
//            ArrayList temIdRefByAdver = new ArrayList();
//            //liste temporaire des annonceurs
//            ArrayList tempAdvertiser = new ArrayList();
//            //tableau de r�sultats total march�
//            object[] oTotal =null;
//            //tableau de r�sultats annonceurs de r�f�rences ou concurrents
//            object[,] tab = null;	
//            //tableau de r�sultats total univers
//            object[,] tabTotalUniv = null;
//            //tableua de r�sultats total famille
//            object[,] tabTotal = null;
//            //Tableau de r�sultats principal
//            object[] tabResult = new object[4];
//            //tableau de r�sultats annonceurs de r�f�rences ou concurrents
//            object[] oTotalAdvert=null;
//            //tableau de r�sultats total univers
//            object[] oTotalUniv = null;
//            //Investissement du mois en cours de traitement
//            Decimal currentMonthInvest=(Decimal)0.0;
//            //Investissement du mois N-1
//            Decimal currentComparMonthInvest=(Decimal)0.0;										
//            //nombre maximal de lignes
//            Int64 maxLinesToTreat=0;						
//            //nombre maximal de colonne				
//            Int64 nbMaxColumn=17;
//            int n=0;
//            #region Compte le nombre de mois d'etude
//            //nombre de mois �tudi�s
//            int nbMonths=0;

//            //D�termine dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod="";
//            try {
//                absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            }catch(System.Exception ){
//                return null;
//            }
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();

//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodBeginningDate(absolutEndPeriod, webSession.PeriodType);
//            nbMonths =  (int)PeriodEndDate.Month - (int)PeriodBeginningDate.Month + 1;
//            #endregion	
//            //Mois courant
//            string currentMonth="";
//            //Mois courant p�riode N-1
//            string currentComparMonth="";
//            //ID ann�e N
//            int YearID=WebFunctions.Dates.yearID(PeriodBeginningDate,webSession);			
//            #endregion
			
//            #region Calculs pour le total support(univers,march� ou famille) et , les annonceurs et des r�f�rences 						

//            #region Initialisation tableau de r�sultats	pour les annonceurs et des r�f�rences 		
//            /*
//            *Initialisation du tableau ("tab") qui contiendra (1 annonceur par ligne) la liste des annonceurs
//            de r�f�rences ou concurrents et leurs investissements mensuels,Evolution par rapport au mois 
//            de la p�rode N-1,nombre de r�f�rences et le budget moyen 
//            */
//            //Groupe de donn�es pour annonceurs de r�f�rences ou concurrents
//            ds=IndicatorDataAccess.GetData(webSession,true,true);
		
//            if(ds!=null)dt=ds.Tables[0];
//            if(IsAdvertiserSelected(webSession) && dt!=null){
//                if(dt.Rows.Count>0){
//                    oTotalAdvert = GetGlobalParams(dt,PeriodBeginningDate,PeriodEndDate,webSession,nbMaxColumn,true);
//                    //initialisation du tableau de r�sultats pour annonceurs de r�f�rences ou concurrents
//                    tab = (object[,])oTotalAdvert[2];
//                }
//            }	
//            #endregion

//            #region tableau de r�sultats pour le total univers 
//            /*
//            *Initialisation et insertion de donn�es, pour tableau ("tabTotalUniv") de r�sultats du total univers.
//            Chaque ligne contient : l'investissements mensuel,evolution par rapport au mois 
//            de la p�rode N-1,nombre de r�f�rences et le budget moyen ,1er annonceur avec son
//            investissement et SOV,1 ere r�f�rence avec son investissement et SOV.
//            */
//             //Groupe de donn�es pour total univers 		
//            dsTotalUniverse=IndicatorDataAccess.GetData(webSession,false,true);
//            //insertion des donn�es dans le tableau
//            if(dsTotalUniverse!= null && dsTotalUniverse.Tables[0]!=null && dsTotalUniverse.Tables[0].Rows.Count>0){				
//                oTotalUniv = GetGlobalParams(dsTotalUniverse.Tables[0],PeriodBeginningDate,PeriodEndDate,webSession,nbMaxColumn,false);
//                tabTotalUniv = (object[,]) oTotalUniv[2];
//            }
//            #endregion

//            #region tableau de r�sultats pour le total famille ou march�
//            /*
//            *Pour le total march� chaque ligne de la tablea de donn�es recup�r�e correspond
//            � l'investissement mensuel,evolution par rapport au mois 
//            de la p�rode N-1,nombre de r�f�rences et le budget moyen ,1er annonceur avec son
//            investissement et SOV,1 ere r�f�rence avec son investissement et SOV.
//            Remarque : Traitement distinct du total famille pour optimiser les temps de r�ponses
//            par utilisation de proc�dures stock�es.
//            */	  			
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal){
//                dsTotal=IndicatorDataAccess.GetSeasonalityTotalForPeriod(webSession,webSession.ComparaisonCriterion); //Total march�
//            }
//            else dsTotal=IndicatorDataAccess.GetData(webSession,false,false); //Total famille
//            /*
//            *Initialisation et insertion de donn�es, pour tableau ("tabTotal") de r�sultats du total famille.
//            Chaque ligne contient : l'investissements mensuel,evolution par rapport au mois 
//            de la p�rode N-1,nombre de r�f�rences et le budget moyen ,1er annonceur avec son
//            investissement et SOV,1 ere r�f�rence avec son investissement et SOV.
//            */		
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && dsTotal!=null && dsTotal.Tables[0]!=null && (dsTotal.Tables[0].Rows.Count>0)){
//                oTotal = GetGlobalParams(dsTotal.Tables[0],PeriodBeginningDate,PeriodEndDate,webSession, nbMaxColumn,false);
//                if(oTotal!=null)tabTotal = (object[,]) oTotal[2];
//            }
//            #endregion
			
//            // Il n'y a pas de donn�es
//            if(tab==null && tabTotal==null && tabTotalUniv==null && dsTotal==null )return(new object[0]);			
//            oldIdAdvertiser=0;			
//            #endregion			

//            #region Construction du tableau de r�sultats pour les annonceurs de r�f�rences ou concurrents s�lectionn�s (optionnel)
//            /*Pour annonceurs de r�f�rences ou concurrents chaque ligne du tableau contiendra ( 1 ligne par annonceur et par mois)
//                Les investissements mensuels, evolution, nb de r�f�rences,budget moyen.				
//            */
//            //On recupere le nombre d'annonceurs (pour s�lection annonceurs concurrents ou r�f�rences)
//            if(oTotalAdvert !=null && oTotalAdvert[0]!=null && dt!=null && dt.Rows.Count>0 && IsAdvertiserSelected(webSession))nbAdvertiser=int.Parse(oTotalAdvert[0].ToString());
										
//            //Nombre de lignes � traiter (pour s�lection annonceurs concurrents ou r�f�rences)
//            maxLinesToTreat=nbMonths;

//            //Nombre total de lignes (pour s�lection annonceurs concurrents ou r�f�rences)
//            nbTabLines=(oTotalAdvert !=null)?int.Parse(oTotalAdvert[3].ToString()):0;
		
//            //Pour les anonceurs (r�f�rences ou concurrents) s�lectionn�s (optionnel)		
//            if(dt!= null && tab!=null ) {			
//                #region remplissage du tab et calculs investissements

//                #region libell�s tab
//                // Libell�s du tableaux 				
//                tab[0,ConstResults.Seasonality.MONTH_COLUMN_INDEX]="LIBELLE Mois";
//                tab[0,ConstResults.Seasonality.VALUE_MONTH_COLUMN_INDEX]="ID Mois";
//                tab[0,ConstResults.Seasonality.COMPAR_MONTH_COLUMN_INDEX]="ID Mois Concurrent";
//                tab[0,ConstResults.Seasonality.COMPAR_VALUE_MONTH_COLUMN_INDEX]="LIBELLE Mois Concurrent";
//                tab[0,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX]="id Annonceur";
//                tab[0,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX]="Libelle Annonceur";
//                tab[0,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]="Investissement N";
//                tab[0,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX]="Investissement N-1";				
//                tab[0,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]="Evol N/N-1";
//                tab[0,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]=GestionWeb.GetWebWord(1152,webSession.SiteLanguage);
//                tab[0,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]=GestionWeb.GetWebWord(1153,webSession.SiteLanguage);
//                tab[0,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX]=GestionWeb.GetWebWord(1154,webSession.SiteLanguage);
//                tab[0,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX]="FirstAdInvest";
//                tab[0,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX]=GestionWeb.GetWebWord(437,webSession.SiteLanguage);
//                tab[0,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX]=GestionWeb.GetWebWord(1155,webSession.SiteLanguage);
//                tab[0,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX]="FirstRefInvest";
//                tab[0,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX]=GestionWeb.GetWebWord(437,webSession.SiteLanguage);
//                #endregion	

//                #region calcul avec droits produits	
			
//                currentLine+=1;				
//                //On remplit la colonne des lib�ll�s de mois
//                tab = FillMonthIndexColum(tab, nbTabLines,(int)PeriodBeginningDate.Month,(int)PeriodEndDate.Month,PeriodEndDate,ConstResults.Seasonality.MONTH_COLUMN_INDEX,ConstResults.Seasonality.VALUE_MONTH_COLUMN_INDEX,webSession);
//                if(webSession.ComparativeStudy)tab = FillMonthIndexColum(tab,nbTabLines,(int)PeriodBeginningDate.Month,(int)PeriodEndDate.Month,PeriodEndDate.AddYears(-1),ConstResults.Seasonality.COMPAR_MONTH_COLUMN_INDEX,ConstResults.Seasonality.COMPAR_VALUE_MONTH_COLUMN_INDEX,webSession);				
				
//                #region Pour chaque ligne de donn�es
			
//                foreach(DataRow currentRow in dt.Rows){	
//                    //On passe aux lignes dans le tab correspondant au prochain annonceur
//                    if(IsAdvertiserSelected(webSession) && oldIdAdvertiser!=(Int64)currentRow["id_advertiser"] && Start==1){
//                        currentLine += nbMonths;	
//                        maxLinesToTreat+=nbMonths;
//                    }

//                    #region remplissage du tableau par annonceur
//                    //Pour un annonceur
//                    n=PeriodBeginningDate.Month;
//                    for(Int64 i=currentLine;i<=maxLinesToTreat;i++){
//                        //id et Libelle annonceur 
//                        if(IsAdvertiserSelected(webSession) && oldIdAdvertiser!=(Int64)currentRow["id_advertiser"]  && !tempAdvertiser.Contains(currentRow["id_advertiser"].ToString())){
//                            tab[i,ConstResults.Seasonality.ADVERTISER_COLUMN_INDEX]=currentRow["advertiser"].ToString();
//                            tab[i,ConstResults.Seasonality.ID_ADVERTISER_COLUMN_INDEX]=currentRow["id_advertiser"].ToString();															
//                                //Mois en cours
//                                currentMonth=WebFunctions.Dates.GetMonthAlias(n,YearID,3,webSession);
//                                //Calcul des investissements de chaque mois s�lectionn� sur l'ann�e N (Nouvelle version)
//                                tab[i,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]=currentMonthInvest=Decimal.Parse(dt.Compute("sum("+currentMonth+")", " id_advertiser="+currentRow["id_advertiser"].ToString()).ToString());																																		
//                                //Calcul nombre de r�f�rences mensuelles par annonceur															
//                                tab[i,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]=tempTotalRef=int.Parse(dt.Compute("count("+currentMonth+")", ""+currentMonth.ToString()+" >0 AND id_advertiser="+currentRow["id_advertiser"].ToString()).ToString());																																																	
//                                //Calcul du budget moyen = budget/nombre de r�f�rences																													
//                                tab[i,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]=(tempTotalRef>0 && (Decimal.Parse(currentMonthInvest.ToString())>=(Decimal)0.0))? (Decimal.Parse(currentMonthInvest.ToString())/Decimal.Parse(tempTotalRef.ToString())) : Decimal.Parse(currentMonthInvest.ToString());																																												
//                        }																	
//                        //Calcul de l'�volution par rapport � ann�e N-1 
//                        if(webSession.ComparativeStudy && oldIdAdvertiser!=(Int64)currentRow["id_advertiser"]  && !tempAdvertiser.Contains(currentRow["id_advertiser"].ToString())){
//                            currentComparMonth=tab[i,ConstResults.Seasonality.COMPAR_MONTH_COLUMN_INDEX].ToString();
//                            //Calcul des investissements de chaque mois s�lectionn� sur l'ann�e N -1												
//                            if(WebFunctions.CheckedText.IsStringEmpty(currentComparMonth))						   
//                            tab[i,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX]=currentComparMonthInvest=Decimal.Parse(dt.Compute("sum("+currentComparMonth+")", " id_advertiser="+currentRow["id_advertiser"].ToString()).ToString());																																									
//                            //Calcul de l'evolution anne� N par rapport N-1	= ((N-(N-1))*100)/N-1 																					
//                            tab[i,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]=(tab[i,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]!=null && tab[i,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX]!=null && 
//                                Decimal.Parse(tab[i,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX].ToString())!=(Decimal)0.0 )?(((Decimal.Parse(tab[i,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString()) - Decimal.Parse(tab[i,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX].ToString()) ) * (Decimal)100.0)/Decimal.Parse(tab[i,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX].ToString())) : (Decimal)0.0;						
//                        }
//                        n++;									
//                    }				
//                    #endregion

//                    //Traitement distinct des annonceurs
//                    if(!tempAdvertiser.Contains(currentRow["id_advertiser"].ToString())){
//                        tempAdvertiser.Add(currentRow["id_advertiser"].ToString());
//                    }
//                    Start=1;					
//                    oldCurrentLine=currentLine;
//                    oldMaxLinesToTreat=maxLinesToTreat;
//                    tempTotalRef=0;							
//                    oldIdAdvertiser=(Int64)currentRow["id_advertiser"];
//                }
//                #endregion	

//                tempTotalRef=0;	
			
//                #endregion	
									
//                #endregion										
//            }	
//            #endregion
			
//            if(tab!=null)tabResult[0]=tab;
//            if(tabTotal!=null)tabResult[1]=tabTotal;
//            if(tabTotalUniv!=null)tabResult[2]=tabTotalUniv;
//            if(dsTotal!=null && webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal)tabResult[3]=dsTotal.Tables[0];
				
//            return tabResult;
//        }
//        #endregion

//        #region "Rules" Saisonnalit� pour pr�sentation graphique 
//        /// <summary>
//        /// Donne sur le total support de l'univers,march� ou famille, ou sur les
//        /// annonceurs de r�f�rences ou concurrents :
//        /// -les investissements de l'ann�e N		
//        /// </summary>
//        /// <remarks>Cette m�thode est utilis�e pour la pr�sentation graphique des donn�es.</remarks>
//        /// <param name="webSession">session du client</param>		
//        /// <returns>tableau mensuel comparatif mois par mois</returns>
//        public static object[,] GetChartFormattedTable(WebSession webSession){
			
//            #region variables
//            //index ligne tableau de r�sultats 
//            int IndexTabRow =0;
//            //nombre max de lignes tab
//            int nbMaxRow=0;
//            //Alias mois �tudi�
//            string MonthAlias="";			
//            //tableau des r�sultats investissements mensuels
//            object[,] tab = null;
//            //Groupes de donn�es pour annonceurs de r�f�rences ou concurrents
//            DataSet ds=null;
//            //Groupes de donn�es pour total supports de l'univers
//            DataSet dsTotalUniverse=null;
//            //Groupes de donn�es pour total supports de march� ou famille
//            DataSet dsTotal=null;
//            //Tables  de donn�es pour annonceurs de r�f�rences ou concurrents
//            DataTable dt=null;
//            //Tables de donn�es pour total supports de l'univers
//            DataTable dtTotalUniverse=null;
//            //Tables de donn�es pour total supports de march� ou famille
//            DataTable dtTotal=null;
//            //annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            #endregion

//            #region liste des annonceurs de r�f�rences et concurrents						

//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            #endregion

//            #region Chargement des donn�es dans "DataSet"			
//            //Chargement des investissments mensuels pour annonceurs de r�f�rences et/ou concurrents
//            if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList.ToString().Trim()))
//            ds=IndicatorDataAccess.GetChartData(webSession,true,true);
//            //Chargement des investissments mensuels pour total univers
//            dsTotalUniverse=IndicatorDataAccess.GetChartData(webSession,false,true);
//            //Chargement des investissments mensuels pour total famille ou march�
//            dsTotal=IndicatorDataAccess.GetChartData(webSession,false,false);			
//            #endregion

//            #region Chargement des donn�es dans "DataTable"
//            //Chargement des investissments mensuels pour annonceurs de r�f�rences et/ou concurrents dans table de donn�es
//            if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
//                dt=ds.Tables[0];
//            }
//            //Chargement des investissments mensuels total univers dans table de donn�es
//            if(dsTotalUniverse!=null && dsTotalUniverse.Tables[0]!=null && dsTotalUniverse.Tables[0].Rows.Count>0){
//                dtTotalUniverse=dsTotalUniverse.Tables[0];
//            }
//            //Chargement des investissments mensuels total famille ou march� dans table de donn�es
//            if(dsTotal!=null && dsTotal.Tables[0]!=null && dsTotal.Tables[0].Rows.Count>0){
//                dtTotal=dsTotal.Tables[0];
//            }			
//            #endregion
			
//            //Calcul nombre max de lignes du tab = nombre de lignes chaque datatable * nombre de mois �tudi�s
//            if(dt!=null && dt.Rows.Count >0)nbMaxRow+= dt.Rows.Count*(dt.Columns.Count - 2);
//            if(dtTotalUniverse!=null && dtTotalUniverse.Rows.Count ==1)nbMaxRow+= dtTotalUniverse.Rows.Count*(dtTotalUniverse.Columns.Count);
//            if(dtTotal!=null && dtTotal.Rows.Count ==1)nbMaxRow+= dtTotal.Rows.Count*(dtTotal.Columns.Count);
			
//            //Initialisation des dimensions du tableau
//            if(nbMaxRow>0)tab = new object[nbMaxRow,ConstResults.Seasonality.NB_MAX_COLUMNS_COLUMN_INDEX];
//            // Il n'y a pas de donn�es
//            if(tab==null )return(new object[0,0]);	

//            #region p�riode �tudi�e
//            //D�termine dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);			
//            #endregion
			
//            #region Cr�ation du tableau de r�sultats							
			
//            //Pour chaque mois s�lectionn� on ajoute une ligne dans tableau de r�sultats
//            for(int i=PeriodBeginningDate.Month;i<=PeriodEndDate.Month;i++){
//                MonthAlias = WebFunctions.Dates.GetMonthAlias(i,WebFunctions.Dates.yearID(PeriodBeginningDate,webSession),3,webSession);
//                if(WebFunctions.CheckedText.IsStringEmpty(MonthAlias.ToString().Trim())){						
//                    /*ID annonceur de r�fence ou concurrents, ou total (march� ou univers ou famille)*/
//                    //Cas ID total march� ou famille
//                    if(dtTotal!=null && dtTotal.Rows.Count==1 && dtTotal.Columns.Contains(MonthAlias.ToString().Trim())){
//                        //Insertion ID Mois �tudi�								
//                        tab[IndexTabRow,ConstResults.Seasonality.ID_MONTH_COLUMN_INDEX] = WebFunctions.Dates.GetDateFromAlias(MonthAlias.ToString().Trim()).Month.ToString();
//                        if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal){
//                            //ID total march�
//                            tab[IndexTabRow,ConstResults.Seasonality.ID_ELEMENT_COLUMN_INDEX] = ConstResults.Seasonality.ID_TOTAL_MARKET_COLUMN_INDEX.ToString();
//                            //libell� total march�
//                            tab[IndexTabRow,ConstResults.Seasonality.LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1190,webSession.SiteLanguage).ToString();										
//                        }
//                        else if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                            //ID total famille
//                            tab[IndexTabRow,ConstResults.Seasonality.ID_ELEMENT_COLUMN_INDEX] = ConstResults.Seasonality.ID_TOTAL_SECTOR_COLUMN_INDEX.ToString();
//                            //lib�ll� total famille
//                            tab[IndexTabRow,ConstResults.Seasonality.LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1189,webSession.SiteLanguage).ToString();										
//                        }
//                        //investissment mensuel total march� ou famille
//                        tab[IndexTabRow,ConstResults.Seasonality.INVEST_COLUMN_INDEX] =  dtTotal.Rows[0][MonthAlias.ToString().Trim()].ToString();
//                        IndexTabRow++;
//                    }
//                    //Cas ID total univers
//                    if(dtTotalUniverse!=null && dtTotalUniverse.Rows.Count==1 && dtTotalUniverse.Columns.Contains(MonthAlias.ToString().Trim())){
//                        //Insertion ID Mois �tudi�								
//                        tab[IndexTabRow,ConstResults.Seasonality.ID_MONTH_COLUMN_INDEX] = WebFunctions.Dates.GetDateFromAlias(MonthAlias.ToString().Trim()).Month.ToString();
//                        //ID total univers
//                        tab[IndexTabRow,ConstResults.Seasonality.ID_ELEMENT_COLUMN_INDEX] = ConstResults.Seasonality.ID_TOTAL_UNIVERSE_COLUMN_INDEX.ToString();
//                        //lib�ll� total univers
//                        tab[IndexTabRow,ConstResults.Seasonality.LABEL_ELEMENT_COLUMN_INDEX] = GestionWeb.GetWebWord(1188,webSession.SiteLanguage).ToString();
//                        //investissment mensuel total univers
//                        tab[IndexTabRow,ConstResults.Seasonality.INVEST_COLUMN_INDEX] =  dtTotalUniverse.Rows[0][MonthAlias.ToString().Trim()].ToString();
//                        IndexTabRow++;
//                    }								
//                    //Cas ID annonceurs
//                    if(dt!=null && dt.Rows.Count>0 && dt.Columns.Contains(MonthAlias.ToString().Trim())  && dt.Columns.Contains("id_advertiser") && dt.Columns.Contains("advertiser")){
//                        for(int j=0; j<dt.Rows.Count;j++){
//                            //Insertion ID Mois �tudi�								
//                            tab[IndexTabRow,ConstResults.Seasonality.ID_MONTH_COLUMN_INDEX] = WebFunctions.Dates.GetDateFromAlias(MonthAlias.ToString().Trim()).Month.ToString();
//                            //ID annonceur
//                            tab[IndexTabRow,ConstResults.Seasonality.ID_ELEMENT_COLUMN_INDEX]= dt.Rows[j]["id_advertiser"].ToString();
//                            //lib�ll� annonceur
//                            tab[IndexTabRow,ConstResults.Seasonality.LABEL_ELEMENT_COLUMN_INDEX] =  dt.Rows[j]["advertiser"].ToString();
//                            //investissment mensuel
//                            tab[IndexTabRow,ConstResults.Seasonality.INVEST_COLUMN_INDEX] =  dt.Rows[j][MonthAlias.ToString().Trim()].ToString();
//                            IndexTabRow++;
//                        }
//                    }																
//                }
//            }
			
			
//            #endregion
			
//            return tab;
//        }
//        #endregion

//        #region Rules saisonnalit� pour les annonceurs de r�f�rences et/ou concurrents
//        /// <summary>
//        /// Fonction qui formate un DataSet en un tableau mensuel comparatif mois par mois des investissements 
//        /// </summary>
//        /// <param name="webSession">session du client</param>		
//        /// <returns>Tableau format�</returns>
//        public static object[] GetSeasonalityFormattedTable(WebSession webSession){
//            #region variables
//            DataSet ds=null;
//        //	DataSet dsTotalUniverse=null;
//        //	DataSet dsTotal=null;
//            DataTable dt=null;			
//        //	DataTable dtTotalUniverse=null;		
//            object[] tabResult = new object[3];
//            #endregion
//             ds = IndicatorDataAccess.GetSeasonalityData(webSession);
//            if(ds!=null && ds.Tables[0] !=null && ds.Tables[0].Rows.Count>0){
//                dt = ds.Tables[0];
//            }
//            return tabResult;
//        }
//        #endregion

//        #region Totaux Rules Saisonnalit�
		
//        /// <summary>
//        /// Fonction qui formate un DataSet en un tableau mensuel comparatif mois par mois des investissements 
//        /// Pour le total periode (univers ou famille ou march�)
//        /// Renvoie aussi le premier annonceur et/ou la premi�re r�f�rence
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="comparisonCriterion">Crit�re de comparaison</param>		
//        /// <returns>Tableau format�</returns>
//        public static object[] GetSeasonalityTotalFormattedTable(WebSession webSession,WebConstantes.CustomerSessions.ComparisonCriterion comparisonCriterion){
//            #region variables			
//            DataSet dsTotal=null;				
//            object[] tabResult = new object[3];
//            #endregion
//            dsTotal=IndicatorDataAccess.GetSeasonalityTotalForPeriod(webSession,comparisonCriterion );
//            return tabResult;
//        }
//        #endregion

//        #region methodes internes

//        /// <summary>
//        /// Rempli le tableau d'objets avec les lib�ll�s des mois s�lectionn�s
//        /// </summary>
//        /// <param name="tab">tableau bidimensionnelle d'object</param>
//        /// <param name="nbTabLines">nombre maximale de lignes</param>
//        /// <param name="beginmonth">premier du mois de la p�riode s�lectionn�e</param>
//        /// <param name="endmonth">dernier mois de la p�riode s�lectionn�e</param>
//        /// <param name="MONTH_COLUMN_INDEX">index de colonne pour le lib�ll� d'un mois</param>
//        /// <param name="VALUE_MONTH_COLUMN_INDEX">index de colonne pour l'identifiant d'un mois</param>
//        /// <param name="PeriodEndDate">fin de la p�riode �tudi�e</param>
//        /// <param name="webSession">Session client</param>
//        /// <returns>tabelau objet 2 dimensions</returns>
//        public static object[,] FillMonthIndexColum(object[,] tab, Int64 nbTabLines, int beginmonth, int endmonth,DateTime PeriodEndDate,Int64 MONTH_COLUMN_INDEX, Int64 VALUE_MONTH_COLUMN_INDEX ,WebSession webSession){
			
//            int year=0;
//            int tempMonth=beginmonth;
//            int downLoadDate=webSession.DownLoadDate;
//            if(DateTime.Now.Year>downLoadDate){
//                if(PeriodEndDate.Year==DateTime.Now.Year-1)year=0;
//                else if(PeriodEndDate.Year==DateTime.Now.Year-2)year=1; 
//                else if(PeriodEndDate.Year==DateTime.Now.Year-3)year=2;
//            }else{
//                if(PeriodEndDate.Year==DateTime.Now.Year-1)year=1;
//                else if(PeriodEndDate.Year==DateTime.Now.Year-2)year=2; 
//            }			
////			if(PeriodEndDate.Year==DateTime.Now.Year-1)year=1;
////			else if(PeriodEndDate.Year==DateTime.Now.Year-2)year=2; 
//            for(int l=1;l< nbTabLines;l++){
//                tab[l,MONTH_COLUMN_INDEX]=WebFunctions.Dates.GetMonthAlias(tempMonth,year,3,webSession);
//                tab[l,VALUE_MONTH_COLUMN_INDEX]=tempMonth;				
//                if(tempMonth==endmonth)tempMonth=beginmonth-1;
//                tempMonth++;
//            }
//            return tab;
//        }
		

//        #region  Calculs totaux univers ou march� ou famille
//        /// <summary>
//        /// Retourne le nombre d'annonceurs ou r�f�rences , le 1er annonceur ou r�f�rence et son investissement
//        /// </summary>
//        /// <param name="dt">DataTable</param>
//        /// <param name="PeriodBeginningDate">debut date etude</param>
//        /// <param name="PeriodEndDate">fin date etude</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="nbMaxColumn">nombre maximal de colonnes pour le tableau</param>	
//        /// <param name="isAdvertCalculation">Est ce que le calcule concerne les annonceurs</param>	
//        /// <returns> object[] avec nombre d'annonceurs , le 1er annonceur et son investissement</returns>
//        public static object[] GetGlobalParams(DataTable dt,DateTime PeriodBeginningDate,DateTime PeriodEndDate,WebSession webSession,Int64 nbMaxColumn, bool isAdvertCalculation){			
			
//            #region Variables annonceurs
//            Decimal TempFirstAdInvest=(Decimal)0.0;			
//            string TempFirstAdInvestName="";
//            object TempForAdSov="";
//            object TempForRefSov="";
//            string currentMonth="";
//            int YearID=0;			
//            ArrayList TempAdvertiserIds = new ArrayList();
//            int MonthsInterval = (PeriodEndDate.Month - PeriodBeginningDate.Month)+1;
//            string[,] AdValues=null;
//            string[,] RefValues=null;
//            object[] AdObject=new object[5];
//            if(MonthsInterval>0)
//            AdValues = new string[MonthsInterval,3];
//            RefValues = new string[MonthsInterval,3];
//            int start=0;
//            Int64 nbTabLines=0;
//            Int64 oldIdAdvertiser=0;
//            int indexLine=0;
//            int t=0;		
//            #endregion

//            #region variables r�f�rences
//            ArrayList IdProductComputedList = new ArrayList();
//            Decimal TempFirstRefInvest=(Decimal)0.0;
//            string TempFirstRefInvestName="";
//            int startRef=0;
//            int indexLineRef=0;
//            int nbProduct=0;
//            int nbAdvertiser=0;			
//            Int64 oldIdProduct=0;
//            Int64 TotStartIndex=0;
//            object[,] tab =null;	
//            int[] TotNbRefByMonth = new int[MonthsInterval];
//            #endregion
			
//            #region Calcul investissement, 1er r�f�rence, 1er annonceur 
//            //Calcul nombre annonceurs, 1er annonceur et son investissement
//            if(dt.Rows.Count>0){
//                foreach(DataRow currentRow in dt.Rows){
//                    //ann�e de l'etude					
//                    YearID=WebFunctions.Dates.yearID(PeriodBeginningDate,webSession);
	
//                    #region annonceur									
//                    if(dt.Columns.Contains("id_advertiser") && oldIdAdvertiser!=(Int64)currentRow["id_advertiser"] ){
//                        //nombre d'annonceurs
//                        nbAdvertiser++;									
//                        //Investissement annonceur courant
//                        if(!isAdvertCalculation){
//                            TempFirstAdInvestName=currentRow["advertiser"].ToString();					
//                            for(int s=(int)PeriodBeginningDate.Month;s<=(int)PeriodEndDate.Month;s++) {					
//                                currentMonth=WebFunctions.Dates.GetMonthAlias(s,YearID,3,webSession);
//                                if(WebFunctions.CheckedText.IsStringEmpty(currentMonth))						
//                                    TempFirstAdInvest=Decimal.Parse(dt.Compute("Sum("+currentMonth+")", "id_advertiser = "+currentRow["id_advertiser"].ToString()+"").ToString());																																		
//                                if(start==0){
//                                    AdValues[indexLine,0]=s.ToString();
//                                    AdValues[indexLine,1]=TempFirstAdInvest.ToString();
//                                    AdValues[indexLine,2]=TempFirstAdInvestName;	
//                                }
//                                else if(Decimal.Parse(AdValues[indexLine,1].ToString())<TempFirstAdInvest) {
//                                    AdValues[indexLine,0]=s.ToString();
//                                    AdValues[indexLine,1]=TempFirstAdInvest.ToString();
//                                    AdValues[indexLine,2]=TempFirstAdInvestName;						
//                                }
//                                indexLine++;						
//                            }							
//                            indexLine=0;
//                            TempFirstAdInvest=(Decimal)0.0;							
//                        }					
//                    }
//                    start=1;
//                    oldIdAdvertiser=(Int64)currentRow["id_advertiser"];
//                    #endregion

//                    #region produit
//                    if(oldIdProduct!=(Int64)currentRow["id_product"] && !IdProductComputedList.Contains(Int64.Parse(currentRow["id_product"].ToString())) && start==1 ){
//                        //nombre de produits
//                        nbProduct++;
//                        //Investissement produit courant
//                        if(!isAdvertCalculation){
//                            TempFirstRefInvestName=currentRow["product"].ToString();
//                            for(int p=(int)PeriodBeginningDate.Month;p<=(int)PeriodEndDate.Month;p++) {					
//                                currentMonth=WebFunctions.Dates.GetMonthAlias(p,YearID,3,webSession);
//                                if(WebFunctions.CheckedText.IsStringEmpty(currentMonth))													
//                                    TempFirstRefInvest=Decimal.Parse(dt.Compute("Sum("+currentMonth+")", "id_product = "+currentRow["id_product"].ToString()+"").ToString());																																									
//                                if(startRef==0){
//                                    RefValues[indexLineRef,0]=p.ToString();
//                                    RefValues[indexLineRef,1]=TempFirstRefInvest.ToString();
//                                    RefValues[indexLineRef,2]=TempFirstRefInvestName.ToString();	
//                                }
//                                else if(Decimal.Parse(RefValues[indexLineRef,1].ToString())<TempFirstRefInvest) {
//                                    RefValues[indexLineRef,0]=p.ToString();
//                                    RefValues[indexLineRef,1]=TempFirstRefInvest.ToString();
//                                    RefValues[indexLineRef,2]=TempFirstRefInvestName.ToString();
//                                }											
//                                indexLineRef++;						
//                            }						
//                            indexLineRef=0;						
//                            TempFirstRefInvest=(Decimal)0.0;
//                        }					
//                        IdProductComputedList.Add(Int64.Parse( currentRow["id_product"].ToString()));
//                    }
//                    startRef=1;
//                    oldIdProduct=(Int64)currentRow["id_product"];
//                    #endregion				
//                }
//            } 
//            #region totaux nombre de r�f�rences
//            try{
//                if(!isAdvertCalculation){
//                    t=0;
//                    for(int n=(int)PeriodBeginningDate.Month;n<=(int)PeriodEndDate.Month;n++) {					
//                        currentMonth=WebFunctions.Dates.GetMonthAlias(n,YearID,3,webSession);
//                        TotNbRefByMonth[t] =int.Parse(dt.Compute("count("+currentMonth+")", ""+currentMonth.ToString()+" >0 ").ToString());																																		
//                        t++;
//                    }
//                }
//            }catch(Exception nbRefErr){
//                throw (new TNS.AdExpress.Web.Exceptions.IndicatorSeasonalityException("Impossible de d�terminer le nombre de r�f�rences pour chaque mois :",nbRefErr));
//            }
//            #endregion
//            //Calcul nombre de r�f�rence par mois (seulement pour total univers ou famille ou march�)

//            TotStartIndex = (Int64)1;
//            //Nombre total de lignes
//            if(isAdvertCalculation)			
//            nbTabLines=(nbAdvertiser>0)?((Int64)nbAdvertiser*(Int64)MonthsInterval)+1:MonthsInterval+1;
//            else nbTabLines=(Int64)MonthsInterval+1;
//            // Tableau de r�sultat 	
//            if(tab==null)tab=new object[nbTabLines, nbMaxColumn];
//            if(!isAdvertCalculation){
//                tab = GlobalCompute(dt,tab,TotStartIndex,webSession,nbProduct,PeriodBeginningDate,PeriodEndDate,TotNbRefByMonth);
//            }
//            //On rempli le tableau avec 1er annonceur, 1ere r�f�rence, SOV par mois
//            if(!isAdvertCalculation && tab!=null){
//                indexLine=0;
//                 t=0;
//                for(int m=(int)PeriodBeginningDate.Month;m<=(int)PeriodEndDate.Month;m++) {	
//                    //nolmbre de r�f�rences par mois
//                    if(TotNbRefByMonth!=null)tab[TotStartIndex,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]=TotNbRefByMonth[t].ToString();
//                    if(AdValues!=null && AdValues.Length>0 ){
//                        //1er annonceur			
//                        if(AdValues[indexLine,2]!=null)tab[TotStartIndex,ConstResults.Seasonality.FIRST_ADVERTISER_COLUMN_INDEX]=AdValues[indexLine,2].ToString();
//                        if(AdValues[indexLine,1]!=null){
//                            //investissement 1er annonceur				
//                            tab[TotStartIndex,ConstResults.Seasonality.FIRST_ADVERTISER_INVEST_COLUMN_INDEX]=AdValues[indexLine,1].ToString() ;
//                            //SOV 1er annonceur				
//                            TempForAdSov=tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX];				
//                            tab[TotStartIndex,ConstResults.Seasonality.FIRST_ADVERTISER_SOV_COLUMN_INDEX]=
//                                (TempForAdSov!=null && Decimal.Parse(tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())>(Decimal)0.0)?(Decimal.Parse(AdValues[indexLine,1].ToString())*(Decimal)100.0)/Decimal.Parse(tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString()) :(Decimal)0.0;
//                        }
//                    }
//                    if(RefValues!=null && RefValues.Length>0){	
//                        //1ere r�f�rence
//                        if(RefValues[indexLine,2]!=null)tab[TotStartIndex,ConstResults.Seasonality.FIRST_PRODUCT_COLUMN_INDEX]=RefValues[indexLine,2].ToString() ;
//                        if(RefValues[indexLine,2]!=null){
//                            //investissement 1ere r�f�rence
//                            tab[TotStartIndex,ConstResults.Seasonality.FIRST_PRODUCT_INVEST_COLUMN_INDEX]=RefValues[indexLine,1].ToString();
//                            //SOV 1ere  r�f�rence = ((N-(N-1))*100)/N-1
//                            TempForRefSov=tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX];
//                            tab[TotStartIndex,ConstResults.Seasonality.FIRST_PRODUCT_SOV_COLUMN_INDEX]=(TempForRefSov!=null && Decimal.Parse(tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString())>(Decimal)0.0)?(Decimal.Parse(RefValues[indexLine,1].ToString())*(Decimal)100.0)/Decimal.Parse(tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX].ToString()) :(Decimal)0.0;										
//                        }
//                    }
//                    TotStartIndex++;
//                    indexLine++;
//                    t++;
//                }
//            }
//            #endregion

//            AdObject[0] = nbAdvertiser;
//            AdObject[1] = nbProduct;
//            AdObject[2]= tab;
//            AdObject[3]= nbTabLines;
//            AdObject[4]= TotNbRefByMonth;

//            return AdObject;
//        }
//        #endregion

//        #region Calcul total investissement par mois
//        /// <summary>
//        /// Calcul investissement total par mois
//        /// </summary>
//        /// <param name="monthID">ID mois</param>
//        /// <param name="YearID">ID ann�e etudi�e</param>
//        /// <param name="dt">source de donn�es</param>
//        /// <param name="webSession">Session client</param>
//        /// <returns>investissement total par mois</returns>
//        public static Decimal TotalInvestmentByMonth(int monthID,int YearID,DataTable dt,WebSession webSession){
//            string currentMonth="";
//            Decimal Invest =(Decimal)0.0;
//            currentMonth=WebFunctions.Dates.GetMonthAlias(monthID,YearID,3,webSession);
//            if(WebFunctions.CheckedText.IsStringEmpty(currentMonth))
//            Invest=Decimal.Parse(dt.Compute("Sum("+currentMonth+")", "").ToString());
//            return Invest;
//        }
//        #endregion

//        #region calcul totaux investissement, evolution, nb r�f�rence, budget moyen
//        /// <summary>
//        /// calcule totaux investissement, evolution, nb r�f�rence, budget moyen
//        /// </summary>
//        /// <param name="dt">Datatable source de donn�es</param>
//        /// <param name="tab">tableau objet � 2 dimension</param>
//        /// <param name="StartIndex">index debut tableau</param>
//        /// <param name="webSession">session client</param>
//        /// <param name="TotalRef">nombre total de r�f�rences</param>
//        /// <param name="PeriodBeginningDate">debut de la p�riode �tudi�e</param>
//        /// <param name="PeriodEndDate">fin de la p�riode �tudi�e</param>
//        /// <param name="TotNbRefByMonth">nombre total de r�f�rences par mois</param>
//        /// <returns>tableau (resultats) objet � 2 dimension</returns>
//        public static object[,] GlobalCompute(DataTable dt,object[,] tab,Int64 StartIndex, WebSession webSession, Int64 TotalRef,DateTime PeriodBeginningDate,DateTime PeriodEndDate,int[] TotNbRefByMonth){		
//            #region calcul total univers 
//            #region variables
//            Decimal TotalInvestByMonth=(Decimal)0.0;
//            Decimal TotalComparativeInvestByMonth=(Decimal)0.0;
//            Decimal Evolution = (Decimal)0.0;
//            Decimal BudgetAverage = (Decimal)0.0;
//            //Int64 TotStartIndex=TOTAL_LINE_START_INDEX;
//            Int64 TotStartIndex=StartIndex;
//            int t=0;
//            #endregion
//            //Calcul des investissements Pour ann�e N
//            for(int s=(int)PeriodBeginningDate.Month;s<=(int)PeriodEndDate.Month;s++) {
//                TotalInvestByMonth = TotalInvestmentByMonth(s,WebFunctions.Dates.yearID(PeriodBeginningDate,webSession),dt,webSession);
//                tab[TotStartIndex,ConstResults.Seasonality.INVESTMENT_COLUMN_INDEX]=TotalInvestByMonth ;
//                try{
//                //Calcul du nombre de r�f�rences 
//                tab[TotStartIndex,ConstResults.Seasonality.REFERENCE_COLUMN_INDEX]=TotalRef;
//                //Calcul du budget moyen par mois s�lectionn�
//                    if(TotNbRefByMonth!=null && TotNbRefByMonth.Length>0){
//                        BudgetAverage=(Decimal.Parse(TotNbRefByMonth[t].ToString()) >(Decimal)0.0)? TotalInvestByMonth/Decimal.Parse(TotNbRefByMonth[t].ToString()) : TotalInvestByMonth;	
//                        tab[TotStartIndex,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]=BudgetAverage;
//                    }
//                    else tab[TotStartIndex,ConstResults.Seasonality.AVERAGE_BUDGET_COLUMN_INDEX]=TotalInvestByMonth;
//                }catch(Exception nbRefErr){
//                    throw (new TNS.AdExpress.Web.Exceptions.IndicatorSeasonalityException("Impossible de d�terminer le budget moyen de chaque mois :",nbRefErr));
//                }						
//                //Calcul des investissements Pour ann�e N-1 si �tude comparative
//                if(webSession.ComparativeStudy){										
//                    TotalComparativeInvestByMonth = TotalInvestmentByMonth(s,WebFunctions.Dates.yearID(PeriodBeginningDate.AddYears(-1),webSession),dt,webSession);
//                    tab[TotStartIndex,ConstResults.Seasonality.COMPAR_INVESTMENT_COLUMN_INDEX]=TotalComparativeInvestByMonth;				
//                    //Calcul de l'evolution anne� N par rapport N-1	= ((N-(N-1))*100)/N-1 
//                    Evolution=(TotalComparativeInvestByMonth >(Decimal)0.0)? ((TotalInvestByMonth - TotalComparativeInvestByMonth)* (Decimal)100.0)/TotalComparativeInvestByMonth : (Decimal)0.0;
//                    tab[TotStartIndex,ConstResults.Seasonality.EVOLUTION_COLUMN_INDEX]=Evolution;
//                }	
//                TotStartIndex++;
//                t++;
//            }			
//            return tab ;
//            #endregion
//        }		
//        #endregion
	
//        #region verifie si annonceurs s�lectionn�s
//        /// <summary>
//        /// Verifie si des annonceurs (r�f�rence et/ou concurrents)ont �t� s�lectionn�s
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <returns></returns>
//        public static bool IsAdvertiserSelected(WebSession webSession){
//            NomenclatureElementsGroup nomenclatureElementsGroup = null;
//            string competitorAdvertiserAccessList ="";
//            string advertiserAccessList="";
//            string tempListAsString = "";

//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0) advertiserAccessList = tempListAsString;
//                }
//                nomenclatureElementsGroup = null;
//                tempListAsString = "";
//            }
			
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0) competitorAdvertiserAccessList = tempListAsString;
//                }
//                tempListAsString = "";
//            }
//            if (WebFunctions.CheckedText.IsStringEmpty(competitorAdvertiserAccessList)) {
//                if (WebFunctions.CheckedText.IsStringEmpty(advertiserAccessList)) advertiserAccessList += "," + competitorAdvertiserAccessList;
//                else advertiserAccessList = competitorAdvertiserAccessList;
//            }

//            return WebFunctions.CheckedText.IsStringEmpty(advertiserAccessList);
//        }
//        #endregion

//        #endregion
//    }
//}

