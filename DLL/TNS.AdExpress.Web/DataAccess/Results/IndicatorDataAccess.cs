//#region Information
//// Auteur: D�d� Mussuma
//// Cr�� le: 24/09/2004
//// Modifi�e le: 
////	24/09/2004	D�d� Mussuma
////	24/08/2005	G. Facon		Solution temporaire pour les IDataSource
//#endregion

//#region Using
//using System;
//using System.Collections;
//using System.Data;
//using System.Windows.Forms;
//using Oracle.DataAccess.Client;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using TNS.AdExpress.Web.Exceptions;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
//using DBConstantes = TNS.AdExpress.Constantes.DB;
//using DateFunctions = TNS.FrameWork.Date;
//using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
//using TNS.FrameWork.DB.Common;
//using TNS.AdExpress.Domain.DataBaseDescription;
//using TNS.AdExpress.Web.Core;
//using TNS.AdExpress.Domain.Web;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
//#endregion


//namespace TNS.AdExpress.Web.DataAccess.Results{
//    /// <summary>
//    /// Acc�s au donn�es pour les indicateurs
//    /// </summary>
//    public class IndicatorDataAccess{				

//        #region Dataset Saisonnalit�
        
//        #region DataSet Saisonnalit� pour sortie tableau
//        /// <summary>
//        /// Fournit la liste des annonceurs,des r�f�rences, les investissements mensuels (p�riode N et N-1)
//        /// pour indicateur Saisonnalit�, sur le total supports de l'univers ou march� ou famille.
//        /// </summary>
//        /// <remarks>Cette m�thode est utilis�e pour la pr�sentation des donn�es sous forme de tableau.</remarks>
//        /// <param name="webSession">Session du client</param>	
//        /// <param name="withAdvertisers">champs annonceurs</param>	
//        /// <param name="withRights">droits produits</param>
//        /// <returns>Donn�es de saisonnalit� (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifi� le 24/11/04</history>
//        public static DataSet GetData(WebSession webSession, bool withAdvertisers, bool withRights){	
//            #region variables
//            //Lib�ll� de la table "recap" cible
//            string recapTableName="";
//            //boll�en pour construire la requ�te sql
//            bool premier=true;
//            bool firstfield=true;
//            bool firstgrouby=true;
//            bool firstorderby=true;
//            bool beginbyand =true;		
//            //Liste des annonceurs concurrents s�lectionn�s
//            string CompetitorAdvertiserAccessList="";
//            //Liste des familles poss�dant au moins un groupe ou une vari�t� s�lectionn�e
//            string listVehicle="";
//            //Pour stocker les donn�es g�n�r�s par la requ�te
//            DataSet ds =null;
//            //Indique si la requ�te sql doit �tre construite et ex�cit�e
//            bool buildSqlStatement=true;
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s	
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);			
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//            recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
			
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName.ToString().Trim())){
//                /*Liste des familles ayant au moins une vari�t� ou un groupe s�lectionn�.
//                 Cette donn�e est n�cesssaire pour calculer le total famille des produits s�lectionn�s.*/
//                if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                    listVehicle=WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(!WebFunctions.CheckedText.IsStringEmpty(listVehicle))buildSqlStatement=false;
//                }
//                if(buildSqlStatement){
//                    #region Mise en forme des mois de la p�riode s�lectionn�e					
//                    string StudyMonths=IntervalMonthsStudy(webSession,true);
//                    #endregion
		
//                    #region Construction de la requ�te

//                    #region Close Select
//                    string sql="select";
			
//                    #region  selection annonceurs de r�f�rences et concurrents
//                    //selection des annonceurs de r�f�rences en acc�s			
//                    sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//                    firstfield=false;				
//                    #endregion
												
//                    #region  selection des familles 
//                    //selection des familles de produits
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        if(!firstfield)sql+=",";
//                        sql+="  "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector";
//                        firstfield=false;
//                    }		
//                    #endregion
		
//                    #region  selection des produits
//                    //selection des produits
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList) || WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)) {
//                        if(!firstfield)sql+=",";
//                        sql+="  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product ";
//                        firstfield=false;
//                    }		
//                    #endregion

//                    #region  selection des investissement mensuels
//                    //selection des investissement mensuels
//                    if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)) {
//                        if(!firstfield)sql+=",";
//                        sql+="  "+StudyMonths+" ";
//                        firstfield=false;
//                    }		
//                    #endregion

//                    sql+=" from";

//                    #region Tables concern�es par la requete
//                    //TABLE RECAP
//                    if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+"";
//                        premier=false;
//                    }
			
//                    //TABLE ADVERTISER			
//                    if(!premier)sql+=",";
//                    sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+"";
//                    premier=false;
			
			
//                    //TABLE SECTOR (Famille)
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        if(!premier)sql+=",";
//                        sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".sector "+DBConstantes.Tables.SECTOR_PREFIXE+"";
//                        premier=false;
//                    }
			
//                    //TABLE PRODUIT
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList) || WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)) {
//                        if(!premier)sql+=",";
//                        sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+"";
//                        premier=false;
//                    }
//                    #endregion

//                    #endregion	

//                    #region Close Where
//                    sql+=" Where";

//                    #region  jointure et choix de la langue
//                    /*jointure et choix de la langue*/
//                    //recap	
//                    if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {		
//                        //Annonceurs			
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser="+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser";
//                        beginbyand=false;
//                    }
			
			
//                    //famille (sector)	
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+="   "+DBConstantes.Tables.SECTOR_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();								
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector="+DBConstantes.Tables.RECAP_PREFIXE+".id_sector";
//                        beginbyand=false;
//                    }

//                    //Produits
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList) || WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)) {
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+="   "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product="+DBConstantes.Tables.RECAP_PREFIXE+".id_product";
//                        beginbyand=false;
//                    }
//                    #endregion
			
//                    #region selections
//                    //Cas du vehicle dans la table pluri
//                    string list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                        sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, !beginbyand);
//                        beginbyand=false;
//                    }
//                    // V�rifie s'il a toujours les droits pour acc�der aux donn�es de ce Vehicle
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                        sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//                    }
//                    //selection des categories et/ou supports			
//                    if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList) || WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
//                        sql+="   "+WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,!beginbyand);
//                        beginbyand=false;
//                    }
//                    //Droits Parrainage TV
//                    if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                        if (!beginbyand) sql += " and ";
//                        sql += "    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                    }

//                    if(withRights){
//                        //selection des annonceurs (advertiser) 
//                        if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList) && withAdvertisers){
//                            sql+="  and  "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser in "+"("+AdvertiserAccessList+")";
//                            beginbyand=false;
//                        }				
//                        //Si la requ�te ne porte pas sur le total famille ou martch�
//                        if(withRights){					
//                            #region S�lection de Produits
//                            // S�lection en acc�s
//                            premier=true;
//                            // group					
//                            if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                                if(!beginbyand)sql+=" and ";
//                                sql+=" (( "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                                premier=false;
//                                beginbyand=false;
//                            }
//                            // Segment					
//                            if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                                if(!premier) sql+=" or";
//                                else {
//                                    if(!beginbyand)sql+=" and ((";
//                                    else sql+=" ((";
//                                }
//                                sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                                premier=false;
//                                beginbyand=false;
//                            }			
//                            if(!premier) sql+=" )";
		
//                            // S�lection en Exception
//                            // group					
//                            if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                                if(premier) {
//                                    if(!beginbyand)sql+=" and ( ";
//                                    else sql+=" ( ";
//                                }
//                                else if(!beginbyand) sql+=" and ";
//                                sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                                premier=false;
//                                beginbyand=false;
//                            }
//                            // segment en Exception					
//                            if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                                if(premier) {
//                                    if(!beginbyand)sql+=" and ( ";
//                                    else sql+=" ( ";
//                                }
//                                else if(!beginbyand) sql+=" and ";
//                                sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                                premier=false;
//                                beginbyand=false;
//                            }
//                            if(!premier) sql+=" )";
		
//                            #endregion				
//                        }	
									
//                    }
//                    //Total famille
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        //listVehicle=getSector(recapTableName,GroupAccessList,SegmentAccessList);
//                        if(WebFunctions.CheckedText.IsStringEmpty(listVehicle.ToString().Trim())){
//                            sql+=(!beginbyand)?" and ": " ";
//                            sql+="  "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector in ( ";
//                            sql+=listVehicle;
//                            sql+=" ) ";
//                            beginbyand=false;
//                        }
//                    }
//                    #endregion
			
//                    #region Nomenclature Produit (droits)
//                    //Seulement sur l'univers produit
//                    if(withRights && WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                        //Droits produits
//                        sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);
//                        beginbyand=false;
//                    }
//                    #endregion

//                    #endregion

//                    #region regroupement
//                    //Regroupement des annonceurs			
//                    sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    firstgrouby=false;
		
//                    //Regroupement des familles
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        sql+=(!firstgrouby)?",":" group by";
//                        sql+="  "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector";
//                        firstgrouby=false;
//                    }	
		
//                    //Regroupement des produits
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList) || WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)) {
//                        sql+=(!firstgrouby)?",":" group by";
//                        sql+="  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product";
//                        firstgrouby=false;
//                    }						
//                    #endregion

//                    #region tri
//                    // Tri des annonceurs			
//                    sql+=" order by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    firstorderby=false;
			
	
//                    //Tri familles de produits
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                        sql+=(!firstorderby)?",":" order by";
//                        sql+="  "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector";
//                        firstorderby=false;
//                    }
			
//                    //Tri des produits
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList) || WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)) {
//                        sql+=(!firstorderby)?",":" order by";
//                        sql+="  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product";
//                        firstorderby=false;
//                    }
						
//                    #endregion
		
//                    #endregion
		
//                    #region Execution de la requ�te
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour le tableau saisonalit�: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
//            return(ds);
//        }

//        #endregion

//        #region DataSet Saisonnalit� pour sortie graphique
//        /// <summary>
//        /// Fournit la liste des annonceurs,des r�f�rences, les investissements mensuels (p�riode N et N-1)
//        /// pour indicateur Saisonnalit�, sur le total supports de l'univers ou march� ou famille.
//        /// </summary>
//        /// <remarks>Cette m�thode est utilis�e pour la pr�sentation des donn�es sous forme de graphique.</remarks>
//        /// <param name="webSession">Session du client</param>	
//        /// <param name="withAdvertisers">champs annonceurs</param>	
//        /// <param name="withRights">droits produits</param>
//        /// <returns>Donn�es de saisonnalit� (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifi� le 30/11/04</history>
//        public static DataSet GetChartData(WebSession webSession, bool withAdvertisers, bool withRights) {	
			
//            #region variables
//            //Lib�ll� de la table "recap" cible
//            string recapTableName="";
//            //boll�en pour construire la requ�te sql
//            bool premier=true;
//            bool firstfield=true;
//            //bool firstgrouby=true;
//            //bool firstorderby=true;
//            bool beginbyand =true;		
//            //Liste des annonceurs concurrents s�lectionn�s
//            string CompetitorAdvertiserAccessList="";
//            //Liste des familles poss�dant au moins un groupe ou une vari�t� s�lectionn�e
//            string listVehicle="";
//            //Pour stocker les donn�es g�n�r�s par la requ�te
//            DataSet ds =null;
//            //Indique si la requ�te sql doit �tre construite et ex�cit�e
//            bool buildSqlStatement=true;
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s		
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);		
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
			
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName.ToString().Trim())){
//                /*Liste des familles ayant au moins une vari�t� ou un groupe s�lectionn�.
//                 Cette donn�e est n�cesssaire pour calculer le total famille des produits s�lectionn�s.*/
//                if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                    listVehicle=WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(!WebFunctions.CheckedText.IsStringEmpty(listVehicle))buildSqlStatement=false;
//                }
//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName) && buildSqlStatement){
//                    #region Mise en forme des mois de la p�riode s�lectionn�e					
//                    string StudyMonths=IntervalMonthsStudy(webSession,false);
//                    #endregion
		
//                    #region Construction de la requ�te

//                    #region Close Select
//                    string sql="select";
			
//                    #region  selection annonceurs de r�f�rences et concurrents
//                    if(withAdvertisers){
//                        //selection des annonceurs de r�f�rences en acc�s			
//                        sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";					
//                        firstfield=false;	
//                    }
//                    #endregion

//                    #region  selection des investissement mensuels
//                    //selection des investissement mensuels
//                    if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)) {
//                        if(!firstfield)sql+=",";
//                        sql+="  "+StudyMonths+" ";
//                        firstfield=false;
//                    }		
//                    #endregion

//                    sql+=" from";

//                    #region Tables concern�es par la requete
//                    //TABLE RECAP
//                    if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+"";
//                        premier=false;
//                    }			
//                    //TABLE ADVERTISER	
//                    if(withAdvertisers){
//                        if(!premier)sql+=",";
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+"";
//                        premier=false;
//                    }				
//                    #endregion

//                    #endregion	

//                    #region Close Where
//                    sql+=" Where";

//                    #region  jointure et choix de la langue
//                    /*jointure et choix de la langue*/
////					//recap						
////					sql+=(!beginbyand)?" and ": " ";
////					// Langue				
////					sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage.ToString();				
////					beginbyand=false;
//                    if(withAdvertisers){
//                        //Annonceurs			
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser="+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser";
//                        beginbyand=false;
//                    }										
//                    #endregion
			
//                    #region s�lections
//                    //Cas du vehicle dans la table pluri
//                    string list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                        sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, !beginbyand);
//                        beginbyand=false;
//                    }
//                    // V�rifie s'il a toujours les droits pour acc�der aux donn�es de ce Vehicle
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                        sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);	
//                        beginbyand=false;
//                    }
//                    //selection des categories et/ou supports			
//                    if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList) || WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
//                        sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,!beginbyand);
//                        beginbyand=false;
//                    }
//                    //Droits Parrainage TV
//                    if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                        if (!beginbyand) sql += " and ";
//                        sql += "    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                    }
//                    //selection des annonceurs (advertiser) 
//                    if(withAdvertisers && WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList) && withAdvertisers){
//                        if(!beginbyand)sql+=" and ";
//                        sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser in "+"("+AdvertiserAccessList+")";
//                        beginbyand=false;
//                    }	
					
//                    //Si la requ�te ne porte pas sur le total famille ou march�
//                    if(withRights){					
//                        #region S�lection de Produits
//                        // S�lection en acc�s
//                        premier=true;
//                        // group					
//                        if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                            if(!beginbyand)sql+=" and ";
//                            sql+=" (( "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        // Segment					
//                        if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                            if(!premier) sql+=" or";
//                            else {
//                                if(!beginbyand)sql+=" and ((";
//                                else sql+=" ((";
//                            }
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }			
//                        if(!premier) sql+=" )";
		
//                        // S�lection en Exception
//                        // group					
//                        if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                            if(premier) {
//                                if(!beginbyand)sql+=" and ( ";
//                                else sql+=" ( ";
//                            }
//                            else if(!beginbyand) sql+=" and ";
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        // segment en Exception					
//                        if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                            if(premier) {
//                                if(!beginbyand)sql+=" and ( ";
//                                else sql+=" ( ";
//                            }
//                            else if(!beginbyand) sql+=" and ";
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        if(!premier) sql+=" )";
		
//                        #endregion				
//                    }									
//                    //Total famille
//                    if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 						
//                        if(WebFunctions.CheckedText.IsStringEmpty(listVehicle.ToString().Trim())){
//                            if(!beginbyand)sql+=" and ";
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector in ( ";
//                            sql+=listVehicle;
//                            sql+=" ) ";
//                            beginbyand=false;
//                        }
//                    }
//                    #endregion
			
//                    #region Nomenclature Produit (droits)
//                    //Seulement sur l'univers produit
//                    if(withRights && WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                        //Droits produits
//                        sql+=WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);
//                        beginbyand=false;
//                    }
//                    #endregion

//                    #endregion

//                    #region regroupement
//                    if(withAdvertisers){
//                        //Regroupement des annonceurs			
//                        sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
						
//                    }					
//                    #endregion

//                    #region tri
//                    if(withAdvertisers){
//                        // Tri des annonceurs			
//                        sql+=" order by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                        //firstorderby=false;
//                    }						
//                    #endregion
		
//                    #endregion
		
//                    #region Execution de la requ�te
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour le graphique saisonalit�: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
//            return(ds);
//        }

//        #endregion

//        #region Dataset Saisonnalit� - POUR TOTAUX MARCHES (AVEC PROCEDURES STOCKEES)
//        /// <summary>
//        /// Pour le total march� chaque ligne de la table de donn�es recup�r�e correspond
//        ///� l'investissement mensuel,evolution par rapport au mois 
//        ///de la p�rode N-1,nombre de r�f�rences et le budget moyen ,1er annonceur avec son
//        ///investissement et SOV,1 ere r�f�rence avec son investissement et SOV.</summary>
//        ///<remarks> Traitement distinct du total famille pour optimiser les temps de r�ponses
//        /// par utilisation de proc�dures stock�es</remarks>
//        /// <param name="webSession">Session du client</param>				
//        /// <returns>Donn�es de saisonnalit� (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifi� le 24/11/04</history>
//        public static DataSet GetSeasonalityData(WebSession webSession) {	
//            #region variables
//            //annonceur concorrents en acc�s
//            string CompetitorAdvertiserAccessList="";
//            //groupe de donn�es
//            DataSet ds = null; 
//            //est ce 1er terme requete
//            bool premier=true;	
//            //liste des mois �tudi�s p�riode N		
//            ArrayList StudyMonthListArr=null;
//            //liste des mois �tudi�s p�riode N-1
//            ArrayList previousYearStudyMonthsListArr=null;
//            //liste des mois �tudi�s Ann�e actuelle 
//            ArrayList currentYearStudyMonthsListArr=null;
//            //mois �tudi�(s)
//            string StudyMonths="";
//            //mois �tudi�(s) pour la requete 2
//            string StudyMonthsForRequest2="";
//            //mois �tudi�s Ann�e pr�c�dente
//            string previousYearStudyMonths="";
//            //mois �tudi�s Ann�e actuelle 
//            string currentYearStudyMonths="";
//            //premier champ
//            bool firstfield=true;
//            //comence par "And"
//            bool beginbyand =true;
//            //Nom de la table recap
//            string recapTableName="";
//            //Chaine SQL
//            string sql="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion

			
//            #region Identification table recap
//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
//            #endregion

//            #region Mise en forme des mois de la p�riode s�lectionn�e
//            //liste mois p�riode N-1 et/ou N
//            StudyMonths=IntervalMonthsStudy(webSession,true);
//            //liste mois p�riode (alias)
//            StudyMonthsForRequest2=ListMonthsStudy(webSession,false,1,true);
			
//            if(webSession.ComparativeStudy){				
//                //liste mois p�riode N
//                currentYearStudyMonths=ListMonthsStudy(webSession,false,1,true);				
//                //liste mois p�riode N-1
//                previousYearStudyMonths=ListMonthsStudy(webSession,true,1,true);
//            }
//            #endregion			

//            #region Construction de la requ�te
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                #region Requete 2
//                /* Requete 2 : Obtient pour chaque mois : l'annonceur, budget moyen,evolution,nombre de r�f�rences
//                 * � partir des donn�es fournies par la "requete 1".
//                 * */
//                sql="select";

//                #region  selection annonceurs de r�f�rences et concurrents
//                //s�lection des annonceurs de r�f�rences en acc�s			
//                sql+="  id_advertiser,advertiser,nbRef ";
//                firstfield=false;
//                #endregion
//                //On r�cupere la chaine de caract�re repr�sentant chaque mois
//                try{
//                    if(WebFunctions.CheckedText.IsStringEmpty(StudyMonthsForRequest2)){
//                        StudyMonthListArr = new ArrayList(StudyMonthsForRequest2.Split(','));
//                        if(StudyMonthListArr!=null && StudyMonthListArr.Count >0){
//                            foreach (string month in StudyMonthListArr){
//                                if(WebFunctions.CheckedText.IsStringEmpty(month)){
//                                    if(!firstfield)sql+=",";
//                                    sql+=" "+month;
//                                    firstfield=false;
//                                    if(!firstfield)sql+=",";
//                                    //Calcul des budgets moyen pour chaque mois
//                                    sql+=" round((request1."+month+"/nbref)) as  budget_moyen_"+month.ToString().Trim()+" ";
//                                    firstfield=false;	
//                                }
//                            }
//                            try{
//                                //Calcul des evolutions pour chaque mois
//                                if(webSession.ComparativeStudy){
//                                    if(WebFunctions.CheckedText.IsStringEmpty(currentYearStudyMonths))
//                                        currentYearStudyMonthsListArr = new ArrayList(currentYearStudyMonths.Split(','));
//                                    if(WebFunctions.CheckedText.IsStringEmpty(previousYearStudyMonths))
//                                        previousYearStudyMonthsListArr = new ArrayList(previousYearStudyMonths.Split(','));
//                                    if(currentYearStudyMonthsListArr!=null && previousYearStudyMonthsListArr!=null && currentYearStudyMonthsListArr.Count >0 && previousYearStudyMonthsListArr.Count>0 && currentYearStudyMonthsListArr.Count==previousYearStudyMonthsListArr.Count){
//                                        for (int ev=0;ev<currentYearStudyMonthsListArr.Count;ev++){																		
//                                            if(!firstfield)sql+=",";
//                                            sql+="decode(request1."+previousYearStudyMonthsListArr[ev].ToString()+",0,-1,ROUND(((request1."+currentYearStudyMonthsListArr[ev].ToString()+"/request1."+previousYearStudyMonthsListArr[ev].ToString()+")*100)-100,0)) as evol";
//                                            firstfield=false;									
//                                        }
//                                    }									
//                                }
//                            }catch(Exception evolErr){
//                                throw (new SectorAnalysisIndicatorDataAccessException(GestionWeb.GetWebWord(1363,webSession.SiteLanguage) + ": "+evolErr.Message));
//                            }
//                        }
//                    }
//                }catch(Exception monthErr){
//                    throw (new SectorAnalysisIndicatorDataAccessException(GestionWeb.GetWebWord(1364,webSession.SiteLanguage) + " : "+monthErr.Message));
//                }
//                sql+=" from (";							
				
//                #region Requete 1
//                /* Requete 1 : Obtient pour chaque mois : l'annonceur, budget moyen,evolution, nombre de r�f�rences.				
//                 * */
//                //Debut Requete 1
//                #region Close Select
//                firstfield=true;
//                sql+="select";

//                #region  Champs annonceurs de r�f�rences et concurrents
//                //selection des annonceurs de r�f�rences en acc�s			
//                sql+="  distinct "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//                //Nombre de r�f�rences
//                sql+=", count("+DBConstantes.Tables.RECAP_PREFIXE+".id_product) as nbRef ";				
//                firstfield=false;				
//                #endregion

//                #region  Champs des investissement mensuels
//                // selection des investissement mensuels
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)) {
//                    if(!firstfield)sql+=",";
//                    sql+="  "+StudyMonths+" ";
//                    firstfield=false;
//                }		
//                #endregion
				
//                sql+=" from";

//                #region Tables concern�es par la requete
//                //TABLE RECAP
//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                    sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+"";
//                    premier=false;
//                }
				
//                //TABLE ADVERTISER			
//                if(!premier)sql+=",";
//                sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+"";
//                premier=false;											
								
//                #endregion

//                #region Close Where
//                sql+=" Where";

//                #region  jointure et choix de la langue
				
//                //Annonceurs			
//                sql+=(!beginbyand)?" and ": " ";
//                // Langue
//                sql+="   "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                // jointure
//                sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser="+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser";
//                beginbyand=false;																			
//                #endregion
			
//                #region selections
//                // cas du vehicle dans la table pluri
//                string list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, !beginbyand);
//                    beginbyand=false;
//                }
//                // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);	
//                    beginbyand=false;
//                }
//                //Si la requ�te ne porte pas sur le total march�			
//                //selection des categories et/ou supports
//                if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList) || WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
//                    sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,!beginbyand);
//                    beginbyand=false;
//                }
//                //Droits Parrainage TV
//                if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                    if (!beginbyand) sql += " and ";
//                    sql += "    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                }
//                //selection des annonceurs (advertiser) 
//                if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
//                    sql+=(!beginbyand)?" and ": " ";
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser in "+"("+AdvertiserAccessList+")";
//                    beginbyand=false;
//                }				
//                //Si la requ�te ne porte pas sur le total famille ou martch�																					
//                #region S�lection de Produits
//                // S�lection en acc�s
//                premier=true;
//                // group					
//                if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                    if(!beginbyand)sql+=" and ";
//                    sql+=" (( "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                    premier=false;
//                    beginbyand=false;
//                }
//                // Segment					
//                if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                    if(!premier) sql+=" or";
//                    else {
//                        if(!beginbyand)sql+=" and ((";
//                        else sql+=" ((";
//                    }
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                    premier=false;
//                    beginbyand=false;
//                }			
//                if(!premier) sql+=" )";
		
//                // S�lection en Exception
//                // group					
//                if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                    if(premier) {
//                        if(!beginbyand)sql+=" and ( ";
//                        else sql+=" ( ";
//                    }
//                    else if(!beginbyand) sql+=" and ";
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                    premier=false;
//                    beginbyand=false;
//                }
//                // segment en Exception					
//                if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                    if(premier) {
//                        if(!beginbyand)sql+=" and ( ";
//                        else sql+=" ( ";
//                    }
//                    else if(!beginbyand) sql+=" and ";
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                    premier=false;
//                    beginbyand=false;
//                }
//                if(!premier) sql+=" )";
		
//                #endregion				
							
//                #endregion
			
//                #region Nomenclature Produit (droits)
//                //Seulement sur l'univers produit				
//                //Droits produits
//                sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);
			
//                #endregion

//                #region regroupement
//                //Regroupement des annonceurs			
//                sql+=" group by "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";																											
//                #endregion

//                #endregion

//                #endregion				
//                #endregion
//                sql+=" ) request1";
//                #endregion
//            }			
//            #endregion

//            if(WebFunctions.CheckedText.IsStringEmpty(sql.ToString().Trim())){
//                #region Execution de la requ�te
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour le tableau saisonalit�: "+sql,err));
//                }
//                #endregion				
//            }

//            return(ds);
//        }
//        #endregion

//        #endregion			

//        #region DataSet Evolution
//        /// <summary>
//        /// Fournit le dataset pour le palmares
//        /// </summary>
//        /// <param name="webSession">Session client</param>		
//        /// <param name="tableType">Type de table</param>
//        /// <returns>dataset pour le palmares</returns>
//        public static DataSet GetEvolutionData(WebSession webSession,FrameWorkConstantes.EvolutionRecap.ElementType tableType){
			
//            #region Variables
//            DataSet ds=null;
//            // Table recap appel�
//            string recapTableName="";
//            // Mois �tudi�s
//            string StudyMonths="";
//            bool premier;
//            string sql="";
//            string list="";
////			string list2="";
//            string VehicleAccessList ="";
//            #endregion

//            #region Construction de la requ�te
			
//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
			
//            StudyMonths=EvolutionIntervalMonthsStudy(webSession);

//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.EvolutionRecap.ElementType.product){
//                sql+=" select "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser, "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product, "+DBConstantes.Tables.PRODUCT_PREFIXE+".product,  ";
//            }else{
//                sql+=" select "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser, ";
//            }
//                sql+=StudyMonths;
		
				
//            sql+=" from ";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" , " ;
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+" ,";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+"";

//            sql+=" where  "; 
//            sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_product="+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product";
//            sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser="+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser";
			
////			sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage+" ";


//            // S�lection de Produits
//            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
//                sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.RECAP_PREFIXE, true);						


	
//            #region s�lection des m�dias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//            }
//            // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//            }
//            //Droits Parrainage TV
//            if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += "   and " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//            }

//            sql+=WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess),true);

//            #endregion

//            #region Nomenclature Produit (droits)		
//            sql+=WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,true);
//            #endregion

//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.EvolutionRecap.ElementType.product){
//                sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".product ";
//            }else{
//                sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//            }
			
//            sql+=" order by  Ecart desc ";

			
			

//            #endregion

//            #region Execution de la requ�te
//            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//            try{
//                ds=dataSource.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour les evolution: "+sql,err));
//            }
//            #endregion			

//            return(ds);
		
//        }

//        #endregion

//        #region DataSet Palmares
		
//        /// <summary>
//        /// Fournit le dataset pour le palmares
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="typeYear">type d'ann�e</param>
//        /// <param name="tableType">type de table</param>
//        /// <returns>donn�es palmares</returns>
//        public static DataSet GetPalmaresData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear,FrameWorkConstantes.PalmaresRecap.ElementType tableType){
			
//            #region Variables
//            DataSet ds=null;
//            // Table recap appel�
//            string recapTableName="";
//            // Mois �tudi�s
//            string StudyMonths="";
			
//            bool premier;
//            string sql="";
//            string list="";
////			string list2="";
//            string VehicleAccessList ="";
//            #endregion

//            #region Construction de la requ�te
			
//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
			
//            StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);
//            //s�lection des 10 plus grand palmares

//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                sql+=" select "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser, "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product, "+DBConstantes.Tables.PRODUCT_PREFIXE+".product,  ";
//            }else{
//                sql+=" select "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser, ";
//            }
//            sql+=StudyMonths;
		
				
//            sql+=" from ";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" , " ;
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+" ,";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+"";

//            sql+=" where  "; 
//            sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_product="+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product";
//            sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser="+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser";
			
////			sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage+" ";

//            // S�lection de Produits
//            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
//                sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.RECAP_PREFIXE, true);						

//            #region s�lection des m�dias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//            }
//            // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//            }
//            //Droits Parrainage TV
//            if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += "  and  " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//            }
//            sql+=WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess),true);

//            #region Nomenclature Produit (droits)		
//            sql+=WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,true);
//            #endregion

//            #endregion

//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".product ";
//            }else{
//                sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//            }
//            if(typeYear==FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear)
//                sql+=" order by  total_N desc ";
//            else{
//                sql+=" order by  total_N1 desc ";				
//            }
			
//            if(webSession.ComparativeStudy && typeYear==FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear){
//                sql+=", total_N1 desc";
//            }
//            //fin s�lection des 10 plus grand palmares
//            #endregion

//            #region Execution de la requ�te
//            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//            try{
//                ds=dataSource.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour les palmares: "+sql,err));
//            }
//            #endregion			

//            return(ds);
//        }
		
//        #endregion		

//        #region DataSet Nouveaut�s
//        /// <summary>
//        /// Cr�e le tableau de r�sultats qui permettra de d�tecter les r�els nouveaux produits
//        /// ou annonceurs des d�marrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le d�but de l'ann�.		
//        /// </summary>
//        /// <remarks>Il fournit les donn�es  pour d�terminer les nouveaux annonceurs ou les nouvellles r�f�rences.
//        /// </remarks>
//        /// <example>
//        /// -Pour les nouvelles r�f�rences l'appel est le suivant :
//        /// <code>dsNovelty=IndicatorDataAccess.getNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,true);</code>
//        /// -Pour les nouveaux annonceurs l'appel est le suivant :
//        /// <code> dsNovelty=IndicatorDataAccess.getNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,true);</code>
//        /// </example>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">produit ou annonceur</param>
//        /// <param name="onAdvertiser">la requ�te porte sur les annonceurs</param>
//        /// <param name="onProduct">la requ�te porte sur les produits</param>
//        /// <param name="TotalType"></param>
//        /// <returns>donn�es pour nouveaut�s</returns>
//        ///<history>[D. V. Mussuma]  Modifi� le 24/11/04</history>
//        public static DataSet GetNoveltyData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,bool onAdvertiser, bool onProduct,bool TotalType){
			
//            #region variables
//            //Lib�ll� table recap
//            string recapTableName="";
//            //est ce premier terme de la requ�te
//            bool premier=true;
//            //est ce premier champ de la requ�te
//            bool firstfield=true;
//            //condition commence par "AND"		
//            bool beginbyand =true;
//            //liste d'�l�ments
//            string list;
//            //Mois �tudi�s
//            string NoveltyIntervalMonth="";
//            //mois courant
//            string currentMonth="";
//            //Liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //Groupe de donn�es
//            DataSet ds =null;
//            //index ann�e N	
//            string YearSelected="";
//            //index ann�e N
//            int year=0;

//            #endregion

//            #region periode etudi�e
//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);			
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);	
//            #endregion

			
//            if(HasData(PeriodEndDate)){
//                #region construction des listes de produits, media, annonceurs s�lectionn�s
//                //listes de produits, media, annonceurs s�lectionn�s							
//                DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//                string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//                CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//                string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//                string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//                string GroupAccessList = recapUniversSelection.GroupAccessList;
//                string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//                string MediaAccessList = recapUniversSelection.MediaAccessList;
//                string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//                string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//                #endregion

			
//                #region Table recap cible
//                //Determine la table recap � appeler
//                if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                    recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//                else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
//                #endregion

//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                    #region Construction de la requ�te

//                    #region Clause Select
//                    #region requ�te 2 : recup�re les annonceurs ou les nouveaux produits
//                    //Pour r�cup�rer les nouveaux �l�ments on v�rifie que l'investissement depuis
//                    //le d�but de l'ann�e est nul except� pour le mois en cours
//                    string sql="select ";
//                    if(onAdvertiser){
//                        if(!firstfield)sql+=",";
//                        sql+=" id_advertiser,advertiser";
//                        firstfield=false;
//                    }
//                    if(onProduct){
//                        sql+="id_product,product,id_advertiser";
//                        firstfield=false;
//                    }					
//                    //investissements mensuels, mois courant
//                    currentMonth = WebFunctions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
//                    if(WebFunctions.CheckedText.IsStringEmpty(currentMonth)){
//                        if(!firstfield)sql+=",";
//                        sql+=" "+currentMonth;
//                        firstfield=false;
//                    }
//                    if(webSession.ComparativeStudy){
//                        if(!firstfield)sql+=",";
//                        sql+=StringMonthsComparativeStudy(webSession);
//                        firstfield=false;
//                    }
//                    //Somme des investissements des mois pr�c�dents jusqu'au d�but de l'ann�e
//                    //Si la somme est �gale � zero alors le produit ou l'annonceur est nouveau									
//                    NoveltyIntervalMonth = NoveltyIntervalMonthsStudy(webSession,TotalType);
//                    if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth)){
//                        if(!firstfield)sql+=",";
//                        sql+=" isInactive ";
//                    }
					
//                    firstfield=true;
//                    sql+=" from ( ";
//                    #region requ�te 1 : s�lection annonceurs et produits 
//                    sql+="select";

//                    #region  selection annonceurs de r�f�rences et concurrents et/ou selection produits
//                    //selection des annonceurs de r�f�rences en acc�s
//                    if(onAdvertiser){
//                        if(!firstfield)sql+=",";
//                        sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                        firstfield=false;
//                    }
//                    else if(onProduct){
//                        if(!firstfield)sql+=",";
//                        sql+="  "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser ";
//                        firstfield=false;
//                    }												
//                    #endregion

//                    #region s�lections pour la p�riode d'�tude
//                    #region ann�e N
//                    //Investissement mois
//                    if(!firstfield)sql+=",";					
//                     // mois actuel
//                    if(PeriodEndDate.Month==DateTime.Now.Month)
//                    sql+="  "+" sum(exp_euro_N_"+PeriodEndDate.AddMonths(-1).Month.ToString()+") "+WebFunctions.Dates.GetMonthAlias(PeriodEndDate.Month-1,WebFunctions.Dates.yearID(PeriodEndDate.AddMonths(-1).Date,webSession),3,webSession)+" ";
//                    else sql+="  "+" sum(exp_euro_N"+YearSelected+"_"+PeriodEndDate.Month.ToString()+") "+WebFunctions.Dates.GetMonthAlias(PeriodEndDate.Month,WebFunctions.Dates.yearID(PeriodEndDate,webSession),3,webSession)+" ";
//                    //autre mois

//                    //Somme des investissements des mois pr�c�dents jusqu'au d�but de l'ann�e
//                    //Si la somme est �gale � zero alors le produit ou l'annonceur est nouveau														
//                    if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth)){
//                        if(!firstfield)sql+=",";
//                        sql+=NoveltyIntervalMonth;
//                        firstfield=false;
//                    }
					

//                    #endregion
//                    #endregion
				
//                    #region Tables concern�es par la requete
//                    sql+=" from ";
//                    sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" , " ;
//                    sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+" ,";
//                    sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+"";

//                    sql+=" where  "; 
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_product="+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product";
//                    sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser="+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser";
			
////					sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//                    sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//                    sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage+" ";								
//                    beginbyand=false;
//                    #endregion
													
//                    #region S�lection de Produits
//                        // S�lection en acc�s
//                        premier=true;
//                        // group					
//                        if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                            if(!beginbyand)sql+=" and ";
//                            sql+=" (( "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        // Segment					
//                        if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                            if(!premier) sql+=" or";
//                            else {
//                                if(!beginbyand)sql+=" and ((";
//                                else sql+=" ((";
//                            }
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }			
//                        if(!premier) sql+=" )";
		
//                        // S�lection en Exception
//                        // group					
//                        if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                            if(premier) {
//                                if(!beginbyand)sql+=" and ( ";
//                                else sql+=" ( ";
//                            }
//                            else if(!beginbyand) sql+=" and ";
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        // segment en Exception					
//                        if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                            if(premier) {
//                                if(!beginbyand)sql+=" and ( ";
//                                else sql+=" ( ";
//                            }
//                            else if(!beginbyand) sql+=" and ";
//                            sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                            premier=false;
//                            beginbyand=false;
//                        }
//                        if(!premier) sql+=" )";
		
//                        #endregion				
					
//                    #region s�lection des m�dias
//                    list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                        sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                    }
//                    // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                        sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//                    }
//                    //Droits Parrainage TV
//                    if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                        if (!beginbyand) sql += " and ";
//                        sql += "    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                    }
//                    sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess),true);
//                    #endregion

//                    #region Nomenclature Produit (droits)
//                    //Seulement sur l'univers produit
////					if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                        //Droits produits
//                        sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,true);
////					}
//                    #endregion
				
//                    #region regroupement
//                    if(onProduct){
//                        sql+=" group by "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".product,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser ";
//                    }else{
//                        sql+=" group by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//                    }
//                    #endregion

//                    #region tri
//                    if(onProduct){
//                        sql+=" order by "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product,  "+DBConstantes.Tables.PRODUCT_PREFIXE+".product,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser ";
//                    }else{
//                        sql+=" order by "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser,  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//                    }					
//                    #endregion

//                    #endregion
//                    sql+=" ) ";
//                    if(TotalType){//TotalType==false permet le calcul du total univers
//                        if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth) || WebFunctions.CheckedText.IsStringEmpty(currentMonth))
//                        sql+=" where  "; 
//                        if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth)){
//                            sql+=" isInactive = 0 "; 
//                            beginbyand=false;
//                        }
//                        if(WebFunctions.CheckedText.IsStringEmpty(currentMonth)){						
//                            if(!beginbyand)sql+=" and ";
//                            sql+="  "+currentMonth+" > 0";
//                        }
//                    }
			
//                    #endregion
//                    #endregion				
					
//                    #endregion

//                    #region Execution de la requ�te
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour les nouveaut�s: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
			
//            return(ds);
//        }
//        #endregion

//        #region DataSet(s) Strat�gie M�dia

//        #region Strat�gie M�dia Calcule Investissements par m�dia
//        /// <summary>
//        /// Charge les donn�es pour cr�er la pr�sentation graphique de strat�gie m�dia (analsyse sectorielle).
//        /// Recup�re la r�partition m�dia sur le total de la p�riode 
//        ///contenant les �l�ments ci-apr�s :
//        ///en ligne :
//        ///l'investissement total famille ou march� ou univers  				
//        ///de la p�riode N et N-1		
//        ///d�clin� par media, cat�gorie ,supports, annonceur.
//        /// </summary>
//        ///<remarks>
//        /// Elle est utilis�e aussi bien que pour remonter les donn�es du total univers, march�, famille
//        ///que pour celles des annonceurs de r�f�rences ou concurrents.
//        ///Exemples d'appel en fonction du type de donn�es cibl�es :
//        ///</remarks>
//        /// <example>-Pour recup�rer les investissements des annonceurs de r�f�rences et concurrents sur la p�riode N et N-1
//        /// <code>dsAdvertiser = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,true);
//        /// </code>
//        /// -Pour recup�rer les investissements pour les totaux univers par media,cat�gorie, support sur la p�riode N et N-1
//        /// <code>dsTotalUniverse = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,false);</code>
//        /// -Pour recup�rer les investissements pour les totaux march�s ou famille par media,cat�gorie, support sur la p�riode N et N-1
//        /// <code>dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.marketTotal,false);</code>
//        /// </example>		
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">produit ou annonceur</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison</param>
//        /// <param name="RefenceOrCompetitorAdvertiser">calcul pour annonceur de r�f�rences ou concurrents</param>
//        /// <returns>groupe de donn�es pour strat�gie m�dia</returns>
//        /// <history>[D. V. Mussuma]  Modifi� le 28/01/05</history>
//        public static DataSet getMediaStrategyData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,bool RefenceOrCompetitorAdvertiser){
			
//            #region variables
//            //lib�ll� table recap
//            string recapTableName="";
//            // Mois �tudi�s
//            string StudyMonths="";
//            //Est ce premier membre de la requ�te
//            bool premier=true;						
//            //Est ce premier membre de la requ�te � trier
//            bool firstorderby=true;			
//            //annonceur concurrents
//            string CompetitorAdvertiserAccessList="";
//            //groupe de donn�es
//            DataSet ds = null;
//            //Chaine sql
//            string sql="";			
//            //index pour alias sous requete
//            string indexTb0="1";
//             bool buildStatement=true;
//            string totalSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion					

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException (GestionWeb.GetWebWord(1372,webSession.SiteLanguage)));						
			
//            //V�rifie si poss�de les donn�es pour les familles s�lectionn�es
//            if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                totalSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                if(!WebFunctions.CheckedText.IsStringEmpty(totalSector))
//                    buildStatement=false;
//            }

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName) && buildStatement){
//                //Periode de l'�tude
//                #region periode etudi�e
//                //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fr�quence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
//                //Mois concern�s
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);
//                #endregion

//                #region construction de la requ�te																		
//                #region REQUETE 2				
//                /* Table REQUETE 2 (requete principal) :	
//                 Recup�re la r�partition m�dia sur le total de la p�riode 
//                 contenant les �l�ments ci-apr�s :
//                 en ligne :
//                 l'investissement total famille ou march� ou univers  				
//                 de la p�riode N et N-1		
//                 d�clin� par media, cat�gorie ,supports, annonceur.
//                 */
//                //Niveau famille
//                sql+=" select ";
//                premier=false;				
//                //Champs m�dia
//                sql+=GetMediaStrategyFieldsForMediaDetail(webSession,"maxt","");				
//                //Champs produits
//                sql+=GetProductFieldsForMediaStrategy(tableType,"","maxt","maxt",false,".");
//                //investissements sur p�riode N et N-1
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=" maxt.total_N ";				
//                    if(webSession.ComparativeStudy){
//                        if(!premier)sql+=" ,";
//                        sql+=" maxt.total_N1 ";
//                    }
//                    premier=false;
//                }
//                sql+=" from (";
//                #region REQUETE 1 : r�partition m�dia sur le total de la p�riode et par annonceur
//                    sql+=MediaStrategySubQuery(webSession,recapTableName,comparisonCriterion,tableType,StudyMonths,indexTb0,GroupAccessList,GroupExceptionList,SegmentAccessList,SegmentExceptionList,CategoryAccessList,MediaAccessList,AdvertiserAccessList,RefenceOrCompetitorAdvertiser);
//                #endregion
//                sql+=" ) maxt,"; //Table gen�r�e par la requete 1 :r�partition m�dia sur le total de la p�riode et par annonceur
//                //Tables m�dias
//                sql+=GetMediaStrategyTablesForMediaDetail(webSession);

//                sql+=" where ";

//                #region jointure et choix de la langue
//                //jointure et choix de la langue
//                sql+="  "+GetMediaStrategyJointForMediaDetail(webSession,"maxt");					
//                #endregion											

//                #region tri des enregistrements 
//                //tri
//                sql+=" order by ";
//                //tri des m�dias
//                sql+=GetMediaStrategyOrderForMediaDetail(webSession,"maxt","asc",true);
//                firstorderby=false;				
//                //tri des produits
//                if(!firstorderby)sql+=" ,";
//                sql+=GetProductByOrderForMediaStrategy(tableType,"maxt","asc");
//                #endregion

//                #endregion											

//                #endregion

//                #region Execution de la requ�te
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    return(dataSource.Fill(sql.ToString()));
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour start�gie media: "+sql,err));
//                }
//                #endregion	
//            }
			

//            return (ds);
//        }
//        #endregion
		
//        #region Strat�gie m�dia premier annonceur ou r�f�rence
//        /// <summary>
//        /// Goupe de donn�es qui fournit les premiers annonceurs de r�f�rences ou concurrents par m�dia
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table g�n�rer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison (univers ou march�  ou famille)</param>
//        /// <param name="mediaLevel">niveau m�dia (m�dia ou cat�gorie ou support)</param>
//        /// <history>[D. V. Mussuma]  Modifi� le 28/01/05</history>
//        /// <returns>requete sql</returns>
//        public static DataSet GetMediaStrategy1stElmntData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel){
//            #region variables
//            //Groupe de donn�es
//            DataSet ds = null; 
//            string sql="";
//            //sous requete sql
//            string sqlChild="";
//            //pr�fixe sous requete 
//            string sr="r2";			
//            #endregion

//            #region construction de la requ�te

//            #region table virtuelle 1 et 2 : les premiers anonceurs ou r�f�rences par m�dia
//            //Max investissments des premiers annonceurs ou r�f�rences par m�dia
//            sqlChild=MediaStrategy1stElmntSql(webSession,tableType,comparisonCriterion,mediaLevel);
//            if(WebFunctions.CheckedText.IsStringEmpty(sqlChild.ToString().Trim())){
//                sql+=" select ";
//                //Choix champ m�dia
//                sql+=getFieldForMediaLevel(mediaLevel,sr,".");						
//                //Choix champ produit ou annonceur
//                sql+=GetProductFieldsForMediaStrategy(tableType,"",sr,sr,false,".");
//                //investissement premier �l�ment	
//                sql+=", "+sr+".total_N";									
//                sql+=" from (";
//                sr="r1";
//                sql+="select max(total_N) as total_N,";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel)					
//                    sql+=" id_vehicle ";										
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel)					
//                    sql+=" id_category  ";												
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel)				
//                    sql+=" id_media ";
//                sql+=" from ( ";
//                sql+=sqlChild;
//                sql+=" ) ";	
//                sql+=" group by ";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel)				
//                    sql+="  id_vehicle ";									
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel)				
//                    sql+=" id_category  ";					
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel)					
//                    sql+=" id_media ";								
//                sql+=" ) "+sr;	
//                //investissments des premiers annonceurs ou r�f�rences par m�dia				
//                sr="r2";				
//                sql+=" , (";				
//                sql+=sqlChild;
//                sql+=" ) "+sr;
//                sql+=" where " ;
//                //jointures
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel)					
//                    sql+="  r1.id_vehicle=r2.id_vehicle ";										
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel)					
//                    sql+=" r1.id_category=r2.id_category   ";												
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel)				
//                    sql+="  r1.id_media=r2.id_media ";
//                sql+=" and r1.TOTAL_N=r2.TOTAL_N ";
//                sql+=" and r2.TOTAL_N>0 ";
				
//                #region Ex�cution de la requ�te
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour start�gie media: "+sql,err));
//                }
//                #endregion

//            }
//            #endregion

//            #endregion
		
//            return ds;
//        }
//        /// <summary>
//        /// Requete sql pour r�cup�rer les premiers annonceurs de r�f�rences ou concurrents par m�dia
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table g�n�rer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison (univers ou march�  ou famille)</param>
//        /// <param name="mediaLevel">niveau m�dia (m�dia ou cat�gorie ou support)</param>
//        /// <returns>requete sql</returns>
//        public static string MediaStrategy1stElmntSql(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel){
			
//            #region variables
//            // Mois �tudi�s
//            string StudyMonths="";
//            //liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //requ�te sql
//            string sql="";
//            //Lib�ll� table recap cible
//            string recapTableName="";
//            //est t'il le premier champ de la requ�te
//            bool premier=true;
//            //liste d'�l�ments
//            string list="";
//            //est t'il le premier terme � grouper
//            bool firstgrouby=true;			
//            //liste des familles ayant au moins un �l�ment s�lectionn�
//            string listSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion		
			
//            #region nom de la table recap
//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));						
//            #endregion

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                //Periode de l'�tude
//                #region periode etudi�e
//                //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fr�quence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();
				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//                #endregion

//                //Liste des mois �tudi�s
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);

//                #region construction de la sous requ�te
		
//                sql+="select ";

//                #region champs de la requ�te
//                // champs du niveau m�dia ( m�dia ou cat�gorie  ou support)
//                sql+=getFieldForMediaLevel(mediaLevel,"","");
//                premier=false;
//                //champs produit
//                sql+=GetProductFieldsForMediaStrategy(tableType,"","","",premier,"");
//                //investissments
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=" sum(total_N) as total_N";
//                    premier=false;
//                }
//                #endregion

//                sql+=" from ( ";
//                premier=true;
//                sql+=" select ";

//                #region champs de la sous requ�te
//                //Champs du niveau m�dia ( m�dia ou cat�gorie  ou support)
//                sql+=getFieldForMediaLevel(mediaLevel,DBConstantes.Tables.RECAP_PREFIXE,".");								
//                premier=false;
//                //Champs produit
//                sql+=GetProductFieldsForMediaStrategy(tableType,"",DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,premier,".");
//                //liste des mois �tudi�s
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=StudyMonths;
//                    premier=false;
//                }
//                #endregion

//                sql+=" from  ";

//                #region table concern�es 				
//                sql+=GetMediaStrategyProductTables(recapTableName,tableType,"");
//                #endregion

//                sql+=" where  ";

//                #region jointure et choix de la langue 
//                //Jointures
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_product="+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product";
//                else sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser="+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser";
//                //Choix de la langue
////				sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                    sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//                else sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//                #endregion

//                //S�lections des familles pour le total famille
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(WebFunctions.CheckedText.IsStringEmpty(listSector))
//                        sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector in ("+listSector+") ";
//                }

//                //S�lection produit pour le calcul du total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region S�lection de Produits (groupes et/ou vari�t�s) 
//                    // S�lection en acc�s
//                    premier=true;
//                    // group				
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                        sql+=" and (("+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                        premier=false;
//                    }
//                    // Segment				
//                    if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                        if(!premier) sql+=" or";
//                        else sql+=" and ((";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                        premier=false;
//                    }			
//                    if(!premier) sql+=" )";
			
//                    // S�lection en Exception
//                    // group				
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                        if(premier) sql+=" and (";
//                        else sql+=" and";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                        premier=false;
//                    }
//                    // segment en Exception				
//                    if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                        if(premier) sql+=" and (";
//                        else sql+=" and";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                        premier=false;
//                    }
//                    if(!premier) sql+=" )";
			
//                    #endregion			
//                }	
//                #region droits m�dia
//                // cas du vehicle dans la table pluri
//                list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession, WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                }
//                // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//                }

//                //Droits Parrainage TV
//                if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += " and   " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                }
//                #endregion				

//                #region s�lection des m�dias 				
//                // Media
//                //Non utilis� si s�lection plurimedia
//                if(!DBConstantes.Tables.RECAP_PLURI.ToString().Trim().Equals(recapTableName.Trim())){
//                    list=webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess);					
				
//                    sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),list,true);
//                }
//                #endregion
				
//                //Gestion des droits uniquement pour le total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region Nomenclature Produit (droits)		
//                    sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,true);
//                    #endregion
//                }
				
//                #region tri et regroupement  sous requete
//                sql+=" group by ";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel){				
//                    sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_vehicle ";
//                    firstgrouby=false;	
//                }
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel){				
//                    sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_category  ";
//                    firstgrouby=false;
//                }				
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel){				
//                    sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_media ";
//                    firstgrouby=false;
//                }
//                //Cas R�f�rences
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                    if(!firstgrouby)sql+=" , ";
//                    sql+=" "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product, "+DBConstantes.Tables.PRODUCT_PREFIXE+".product  ";
//                    firstgrouby=false;
//                }else{ //Cas Annonceur
//                    if(!firstgrouby)sql+=" , ";
//                    sql+=" "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    firstgrouby=false;
//                }
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel)
//                    sql+=" order by "+DBConstantes.Tables.RECAP_PREFIXE+".id_vehicle asc ";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel)
//                    sql+=" order by "+DBConstantes.Tables.RECAP_PREFIXE+".id_category asc ";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel)
//                    sql+=" order by "+DBConstantes.Tables.RECAP_PREFIXE+".id_media asc ";
//                sql+=" ) ";						
//                #endregion

//                #region tri et regroupement requete parent 
//                sql+=" group by ";
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel==mediaLevel){				
//                    sql+="  id_vehicle ";
//                    firstgrouby=false;	
//                }
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel){					
//                    sql+=" id_category  ";
//                    firstgrouby=false;
//                }				
//                if(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel){					
//                    sql+=" id_media ";
//                    firstgrouby=false;
//                }
//                //Cas R�f�rences
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                    if(!firstgrouby)sql+=" , ";
//                    sql+=" id_product, product  ";
//                    firstgrouby=false;
//                }else{ //Cas Annonceur
//                    if(!firstgrouby)sql+=" , ";
//                    sql+=" id_advertiser,advertiser";
//                    firstgrouby=false;
//                }				
										
//                #endregion
//                #endregion
//            }

//            return sql;

//        }
//        #endregion
		
//        #region r�cup�rer les premiers annonceurs de r�f�rences ou concurrents pour plurim�dia
//        /// <summary>
//        /// Requete sql pour r�cup�rer les premiers annonceurs de r�f�rences ou concurrents pour plurim�dia
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table g�n�rer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison (univers ou march�  ou famille)</param>		
//        /// <returns>requete sql</returns>
//        public static DataSet GetPluriMediaStrategy1stElmntData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion){
//            #region variables
//            //Groupe de donn�es
//            DataSet ds = null; 
//            string sql="";
//            //sous requete sql
//            string sqlChild="";			
//            #endregion

//            #region table virtuelle 1 et 2 : les premiers anonceurs ou r�f�rences par m�dia
//            //Max investissments des premiers annonceurs ou r�f�rences pour plurimedia
//            sqlChild=PluriMediaStrategy1stElmntSql(webSession,tableType,comparisonCriterion);
//            if(WebFunctions.CheckedText.IsStringEmpty(sqlChild.ToString().Trim())){
//                sql+=" select ";
				
//                //choix produit
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){										
//                    sql+=" r2.product, ";				
//                }				
//                else{//choix annonceur									
//                    sql+=" r2.advertiser, ";					
//                }							
//                sql+=" r1.total_N "; //investissement premier �l�ment									
//                sql+="  from ";
//                sql+=" ( ";
//                    sql+="  select max(total_N) as total_N from ( ";
//                    sql+=sqlChild;
//                    sql+=" ) ";
//                sql+=" ) r1 ";
//                sql+=" , ";
//                sql+=" ( ";
//                sql+=sqlChild;
//                sql+=" ) r2";
//                sql+=" where r1.total_N=r2.total_N ";
//                sql+=" and r2.total_N>0 ";

//                #region Execution de la requ�te
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es pour start�gie media: "+sql,err));
//                } 				
//                #endregion
//            }
//            #endregion

//            return ds;
//        }
//        #endregion

//        #region 1 er �l�ment pour plurim�dia 
//        /// <summary>
//        /// Requete sql pour r�cup�rer les premiers annonceurs de r�f�rences ou concurrents pour plurim�dia
//        /// </summary>
//        /// <param name="webSession">sesion du client</param>
//        /// <param name="tableType">type de la table g�n�rer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison (univers ou march�  ou famille)</param>
//        /// <returns>requete sql</returns>
//        public static string PluriMediaStrategy1stElmntSql(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion){
			
//            #region variables
//            // Mois �tudi�s
//            string StudyMonths="";
//            //liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //requ�te sql
//            string sql="";
//            //Lib�ll� table recap cible
//            string recapTableName="";
//            //est t'il le premier champ de la requ�te
//            bool premier=true;
//            //liste d'�l�ments
//            string list="";
//            //est t'il le premier terme � grouper
//            bool firstgrouby=true;			
//            //liste des familles ayant au moins un �l�ment s�lectionn�
//            string listSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs s�lectionn�s
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion		
			
//            #region nom de la table recap
//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));						
//            #endregion

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                //Periode de l'�tude
//                #region periode etudi�e
//                //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fr�quence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();
				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//                #endregion

//                //Liste des mois �tudi�s
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);

//                #region construction de la requ�te
//                sql+=" select ";

//                #region champs de la  requ�te				
//                //choix produit
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){										
//                    sql+=""+DBConstantes.Tables.PRODUCT_PREFIXE+".product";
//                    premier=false;
//                }				
//                else{//choix annonceur										
//                    sql+=""+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    premier=false;
//                }	
//                //liste des mois �tudi�s
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=StudyMonths;
//                    premier=false;
//                }
//                #endregion

//                sql+=" from ";

//                #region table concern�es 				
//                sql+=GetMediaStrategyProductTables(recapTableName,tableType,"");
//                #endregion

//                sql+=" where  ";

//                #region jointure et choix de la langue 
//                //Jointures et choix de la langue				
//                sql+="  "+GetMediaStrategyJointForProduct(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,tableType,"");
//                #endregion

//                //S�lections des familles pour le total famille
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(WebFunctions.CheckedText.IsStringEmpty(listSector))
//                        sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector in ("+listSector+") ";
//                }

//                //S�lection produit pour le calcul du total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region S�lection de Produits (groupes et/ou vari�t�s) 
//                    // S�lection en acc�s
//                    premier=true;
//                    // group				
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                        sql+=" and (("+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                        premier=false;
//                    }
//                    // Segment				
//                    if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                        if(!premier) sql+=" or";
//                        else sql+=" and ((";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                        premier=false;
//                    }			
//                    if(!premier) sql+=" )";
			
//                    // S�lection en Exception
//                    // group				
//                    if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                        if(premier) sql+=" and (";
//                        else sql+=" and";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                        premier=false;
//                    }
//                    // segment en Exception				
//                    if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                        if(premier) sql+=" and (";
//                        else sql+=" and";
//                        sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                        premier=false;
//                    }
//                    if(!premier) sql+=" )";
			
//                    #endregion			
//                }				
//                #region s�lection des m�dias 

//                #region droits m�dia
//                // cas du vehicle dans la table pluri
//                list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                }
//                // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//                }
//                //Droits Parrainage TV
//                if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                    sql += " and   " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                }
//                #endregion				
				
//                #endregion
				
//                //Gestion des droits uniquement pour le total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region Nomenclature Produit (droits)		
//                    sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,true);
//                    #endregion
//                }
				
//                #region tri et regroupement 
//                sql+=" group by ";
				
//                //Cas R�f�rences
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                    if(!firstgrouby)sql+=" , ";
//                    sql+="  "+DBConstantes.Tables.PRODUCT_PREFIXE+".product  ";
//                    firstgrouby=false;
//                }else{ //Cas Annonceur
//                    if(!firstgrouby)sql+=" , ";
//                    sql+="  "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    firstgrouby=false;
//                }												
//                #endregion

//                #endregion				
//            }
//            return sql;
//        }

//        #endregion

//        #endregion

//        #region R�cup�re total March� o� total famille
		
//        /// <summary>
//        /// Fournit le total march� ou le total famille
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="typeYear">ann�e courante ou ann�e pr�c�dente</param>
//        /// <returns>total march� ou le total famille</returns>
//        public static Int64 getTotalForPeriod(WebSession webSession, FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){
			
//            #region Variables
//            string sql="";			
//            string VehicleAccessList ="";
//            string list;
//            string list2;
//            string StudyMonths;		
//            // Table recap appel�
//            string recapTableName="";
//            long total=0;

//            #endregion
		
//            #region Construction de la requ�te 

//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResultSegmentLevel((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));

//            StudyMonths=IntervalMonthsStudyForTotal(webSession,typeYear);
			
//            sql+=" select   sum(total)" ;		
//            sql+="  from( ";
//            sql+=" select "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector,";
//            sql+=" "+StudyMonths+"  ";
//            sql+=" as  total"; 
//            sql+=" from "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" , " ;
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".sector "+DBConstantes.Tables.SECTOR_PREFIXE+" ,";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".segment "+DBConstantes.Tables.SEGMENT_PREFIXE+""; 
//            sql+=" where   "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment="+DBConstantes.Tables.SEGMENT_PREFIXE+".id_segment";
//            sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector="+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector";
////			sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.SEGMENT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.SECTOR_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
 
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){ 
//                sql+=" and "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector in ( ";
//                sql+=" select distinct id_sector ";
//                sql+=" from "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" ";
//                //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
//                //list2=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.segmentAccess);
//                list = recapUniversSelection.GroupAccessList;
//                list2 = recapUniversSelection.SegmentAccessList;

				
//                if(list.Length>0){
//                    sql+=" where id_group_ in ("+list+") ";
//                }
//                if(list2.Length>0 && list.Length>0){
//                    sql+=" or id_segment in  ("+list2+")";
//                }
//                else if(list2.Length>0){
//                    sql+=" where id_segment in ("+list2+") ";
//                }
//                sql+=" ) ";
//            }

//            #region s�lection des m�dias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql += WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, true);
//            }
//            // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//            }

//            sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess),true);
//            #endregion

//            sql+=" group by "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector ";
//            sql+=" ) ";

//            #endregion
//            try {
//                IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
//                DataSet ds = source.Fill(sql.ToString());
//                total=long.Parse(ds.Tables[0].Rows[0][0].ToString());
//            }
//            catch(System.Exception err) {
//                throw (new SectorAnalysisIndicatorDataAccessException("Impossible to sum the Total getTotalForPeriod",err));
//            }
//            return (total);
//        }

//        #endregion

//        #region R�cup�re total March� o� total famille
		
//        /// <summary>
//        /// Fournit le total march� ou le total famille pour le mois courant
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="typeYear">ann�e courante ou ann�e pr�c�dente</param>
//        /// <returns></returns>
//        public static Int64 getTotalForMonth(WebSession webSession, FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){
			
//            #region Variables
//            string sql="";			
//            string VehicleAccessList ="";
//            string list;
//            string list2;
//            string StudyMonths;		
//            // Table recap appel�
//            string recapTableName="";
//            long total=0;
//            #endregion
		
//            #region Construction de la requ�te 

//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);

//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResultSegmentLevel((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));

//            StudyMonths=CurrentMonthStudyForTotal(webSession,typeYear);
			
//            sql+=" select   sum(total)" ;		
//            sql+=" from( ";
//            sql+=" select "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector,";
//            sql+=" "+StudyMonths+"  ";
//            sql+=" as  total"; 
//            sql+=" from "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" , " ;
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".sector "+DBConstantes.Tables.SECTOR_PREFIXE+" ,";
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".segment "+DBConstantes.Tables.SEGMENT_PREFIXE+""; 
//            sql+=" where   "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment="+DBConstantes.Tables.SEGMENT_PREFIXE+".id_segment";
//            sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector="+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector";
////			sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.SEGMENT_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
//            sql+=" and "+DBConstantes.Tables.SECTOR_PREFIXE+".id_language="+webSession.SiteLanguage+" ";
 
//            if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){ 
//                sql+=" and "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector in ( ";
//                sql+=" select distinct id_sector ";
//                sql+=" from "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" ";
//                //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
//                //list2=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.segmentAccess);
//                list = recapUniversSelection.GroupAccessList;
//                list2 = recapUniversSelection.SegmentAccessList;

				
//                if(list.Length>0){
//                    sql+=" where id_group_ in ("+list+") ";
//                }
//                if(list2.Length>0 && list.Length>0){
//                    sql+=" or id_segment in  ("+list2+")";
//                }
//                else if(list2.Length>0){
//                    sql+=" where id_segment in ("+list2+") ";
//                }
//                sql+=" ) ";
//            }

		
//            #region s�lection des m�dias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, true);
//            }
//            // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//            }

//            sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess),webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess),true);
//            #endregion

//            sql+=" group by "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector,"+DBConstantes.Tables.SECTOR_PREFIXE+".sector ";
//            sql+=" ) ";

//            #endregion

//            try {
//                IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
//                DataSet ds = source.Fill(sql.ToString());
//                total=long.Parse(ds.Tables[0].Rows[0][0].ToString());
//            }
//            catch(System.Exception err) {
//                throw (new SectorAnalysisIndicatorDataAccessException("Impossible to sum the Total getTotalForMonth",err));
//            }
//            return (total);
//        }

//        #endregion
		
//        #region R�cup�re total March� o� total famille ou univers pour Saisonnalite
		
//        /// <summary>
//        /// Fournit le total march� ou le total famille ou univers
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="comparisonCriterion">Crit�re de commparaison pour le calcul des totaux (famille ou march� ou univers)</param>
//        /// <returns></returns>
//        public static DataSet GetSeasonalityTotalForPeriod(WebSession webSession, WebConstantes.CustomerSessions.ComparisonCriterion comparisonCriterion ){
//            #region variables
//            string CompetitorAdvertiserAccessList="";
//            DataSet ds = new DataSet("Seasonality"); 
//            bool premier=true;									
//            string StudyMonths="";
//            string StudyMonthsForRequest2="";
//            string previousYearStudyMonths="";
//            string currentYearStudyMonths="";			
//            bool firstfield=true;			
//            bool beginbyand =true;
//            string recapTableName="";
//            string sql="";
//            int nbMonth=0;			
//            string strYear_1="";
//            string strYear="";
//            string cat="null";
//            string med="null";
//            bool beginbywhere=true;
//            #endregion
		
//            #region construction des listes de produits, media, annonceurs s�lectionn�s
//            //listes de produits, media, annonceurs s�lectionn�s							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
//            string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
//            CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
//            string SegmentAccessList = recapUniversSelection.SegmentAccessList;
//            string SegmentExceptionList = recapUniversSelection.SegmentExceptionList;
//            string GroupAccessList = recapUniversSelection.GroupAccessList;
//            string GroupExceptionList = recapUniversSelection.GroupExceptionList;
//            string MediaAccessList = recapUniversSelection.MediaAccessList;
//            string CategoryAccessList = recapUniversSelection.CategoryAccessList;
//            string VehicleAccessList = recapUniversSelection.VehicleAccessList;
//            #endregion

//            #region Identification table recap
//            //Determine la table recap � appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap � utiliser."));
//            #endregion

//            #region Mise en forme des mois de la p�riode s�lectionn�e
//            //liste mois p�riode N-1 et/ou N
//            StudyMonths=IntervalMonthsStudy(webSession,true);
//            //liste mois p�riode (alias)
//            StudyMonthsForRequest2=ListMonthsStudy(webSession,false,1,true);
			
//            if(webSession.ComparativeStudy){				
//                //liste mois p�riode N
//                currentYearStudyMonths=ListMonthsStudy(webSession,false,1,true);				
//                //liste mois p�riode N-1
//                previousYearStudyMonths=ListMonthsStudy(webSession,true,1,true);
//            }
//            #endregion			

//            #region Periode �tude
//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();				
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
////			DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
////			DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
//            #endregion

//            #region Construction de la requ�te
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                if(PeriodEndDate.Month - PeriodBeginningDate.Month >=0){
					
					
//                    #region Close Select
//                    #region Mois trait�
//                    //Pour chaque mois de la p�riode trait�e
//                    for(int i=PeriodBeginningDate.Month;i<=PeriodEndDate.Month;i++){
						
//                        firstfield=true;
							
//                        if(nbMonth>0)sql+=" UNION ALL  ";																			
						
//                        #region requete 2
//                        sql+=" select request1.*";
//                        firstfield=false;
//                        //Evolution N/N-1
//                        if(webSession.ComparativeStudy){
//                            if(!firstfield)sql+=",";
//                            sql+="decode(request1.Total_N_1,0,-1,ROUND(((request1.Total_N/request1.Total_N_1)*100)-100,0)) as evol";
//                            firstfield=false;
//                        }
//                        if(!firstfield)sql+=",";
//                        sql+=" round((request1.Total_N/request1.nbref)) as budget_moyen";
//                        firstfield=false;
						
//                        //ann�e N						
//                        if(WebFunctions.Dates.yearID(PeriodEndDate,webSession)>0) strYear=WebFunctions.Dates.yearID(PeriodEndDate,webSession).ToString().Trim();

//                        //TABLE PRODUIT						
//                        if(!firstfield)sql+=",";
//                        sql+=" "+DBConstantes.Tables.PRODUCT_PREFIXE+".product";
//                        firstfield=false;
//                        //TABLE ADVERTISER			
//                        if(!firstfield)sql+=",";
//                        sql+=" "+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
												
//                        sql+=",pkg_recap_test.FIRST_PRODUCT_INVEST(request1.id_product,'exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"') as INVESTMENT_PRODUCT ";
//                        sql+=",pkg_recap_test.FIRST_ADVERTISER_INVEST(request1.id_advertiser,'exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"') as INVESTMENT_ADVERTISER ";
//                        sql+=",ROUND((pkg_recap_test.FIRST_PRODUCT_INVEST(request1.id_product,'exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"')/request1.Total_N)*100,3) as SOV_FIRST_PRODUCT ";
//                        sql+=",ROUND((pkg_recap_test.FIRST_ADVERTISER_INVEST(request1.id_advertiser,'exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"')/request1.Total_N)*100,3) as SOV_FIRST_ADVERTISER ";						
					
//                        sql+=" from (";
//                        firstfield=true;
//                        #region requete 1

//                        sql+="select";						
//                        //Investissement ann�e N												
//                        if(!firstfield)sql+=",";
//                        sql+=" sum(exp_euro_N"+strYear+"_"+i+") as Total_N";
//                        firstfield=false;
//                        //Investissement ann�e N-1						 
//                        if(WebFunctions.Dates.yearID(PeriodEndDate.AddYears(-1),webSession)>0) strYear_1=WebFunctions.Dates.yearID(PeriodEndDate.AddYears(-1),webSession).ToString().Trim();
//                        if(webSession.ComparativeStudy){
//                            if(!firstfield)sql+=",";
//                            sql+=" sum(exp_euro_N"+strYear_1+"_"+i+") as Total_N_1";
//                            firstfield=false;
//                        }
//                        try{
//                            //Nombre de r�f�rences par mois	
//                            if(WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal==comparisonCriterion){
								
//                                if( WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList.ToString().Trim())) cat = "'"+CategoryAccessList+"'";								
//                                if( WebFunctions.CheckedText.IsStringEmpty(MediaAccessList.ToString().Trim())) med = "'"+MediaAccessList+"'";
//                                if(!firstfield)sql+=",";
//                                sql+=" pkg_recap_test.NB_REF_BY_MONTH_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as nbref";
//                                firstfield=false;
//                                //Premiere r�f�rence
//                                if(!firstfield)sql+=",";							
//                                sql+="pkg_recap_test.FIRST_PRODUCT_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as id_product";
//                                firstfield=false;
//                                //Premier annonceur
//                                if(!firstfield)sql+=",";
//                                sql+="pkg_recap_test.FIRST_ADVERTISER_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as id_advertiser, "+i+" as Mois";
//                                firstfield=false;
//                            }
							
//                        }catch (Exception firtsErr){
//                            throw (new SectorAnalysisIndicatorDataAccessException("Impossible de d�terminer le premier annonceur, produit et nombre de r�f�rences pour le total calcul�."+firtsErr.Message));
//                        }
											

//                        sql+=" from";

//                        #region Tables concern�es par la requete
//                        //TABLE RECAP	
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+"";
						
						
//                        premier=true;
//                        #region where pour requete 1
////						sql+=" where ";												
////						// Langue recap				
////						sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage.ToString();				
////						beginbyand=true;

//                        #region selections
//                        //Si la requ�te ne porte pas sur le total march�			
//                        //selection des categories et/ou supports
//                        beginbywhere=true;
//                        if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList) || WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
//                            if(beginbywhere)sql+=" where  ";		
////							sql+="  "+GetMediaSelection(CategoryAccessList,MediaAccessList,!beginbyand);
//                            sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,false);

//                            beginbyand=false;
//                            beginbywhere=false;
//                        }
										
//                        //Si la requ�te ne porte  sur le total univers
//                        if(webSession.ComparaisonCriterion ==WebConstantes.CustomerSessions.ComparisonCriterion.universTotal ){						
//                            #region S�lection de Produits
//                            // S�lection en acc�s
//                            premier=true;
//                            // group					
//                            if(WebFunctions.CheckedText.IsStringEmpty(GroupAccessList)){
//                                if(beginbywhere)sql+=" where  ";	
//                                if(!beginbyand)sql+=" and ";
//                                sql+=" (("+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ in ("+GroupAccessList+") ";
//                                premier=false;
//                                beginbyand=false;
//                                beginbywhere=false;
//                            }
//                            // Segment					
//                            if(WebFunctions.CheckedText.IsStringEmpty(SegmentAccessList)){
//                                if(beginbywhere)sql+=" where  ";	
//                                if(!premier) sql+=" or";
//                                else {
//                                    if(!beginbyand)sql+=" and ((";
//                                    else sql+=" ((";
//                                }
//                                sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment in ("+SegmentAccessList+") ";
//                                premier=false;
//                                beginbyand=false;
//                                beginbywhere=false;
//                            }			
//                            if(!premier) sql+=" )";
		
//                            // S�lection en Exception
//                            // group					
//                            if(WebFunctions.CheckedText.IsStringEmpty(GroupExceptionList)){
//                                if(beginbywhere)sql+=" where ";
//                                if(premier) {
//                                    if(!beginbyand)sql+=" and ( ";
//                                    else sql+=" ( ";
//                                }
//                                else if(!beginbyand) sql+=" and ";
//                                sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_group_ not in ("+GroupExceptionList+") ";
//                                premier=false;
//                                beginbyand=false;
//                                beginbywhere=false;
//                            }
//                            // segment en Exception					
//                            if(WebFunctions.CheckedText.IsStringEmpty(SegmentExceptionList)){
//                                if(beginbywhere)sql+=" where ";
//                                if(premier) {
//                                    if(!beginbyand)sql+=" and ( ";
//                                    else sql+=" ( ";
//                                }
//                                else if(!beginbyand) sql+=" and ";
//                                sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+".id_segment not in ("+SegmentExceptionList+") ";
//                                premier=false;
//                                beginbyand=false;
//                                beginbywhere=false;
//                            }
//                            if(!premier) sql+=" )";
		
//                            #endregion															
//                        }
//                        #region s�lection des familles
//                        //Si la requete porte sur le total famille
//                        if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){ 
//                            if(beginbywhere)sql+=" where ";
//                            if(!beginbyand)sql+=" and ";
//                            sql+="   "+DBConstantes.Tables.SECTOR_PREFIXE+".id_sector in ( ";
//                            sql+=" select distinct id_sector ";
//                            sql+=" from "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+" ";
							
//                            if(GroupAccessList.Length>0){
//                                sql+=" where id_group_ in ("+GroupAccessList+") ";
//                            }
//                            if(SegmentAccessList.Length>0 && GroupAccessList.Length>0){
//                                sql+=" or id_segment in  ("+SegmentAccessList+")";
//                            }
//                            else if(SegmentAccessList.Length>0){
//                                sql+=" where id_segment in ("+SegmentAccessList+") ";
//                            }
//                            sql+=" ) ";
//                            beginbywhere=false;
//                        }
//                        #endregion
								
//                        #endregion
									
//                        #endregion									
//                        sql+=" ) request1 ";
//                        firstfield=false;																						
//                        #endregion
//                        //TABLE PRODUIT						
//                        if(!firstfield)sql+=",";
//                        sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+"";
//                        firstfield=false;
//                        //TABLE ADVERTISER			
//                        if(!firstfield)sql+=",";
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+"";
//                        firstfield=false;
						
//                        sql+=" where ";
//                        beginbyand=true;
//                        //Annonceurs			
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+=" "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.ADVERTISER_PREFIXE+".id_advertiser=request1.id_advertiser";
//                        beginbyand=false;

//                        //Produits						
//                        sql+=(!beginbyand)?" and ": " ";
//                        // Langue
//                        sql+=" "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();				
//                        // jointure
//                        sql+=" and "+DBConstantes.Tables.PRODUCT_PREFIXE+".id_product=request1.id_product";
//                        beginbyand=false;

//                        //MANQUE UNION

//                        #endregion

//                        #endregion
											
//                        nbMonth++;
//                    }
//                    #endregion
					
//                }
//             }
//            #endregion

//            #endregion
		
//            if(WebFunctions.CheckedText.IsStringEmpty(sql.ToString().Trim())){
//                #region Execution de la requ�te
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les donn�es"+sql,err));
//                }
//                #endregion				
//            }

//            return ds;
//        }
//        #endregion

//        #region M�thodes internes
		
//        #region liste mois de la p�riode de l'�tude N et optionnellment N-1
//        /// <summary>
//        /// Obtient la chaine de caract�re representant la p�riode de l'�tude
//        /// Elle est utilis� dans la requete pour la selection des mois
//        /// </summary>
//        /// <param name="webSession">session client</param>	
//        /// <param name="withPreviousYearMonths">bool�en specifiant la chaine retourn�e</param>	
//        /// <returns>chaine de caract�re de la p�riode d'�tude</returns>
//        public static string IntervalMonthsStudy(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, bool withPreviousYearMonths){
//            #region Mise en forme des dates
//            //Determine la p�riode de l'�tude
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;
			
//            int downLoadDate=webSession.DownLoadDate;
			
//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod,webSession.PeriodType);
//            #endregion

//            #region dates (mensuels) des investissements 
//            int StartMonth= PeriodBeginningDate.Month;
//            int EndMonth = PeriodEndDate.Month;

//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);

//            if( !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {				
//                for(int i=StartMonth;i<=EndMonth;i++) {
//                    if(i!=EndMonth)
//                    StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + ",";									
//                    else if(i==EndMonth)StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + "";									
//                    //Recuperation de la periode N-1 pour etude comparative
//                    if(withPreviousYearMonths && webSession.ComparativeStudy && (year==0 || year==1)){
//                        ComparativeYearSelected=(year==1)? "2" : "1";
//                        comparativeYear=(year==1)? 2 : 1;
//                        if(i!=EndMonth)
//                        ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + ",";
//                        else if(i==EndMonth)ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                    }
//                }
//                if(webSession.ComparativeStudy && withPreviousYearMonths){
//                    if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths.ToString().Trim()))StudyMonths+=","+ComparativeStudyMonths;
//                    else StudyMonths=ComparativeStudyMonths;
//                }
//            }		
//            #endregion

//            return StudyMonths;
//        }
//        #endregion

//        #region liste mois de la p�riode �tudi�e N et/ou N-1
//        /// <summary>
//        /// Obtient la chaine de caract�re representant la p�riode de l'�tude
//        /// Elle est utilis� dans la requete pour la selection des mois
//        /// </summary>
//        /// <param name="webSession">session client</param>	
//        /// <param name="nbYear">Nombre d'ann�es</param>
//        /// <param name="onlyAlias">Sans alias</param>
//        /// <param name="withPreviousYearMonths">bool�en specifiant la chaine retourn�e</param>	
//        /// <returns>chaine de caract�re de la p�riode d'�tude</returns>
//        public static string ListMonthsStudy(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, bool withPreviousYearMonths, int nbYear,bool onlyAlias){
//            #region Mise en forme des dates
//            //Determine la p�riode de l'�tude
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;
			
//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region dates (mensuels) des investissements 
//            int StartMonth= PeriodBeginningDate.Month;
//            int EndMonth = PeriodEndDate.Month;
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);
//            if( !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {
//                //On ram�ne les mois de la p�riode N et/ou N-1
//                switch (nbYear){					
//                    case 1 :
//                        for(int i=StartMonth;i<=EndMonth;i++) {
//                            //On retourne uniquement les mois de la p�riode N
//                            if(!withPreviousYearMonths && i!=EndMonth){
//                                if(!onlyAlias)
//                                    StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + ",";
//                                else StudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + ",";
//                            }
//                            else if(!withPreviousYearMonths && i==EndMonth){
//                                if(!onlyAlias)
//                                StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + "";											
//                                else StudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + "";											
//                            }
//                            //Recuperation de la periode N-1 pour etude comparative
//                            if(withPreviousYearMonths && webSession.ComparativeStudy && (year==0 || year==1)){
//                                ComparativeYearSelected=(year==1)? "2" : "1";
//                                comparativeYear=(year==1)? 2 : 1;
//                                if(i!=EndMonth && StartMonth!=EndMonth){
//                                    if(!onlyAlias)ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + ",";
//                                    else ComparativeStudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + ",";
//                                }
//                                else if(i==EndMonth && StartMonth!=EndMonth){
//                                    if(!onlyAlias)ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                                    else ComparativeStudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                                }
//                                else{
//                                    if(!onlyAlias)ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                                    else ComparativeStudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";	
//                                }
//                            }
//                        }
//                        //if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths.ToString().Trim()) && !withPreviousYearMonths)StudyMonths=StudyMonths;
//                        //else 
//                        if(withPreviousYearMonths)StudyMonths=ComparativeStudyMonths;
//                        break;
//                    case 2 :
//                        for(int i=StartMonth;i<=EndMonth;i++) {
//                            if(i!=EndMonth)
//                            StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + ",";									
//                            else if (i==EndMonth)StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,year,3,webSession) + "";									
//                            //Recup�ration de la periode N-1 pour etude comparative
//                            if(withPreviousYearMonths && webSession.ComparativeStudy && (year==0 || year==1)){
//                                ComparativeYearSelected=(year==1)? "2" : "1";
//                                comparativeYear=(year==1)? 2 : 1;
//                                if(i!=EndMonth){
//                                    if(!onlyAlias)
//                                        ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + ",";
//                                    else ComparativeStudyMonths+=" "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + ",";
//                                }
//                                else if(i==EndMonth){
//                                    if(!onlyAlias)
//                                    ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  "+WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                                    else ComparativeStudyMonths+="  "+ WebFunctions.Dates.GetMonthAlias(i,comparativeYear,3,webSession) + "";
//                                }

//                            }
//                        }
//                        if(webSession.ComparativeStudy && withPreviousYearMonths){
//                            if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths.ToString().Trim()))StudyMonths+=","+ComparativeStudyMonths;
//                            else StudyMonths=ComparativeStudyMonths;
//                        }
						
//                    break;
//                    default :
//                        throw (new SectorAnalysisIndicatorDataAccessException("Impossible d'identifier la p�riode � �tudier."));
//                }
//            }	
//            #endregion

//            return StudyMonths;
//        }
//        #endregion     		
		
//        #region p�riode �tudi�e pour indicateur �volution
//        /// <summary>
//        /// Obient la p�riode pour l'indicateur Evolution
//        /// </summary>
//        /// <param name="webSession">sesssion client</param>
//        /// <returns>liste des mois</returns>
//        public static string EvolutionIntervalMonthsStudy(WebSession webSession){
		
//            #region Mise en forme des dates
//            //Determine l'evolution
//            string StudyMonthsEvolution="";
//            // Recup�re l'investissement de la p�riode N
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region dates (mensuels) des investissements 
//            int StartMonth= PeriodBeginningDate.Month;
//            int EndMonth = PeriodEndDate.Month;
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);	
//            if( !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {				
//                for(int i=StartMonth;i<=EndMonth;i++) {															
//                    if(i==EndMonth && StartMonth!=EndMonth){
//                        StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" ) as  total_N , ";
//                    }
//                    else if(i==StartMonth && StartMonth!=EndMonth){
//                        StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                    }
//                    else if(StartMonth==EndMonth){
//                        StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+") total_N , ";
//                    }
//                    else{
//                        StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                    }

//                    if(i==EndMonth && StartMonth!=EndMonth){
//                        StudyMonthsEvolution+="exp_euro_N"+YearSelected+"_"+i.ToString()+"  ";
//                    }
//                    else if(i==StartMonth && StartMonth!=EndMonth){
//                        StudyMonthsEvolution+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                    }
//                    else if(StartMonth==EndMonth){
//                        StudyMonthsEvolution+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+" ";
//                    }
//                    else{
//                        StudyMonthsEvolution+="exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                    }
					
										
//                    ComparativeYearSelected=(year==1)? "2" : "1";
//                    comparativeYear=(year==1)? 2 : 1;
//                    if(i==EndMonth && StartMonth!=EndMonth){
//                        ComparativeStudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")) as Ecart ";
//                    }
//                    else if(i==StartMonth && StartMonth!=EndMonth){
//                        ComparativeStudyMonths+=" - (exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
//                    }
//                    else if(StartMonth==EndMonth){
//                        ComparativeStudyMonths+="-(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+" )) as  Ecart ";
//                    }
//                    else{
//                        ComparativeStudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
//                    }					
//                }				
				
//                StudyMonths+=StudyMonthsEvolution+ComparativeStudyMonths;				
//            }
			
//            #endregion

//            return StudyMonths;	
		
//        }
//        #endregion
		
//        #region p�riode �tudi�e pour total march� ou univers
//        /// <summary>
//        /// Utilis�e dans le total univers et total march�
//        /// </summary>
//        /// <param name="webSession">Session</param>
//        /// <param name="typeYear">type d'ann�e (courante, pr�c�dente)</param>
//        /// <returns></returns>
//        public static string IntervalMonthsStudyForTotal(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){

//            #region Mise en forme des dates
//            //Determine la p�riode de l'�tude
//            string StudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region dates (mensuels) des investissements 
//            int StartMonth= PeriodBeginningDate.Month;
//            int EndMonth = PeriodEndDate.Month;
//            #region A SUPPRIMER
////			if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-1)) {
////				YearSelected="1";
////				year=1;
////			}
////			if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2)) {
////				YearSelected="2";
////				year=2;
////			}	
//            #endregion

//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);

//            if( !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {				
//                for(int i=StartMonth;i<=EndMonth;i++) {
					
//                    if(typeYear==FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear){
					
//                        if(i==EndMonth && StartMonth!=EndMonth){
//                            StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" )";
//                        }
//                        else if(i==StartMonth && StartMonth!=EndMonth){
//                            StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                        }
//                        else if(StartMonth==EndMonth){
//                            StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+") ";
//                        }
//                        else{
//                            StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
//                        }
//                    }else{
//                        //Recuperation de la periode N-1 pour etude comparative
//                        ComparativeYearSelected=(year==1)? "2" : "1";
//                        comparativeYear=(year==1)? 2 : 1;
//                        if(i==EndMonth && StartMonth!=EndMonth){
//                            StudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+") ";
//                        }
//                        else if(i==StartMonth && StartMonth!=EndMonth){
//                            StudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
//                        }
//                        else if(StartMonth==EndMonth){
//                            StudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+" )  ";
//                        }
//                        else{
//                            StudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
//                        }						
//                    }
//                }			
//            }
			
//            #endregion

//            return StudyMonths;
			
//        }
//        #endregion

//        #region Mois courant �tudi�
//        /// <summary>
//        /// Utilis�e dans le total univers et total march�
//        /// </summary>
//        /// <param name="webSession">Session</param>
//        /// <param name="typeYear">type d'ann�e (courante, pr�c�dente)</param>
//        /// <returns></returns>
//        public static string CurrentMonthStudyForTotal(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){

//            #region Mise en forme des dates
//            //Determine la p�riode de l'�tude
//            string StudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            #endregion

//            #region investissement mois courant			
//            int EndMonth = PeriodEndDate.Month;
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);		
//            if(!PeriodEndDate.Equals(null) ) {								
//                    if(typeYear==FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear){																	
//                        StudyMonths="sum(exp_euro_N"+YearSelected+"_"+EndMonth.ToString()+") ";												
//                    }else{
//                        //Recuperation de la periode N-1 pour etude comparative
//                        ComparativeYearSelected=(year==1)? "2" : "1";
//                        comparativeYear=(year==1)? 2 : 1;												
//                        StudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+EndMonth.ToString()+" )  ";																	
//                    }						
//            }			
//            #endregion

//            return StudyMonths;
			
//        }
//        #endregion

//        #region periode etudi�e pour nouveaux produits ou annonceurs
//        /// <summary>
//        /// Gen�re la chaine de cararct�re n�cessaire au s�lections d'investissements
//        /// des nouveaux produits
//        /// </summary>
//        /// <param name="webSession">session client</param>
//        /// <param name="TotalType">total univers ou march� ou famille</param>
//        /// <returns>chaine selection investissements</returns>
//        public static string NoveltyIntervalMonthsStudy(WebSession webSession,bool TotalType){
//            #region Mise en forme des dates
//            //Determine la p�riode de l'�tude
//            string StudyMonths="";			
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";			
//            int year=0;
//            int comparativeYear=0;			
//            // bool hasData=true;

//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
			
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);			
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
			
//            #endregion

//            #region dates (mensuels) des investissements 
//            int StartMonth= PeriodBeginningDate.Month;
//            int EndMonth = PeriodEndDate.Month;
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);		
//            if(HasData(PeriodEndDate) && !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {				
//                for(int i=1;i<=EndMonth-1;i++) {
//                    //Determine inactivit� depuis le d�but de l'ann�e
//                    if(1!=EndMonth && i!=EndMonth-1){
//                        StudyMonths+=" sum(exp_euro_N"+YearSelected+"_"+i.ToString()+") + ";
//                    }
//                    else if((i==EndMonth-1) || 1==EndMonth){
//                        StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+") isInactive";
//                    }																								
//                }
//                //Recuperation de la periode N-1 pour etude comparative
//                if(webSession.ComparativeStudy && (year==0 || year==1)){
//                    ComparativeYearSelected=(year==1)? "2" : "1";
//                    comparativeYear=(year==1)? 2 : 1;
//                    for(int j=1;j<=12;j++){
//                        //investissement mois N-1 
//                        if(j!=12){
//                            ComparativeStudyMonths+=" sum(exp_euro_N"+ComparativeYearSelected+"_"+j.ToString()+" ) "+WebFunctions.Dates.GetMonthAlias(j,comparativeYear,3,webSession)+" , ";
//                        }
//                        else{
//                            ComparativeStudyMonths+=" sum(exp_euro_N"+ComparativeYearSelected+"_"+j.ToString()+" )  "+WebFunctions.Dates.GetMonthAlias(j,comparativeYear,3,webSession) ;
//                        }						
//                    }
//                }
//                if(webSession.ComparativeStudy){
//                    StudyMonths+=","+ComparativeStudyMonths;
//                }								
//            }
			
//            #endregion

//            return StudyMonths;

//        }
			
//        #endregion

//        #region validation periode etude
//        /// <summary>
//        /// Les nouveaux produits ou annonceurs ne sont disponibles que pass�
//        /// le 20 f�vrier s'il s'agit de l'ann�e actuelle
//        /// </summary>
//        /// <param name="PeriodEndDate">fin de p�riode d'�tude</param>
//        /// <returns>bool�en </returns>
//        public static bool HasData(DateTime PeriodEndDate){
//            //Diff�rence en jours, heurs, et minutes.
//            DateTime oldDate = new DateTime(DateTime.Now.Year,3,20);
//            TimeSpan ts = PeriodEndDate - oldDate;
//            // Difference en jours.
//            int differenceInDays = 1; 
//            //L'etude s'effectue sur un mois complet
//            if(!PeriodEndDate.Equals(null) ){
//                //L'etude s'effectue sur un mois complet
//                //Si ann�e �tudi�e == ann�e actuelle, il n'y pas de donn�es disponibles avant le 20 mars												
//                if(PeriodEndDate.Year.Equals(DateTime.Now.Year) || PeriodEndDate.Month==1 || PeriodEndDate.Month==2 || PeriodEndDate.Month==3)
//                differenceInDays = ts.Days;	
//                if(differenceInDays <=0 )return false;
//                else return true;
//            }	
//            else return false;
//        }
//        #endregion

//        #region mois etude comparative ann�e N-1
//        /// <summary>
//        /// Donne la liste des mois pour ann�e N-1
//        /// </summary>
//        /// <param name="webSession">session client</param>
//        /// <returns></returns>
//        public static string StringMonthsComparativeStudy(WebSession webSession){
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fr�quence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();

//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);


//            #region dates (mensuels) des investissements 			
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);		
//            //Recuperation de la periode N-1 pour etude comparative
//            if(webSession.ComparativeStudy && (year==0 || year==1)){
//                ComparativeYearSelected=(year==1)? "2" : "1";
//                comparativeYear=(year==1)? 2 : 1;
//                for(int j=1;j<=12;j++){
//                    //investissement mois N-1 
//                    if(j!=12){
//                        ComparativeStudyMonths+="  "+WebFunctions.Dates.GetMonthAlias(j,comparativeYear,3,webSession)+" , ";
//                    }
//                    else{
//                        ComparativeStudyMonths+="   "+WebFunctions.Dates.GetMonthAlias(j,comparativeYear,3,webSession) ;
//                    }						
//                }
//            }
//            return ComparativeStudyMonths;
//        }
//        #endregion
//        #endregion

//        #region identifiant de l'ann�e : 0==N , 1==N-1,2==N-2
//        /// <summary>
//        /// Obtient l'identifiant de l'ann�e s�lectionn�e  : 0==N , 1==N-1,2==N-2
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="YearSelected">ann�e s�lectionn�</param>
//        /// <param name="year">identifiant ann�e s�lectionn�</param>
//        /// <param name="PeriodBeginningDate">date de d�but</param>
//        private static void GetYearSelected(WebSession webSession, ref string YearSelected,ref int year,DateTime PeriodBeginningDate){
//            if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-1)) {
//                if(DateTime.Now.Year>webSession.DownLoadDate){
//                    YearSelected="";
//                    year=0;
//                }				
//                else{
//                    YearSelected="1";
//                    year=1;
//                }
//            }
//            if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2)) {
//                if(DateTime.Now.Year>webSession.DownLoadDate){
//                    YearSelected="1";
//                    year=1;				
//                }
//                else{				
//                    YearSelected="2";
//                    year=2;
//                }
//            }
//            if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-3)) {
//                if(DateTime.Now.Year>webSession.DownLoadDate){
//                    YearSelected="2";
//                    year=2;				
//                }				
//            }	
//        }
//        #endregion
	
//        #region Sous REQUETE 1 : r�partition m�dia sur le total de la p�riode et par annonceur
//        /// <summary>
//        ///Sous REQUETE 1:	r�cup�re la r�partition m�dia sur le total de la p�riode  contenant les �l�ments ci-apr�s :
//        ///en ligne :
//        ///l'investissement total famille ou march� ou univers  de la p�riode N et N-1 d�clin� par media, cat�gorie ,supports, annonceur.													
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="recapTableName">nom de la table de donn�es</param>
//        /// <param name="comparisonCriterion">crit�re de comparaison (total univers, famille ou march�)</param>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        /// <param name="StudyMonths">p�riode �tudi�e</param>
//        /// <param name="indexSubQuery">index de la sous requ�te</param>
//        /// <param name="GroupAccessList">groupes en acc�s</param>
//        /// <param name="GroupExceptionList">groupes en exceptions</param>
//        /// <param name="SegmentAccessList">vari�t�s en acc�s</param>
//        /// <param name="SegmentExceptionList">vari�t�s en exceptions</param>
//        /// <param name="CategoryAccessList">cat�gories en acc�s</param>
//        /// <param name="MediaAccessList">supports en acc�s</param>
//        /// <param name="AdvertiserAccessList">annonceurs en acc�s</param>
//        /// <param name="RefenceOrCompetitorAdvertiser">choix annonceur de r�f�rences ou concurrents</param>
//        /// <returns>r�partition m�dia sur le total de la p�riode et par annonceur</returns>
//        private static string MediaStrategySubQuery(WebSession webSession,string recapTableName,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.PalmaresRecap.ElementType tableType,string StudyMonths, string indexSubQuery,string GroupAccessList,string GroupExceptionList,string SegmentAccessList,string SegmentExceptionList,string CategoryAccessList,string MediaAccessList,string AdvertiserAccessList,bool RefenceOrCompetitorAdvertiser){
			
//            #region variables
//            string sql="";
//            bool premier =true;
//            bool firstgrouby=true;
//            string dot=".";
//            string list="";
//            string SortOrder="asc";	
			
//            #endregion
			
//            //champs m�dia
//            sql+=" select  ";
//            sql+=GetFieldsForMediaDetail(webSession,DBConstantes.Tables.RECAP_PREFIXE,indexSubQuery);
//            premier=false;			
//            //Champs produits			
//            sql+=GetProductFieldsForMediaStrategy(tableType,indexSubQuery,DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,premier,dot);
//            //les mois sur lesquels s'effectue l'etude
//            if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                if(!premier)sql+=" ,";
//                sql+=StudyMonths;
//                premier=false;
//            }			
//            sql+=" from ";

//            #region tables concern�s
//            sql+=GetMediaStrategyProductTables(recapTableName,tableType,indexSubQuery);				
//            #endregion

//            sql+=" where  "; 

//            //Jointures et choix de la langue
//            sql+=GetMediaStrategyJointForProduct(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,tableType,indexSubQuery);			
			
//            #region s�lection produit
//            //S�lection produit
//            sql+=WebFunctions.SQLGenerator.GetRecapProductSelection(comparisonCriterion,recapTableName,indexSubQuery,GroupAccessList,GroupExceptionList,SegmentAccessList,SegmentExceptionList,AdvertiserAccessList,RefenceOrCompetitorAdvertiser,true);						
//            #endregion

//            #region droits m�dia
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,true);
//                //liste des m�dia autoris�s
////				sql+="  and "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+".id_vehicle in ("+webSession.CustomerLogin.getListIdVehicle()+") ";
//            }
			
//            // V�rifie s'il a toujours les droits pour acc�der aux donn�es de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,true);	
//            }

//            //Droits Parrainage TV
//            if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += " and   " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_category not in (68)  ";
//            }
//            #endregion

//            #region s�lection des m�dias										
//            sql+=WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,indexSubQuery,true);
//            #endregion
					
//            //Gestion des droits uniquement pour le total univers
//            if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                #region Nomenclature Produit (droits)		
//                sql+="  "+WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,true);
//                #endregion
//            }
									
//            #region tri et regroupement
//            //regroupement des enregistements
//            sql+=" group by ";				
//            //Regroupement des m�dias
//            sql+=GetFieldsForMediaDetail(webSession,DBConstantes.Tables.RECAP_PREFIXE,indexSubQuery);
//            firstgrouby=false;
//            if(!firstgrouby)sql+=" , ";
//            //Regroupement des produits
//            sql+=GetProductByGroupForMediaStrategy(tableType,indexSubQuery);
//            //tri des enregistements
//            sql+=" order by ";	
//            sql+=GetMediaStrategyOrderForMediaDetail(webSession,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,SortOrder,false);								
//            #endregion			

//            return sql;
//        }
//        #endregion

//        #region champs pour le niveau de d�tail m�dia
//        /// <summary>
//        ///Champs m�dia de la requ�te en fonction du niveau m�dia
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        /// <param name="indexSubQuery">index sous requ�te</param>
//        /// <returns>champs m�dia � afficher</returns>
//        private static string GetFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+PREFIXE+indexSubQuery+".id_category  ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+PREFIXE+indexSubQuery+".id_category, "+PREFIXE+indexSubQuery+".id_media ";
//                default:
//                     throw (new SectorAnalysisIndicatorDataAccessException ("getFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }			
//        }
//        /// <summary>
//        /// Champs m�dia de la requ�te en fonction du niveau m�dia
//        /// </summary>
//        /// <param name="mediaLevel">niveau m�dia</param>
//        /// <param name="PREFIXE">pr�fixe de la table de donn�es</param>
//        /// <param name="dot">point (caract�re) si champs doit �tre afficher avec le pr�fixe de la table</param>
//        /// <returns>champ m�dia � afficher</returns>
//        private static string getFieldForMediaLevel(FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel,string PREFIXE,string dot){
//            switch(mediaLevel){
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel:
//                    return PREFIXE+dot+"id_vehicle ";
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel:
//                    return PREFIXE+dot+"id_category";
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel:
//                    return PREFIXE+dot+"id_media";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getFieldForMediaLevel(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel mediaLevel)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }
//        }
//        #endregion

//        #region champs pour le niveau de d�tail m�dia pour strat�gie m�dia
//        /// <summary>
//        ///Champs m�dia de la requ�te en fonction du niveau m�dia pour m�dia strat�gie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        /// <param name="indexSubQuery">index sous requ�te</param>
//        /// <returns>champs m�dia � afficher</returns>
//        private static string GetMediaStrategyFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle ,"+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+PREFIXE+indexSubQuery+".id_category,"+DBConstantes.Tables.CATEGORY_PREFIXE+".category";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+PREFIXE+indexSubQuery+".id_category, "+DBConstantes.Tables.CATEGORY_PREFIXE+".category,"+PREFIXE+indexSubQuery+".id_media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".media";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }			
//        }
//        #endregion

//        #region Champs produit pour stat�gie m�dia
//        /// <summary>
//        /// Champs produit pour stat�gie m�dia
//        /// </summary>
//        /// <param name="tableType">type de table ("advertiser" ou "product")</param>		
//        /// <param name="indexSubQuery">index de la sous requ�te</param>
//        /// <param name="ADVERTISER_PREFIXE">pr�fixe annonceur</param>	
//        /// <param name="PRODUCT_PREFIXE">pr�fixe produit</param>	
//        /// <param name="premier">vrai si c'est le premier champ</param>
//        /// <param name="dot">point (caract�re) si champs doit �tre afficher avec le pr�fixe de la table</param>
//        /// <returns>champs produits</returns>
//        private static string GetProductFieldsForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery,string PRODUCT_PREFIXE,string ADVERTISER_PREFIXE,bool premier,string dot){
//            string sql="";
//            //Cas R�f�rences
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                if(!premier)sql+=" ,";
//                sql+=" "+PRODUCT_PREFIXE+indexSubQuery+dot+"id_product, "+PRODUCT_PREFIXE+indexSubQuery+dot+"product  ";
//                premier=false;
//            }else{ //Cas Annonceur
//                if(!premier)sql+=" ,";
//                sql+=" "+ADVERTISER_PREFIXE+indexSubQuery+dot+"id_advertiser, "+ADVERTISER_PREFIXE+indexSubQuery+dot+"advertiser";
//                premier=false;
//            }
//            return sql;
//        }
//        #endregion		

//        #region s�lection des m�dia
////		/// <summary>
////		/// s�lection  m�dia 
////		/// </summary>
////		/// <param name="CategoryAccessList">cat�gories en acc�s</param>
////		/// <param name="MediaAccessList">supports en acc�s</param>	
////		/// <param name="beginbyand">vrai si commence par "and"</param>	
////		/// <returns>s�lection m�dia</returns>
////		private static string GetMediaSelection(string CategoryAccessList,string MediaAccessList,bool beginbyand){
////			return WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,"",beginbyand);
////		}

//        #region s�lection  m�dia pour stat�gie m�dia
////		/// <summary>
////		/// s�lection  m�dia pour stat�gie m�dia
////		/// </summary>
////		/// <param name="CategoryAccessList">cat�gories en acc�s</param>
////		/// <param name="MediaAccessList">supports en acc�s</param>
////		/// <param name="indexSubQuery">index de la sous requ�te</param>
////		/// <param name="beginbyand">vrai si commence par "and"</param>
////		/// <returns>s�lection m�dia</returns>
////		private static string getMediaSelectionForMediaStrategy(string CategoryAccessList,string MediaAccessList,string indexSubQuery,bool beginbyand){
////			string sql="";
////			// Category				
////			if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList)){
////				if(WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)){
//////					if(beginbyand)
//////						sql+="  ( ";
//////					else sql+=" and ( ";
////					if(beginbyand)
////						sql+=" and ( ";
////					else sql+="  ( ";
////				}
////				else if(beginbyand)sql+=" and  ";			
////				sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+".id_category in ("+CategoryAccessList+") ";
////			}
////			// Media			
////			if(WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)){
////				if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList))sql+=" or ";
////				else if(beginbyand)sql+=" and  ";
////				sql+="  "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+".id_media in ("+MediaAccessList+") ";
////				if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList))sql+= " ) ";
////			}	
////			return sql;
////		}
//        #endregion

//        #endregion

//        #region regroupement des produits pour strat�gie m�dia
//        /// <summary>
//        /// Regroupement des produits pour strat�gie m�dia.
//        /// </summary>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requete</param>
//        /// <returns>produit regroup�s</returns>
//        private static string GetProductByGroupForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery){
//            switch(tableType){
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product:
//                    //Cas R�f�rences
//                    return " "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+".id_product, "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+".product  ";
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser:
//                    //Cas Annonceur
//                    return " "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+".advertiser";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getProductByGroupForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery)-->Impossible de d�terminer le type de l'�l�ment de la nomenclature produit."));					
//            }			
//        }
//        #endregion

//        #region Tables pour le niveau de d�tail m�dia pour strat�gie m�dia
//        /// <summary>
//        ///Tables m�dia de la requ�te en fonction du niveau m�dia pour m�dia strat�gie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>		
//        /// <returns>champs m�dia � afficher</returns>
//        private static string GetMediaStrategyTablesForMediaDetail(WebSession webSession){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+"";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+"";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+"";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyTablesForMediaDetail(WebSession webSession)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }						
//        }
//        #endregion

//        #region Tables produits pour strat�gie m�dia
//        /// <summary>
//        /// Choix table de la nomenclature produit
//        /// </summary>
//        /// <param name="recapTableName">table recap</param>
//        /// <param name="tableType">type de table ("advertisr" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requ�te (�ventuellement)</param>
//        /// <returns>nom de la table produit</returns>
//        private static string GetMediaStrategyProductTables(string recapTableName,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType tableType,string indexSubQuery){
//            string sql="";
//            //TABLE RECAP cibl�e
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+" , " ;
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                //TABLE PRODUITS (PRODUCT)
//                sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+"";					
//                //TABLE ANNONCEUR (ADVERTISER)
//            else sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+" ";				
			
//            return sql;
//        }
//        #endregion

//        #region Jointures pour le niveau de d�tail m�dia pour strat�gie m�dia
//        /// <summary>
//        ///Jointures m�dia de la requ�te en fonction du niveau m�dia pour m�dia strat�gie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="SubQueryAlias">alias de la sous requ�te</param>		
//        /// <returns>jointures des m�dia en fonction du niveau s�lectionn�</returns>
//        private static string GetMediaStrategyJointForMediaDetail(WebSession webSession,string SubQueryAlias){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return " "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage
//                            +" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle = "+SubQueryAlias+".id_vehicle ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return  " "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage
//                        +" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle = "+SubQueryAlias+".id_vehicle "
//                        + " and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage
//                            +" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category = "+SubQueryAlias+".id_category ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return  " "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage
//                        +" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle = "+SubQueryAlias+".id_vehicle "
//                        + " and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage
//                        +" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category = "+SubQueryAlias+".id_category "
//                        + " and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage
//                        +" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media = "+SubQueryAlias+".id_media ";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyTablesForMediaDetail(WebSession webSession)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }							
//        }
//        #endregion

//        #region Jointures des �l�ments la s�lection produit pour strat�gie m�dia
//        /// <summary>
//        /// Jointures pour la s�lection produit
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="Tables_RECAP_PREFIXE">prefixe de la table recap</param>
//        /// <param name="PRODUCT_PREFIXE">prefixe produit</param>
//        /// <param name="ADVERTISER_PREFIXE">prefixe annonceur</param>
//        /// <param name="tableType">type de table ( "advertiser" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requ�te</param>
//        /// <returns>code sql des jointures </returns>
//        private static string GetMediaStrategyJointForProduct(WebSession webSession,string Tables_RECAP_PREFIXE,string PRODUCT_PREFIXE,string ADVERTISER_PREFIXE,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType tableType,string indexSubQuery){
//            string sql="";
//            //Jointures
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                sql+="  "+Tables_RECAP_PREFIXE+indexSubQuery+".id_product="+PRODUCT_PREFIXE+indexSubQuery+".id_product";
//            else sql+="  "+Tables_RECAP_PREFIXE+indexSubQuery+".id_advertiser="+ADVERTISER_PREFIXE+indexSubQuery+".id_advertiser";
//            //Choix de la langue
////			sql+=" and "+Tables_RECAP_PREFIXE+indexSubQuery+".id_language_i="+webSession.SiteLanguage+" ";
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                sql+="  and "+PRODUCT_PREFIXE+indexSubQuery+".id_language="+webSession.SiteLanguage+" ";
//            else sql+="  and "+ADVERTISER_PREFIXE+indexSubQuery+".id_language="+webSession.SiteLanguage+" ";

//            return sql;
//        }
//        #endregion

//        #region tri des m�dia pour strat�gie m�dia
//        /// <summary>
//        ///tri des  m�dia de la requ�te en fonction du niveau m�dia pour m�dia strat�gie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        ///<param name="SortOrder">Ordre de tri</param>
//        ///<param name="withMediaLabel">vrai s'il faut ordonner le libell� du m�dia</param>
//        /// <returns>champs m�dia � afficher</returns>
//        private static string GetMediaStrategyOrderForMediaDetail(WebSession webSession,string PREFIXE,string SortOrder,bool withMediaLabel){			
//            string sql="";
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    sql+="  "+PREFIXE+".id_vehicle "+SortOrder;
//                    if(withMediaLabel)sql+=","+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle "+SortOrder;
//                    return sql;
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                     if(withMediaLabel)sql+="  "+PREFIXE+".id_vehicle "+SortOrder+","+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle "+SortOrder
//                          +","+PREFIXE+".id_category "+SortOrder+","+DBConstantes.Tables.CATEGORY_PREFIXE+".category "+SortOrder ;
//                     else sql+="  "+PREFIXE+".id_vehicle "+SortOrder +","+PREFIXE+".id_category "+SortOrder;
//                    return sql;
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                     if(withMediaLabel)sql+="  "+PREFIXE+".id_vehicle "+SortOrder+","+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle "+SortOrder
//                        +","+PREFIXE+".id_category "+SortOrder+","+DBConstantes.Tables.CATEGORY_PREFIXE+".category "+SortOrder 
//                        +" ,"+PREFIXE+".id_media "+SortOrder+","+DBConstantes.Tables.MEDIA_PREFIXE+".media "+SortOrder;
//                     else sql+="  "+PREFIXE+".id_vehicle "+SortOrder+","+PREFIXE+".id_category "+SortOrder+" ,"+PREFIXE+".id_media "+SortOrder;
//                    return sql;
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyOrderForMediaDetail(WebSession webSession,string PREFIXE,string SortOrder,bool withMediaLabel)-->Impossible d'initialiser les champs m�dia de la requ�te."));					
//            }			
//        }
//        #endregion

//        #region tri des produits pour strat�gie m�dia
//        /// <summary>
//        /// tri des produits pour strat�gie m�dia.
//        /// </summary>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        ///<param name="SubQueryAlias">Alias de la sous requ�te</param>
//        ///<param name="SortOrder">ordre de tri</param>
//        /// <returns>produit tri�s</returns>
//        private static string GetProductByOrderForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string SubQueryAlias, string SortOrder){
//            switch(tableType){
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product:
//                    //Cas R�f�rences
//                    return " "+SubQueryAlias+".id_product  "+SortOrder;
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser:
//                    //Cas Annonceur
//                    return " "+SubQueryAlias+".id_advertiser  "+SortOrder;					
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getProductByOrderForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string SubQueryAlias, string SortOrder)-->Impossible de d�terminer le type de l'�l�ment de la nomenclature produit."));					
//            }			
//        }
//        #endregion		

//        #endregion								  
//    }
//}
