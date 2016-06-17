//#region Information
//// Auteur: Dédé Mussuma
//// Créé le: 24/09/2004
//// Modifiée le: 
////	24/09/2004	Dédé Mussuma
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
//    /// Accès au données pour les indicateurs
//    /// </summary>
//    public class IndicatorDataAccess{				

//        #region Dataset Saisonnalité
        
//        #region DataSet Saisonnalité pour sortie tableau
//        /// <summary>
//        /// Fournit la liste des annonceurs,des références, les investissements mensuels (période N et N-1)
//        /// pour indicateur Saisonnalité, sur le total supports de l'univers ou marché ou famille.
//        /// </summary>
//        /// <remarks>Cette méthode est utilisée pour la présentation des données sous forme de tableau.</remarks>
//        /// <param name="webSession">Session du client</param>	
//        /// <param name="withAdvertisers">champs annonceurs</param>	
//        /// <param name="withRights">droits produits</param>
//        /// <returns>Données de saisonnalité (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifié le 24/11/04</history>
//        public static DataSet GetData(WebSession webSession, bool withAdvertisers, bool withRights){	
//            #region variables
//            //Libéllé de la table "recap" cible
//            string recapTableName="";
//            //bolléen pour construire la requête sql
//            bool premier=true;
//            bool firstfield=true;
//            bool firstgrouby=true;
//            bool firstorderby=true;
//            bool beginbyand =true;		
//            //Liste des annonceurs concurrents sélectionnés
//            string CompetitorAdvertiserAccessList="";
//            //Liste des familles possédant au moins un groupe ou une variété sélectionnée
//            string listVehicle="";
//            //Pour stocker les données générés par la requête
//            DataSet ds =null;
//            //Indique si la requête sql doit être construite et exécitée
//            bool buildSqlStatement=true;
//            #endregion

//            #region construction des listes de produits, media, annonceurs sélectionnés	
//            //listes de produits, media, annonceurs sélectionnés							
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

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//            recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
			
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName.ToString().Trim())){
//                /*Liste des familles ayant au moins une variété ou un groupe sélectionné.
//                 Cette donnée est nécesssaire pour calculer le total famille des produits sélectionnés.*/
//                if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                    listVehicle=WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(!WebFunctions.CheckedText.IsStringEmpty(listVehicle))buildSqlStatement=false;
//                }
//                if(buildSqlStatement){
//                    #region Mise en forme des mois de la période sélectionnée					
//                    string StudyMonths=IntervalMonthsStudy(webSession,true);
//                    #endregion
		
//                    #region Construction de la requête

//                    #region Close Select
//                    string sql="select";
			
//                    #region  selection annonceurs de références et concurrents
//                    //selection des annonceurs de références en accès			
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

//                    #region Tables concernées par la requete
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
//                    // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
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
//                        //Si la requête ne porte pas sur le total famille ou martché
//                        if(withRights){					
//                            #region Sélection de Produits
//                            // Sélection en accès
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
		
//                            // Sélection en Exception
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
		
//                    #region Execution de la requête
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour le tableau saisonalité: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
//            return(ds);
//        }

//        #endregion

//        #region DataSet Saisonnalité pour sortie graphique
//        /// <summary>
//        /// Fournit la liste des annonceurs,des références, les investissements mensuels (période N et N-1)
//        /// pour indicateur Saisonnalité, sur le total supports de l'univers ou marché ou famille.
//        /// </summary>
//        /// <remarks>Cette méthode est utilisée pour la présentation des données sous forme de graphique.</remarks>
//        /// <param name="webSession">Session du client</param>	
//        /// <param name="withAdvertisers">champs annonceurs</param>	
//        /// <param name="withRights">droits produits</param>
//        /// <returns>Données de saisonnalité (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifié le 30/11/04</history>
//        public static DataSet GetChartData(WebSession webSession, bool withAdvertisers, bool withRights) {	
			
//            #region variables
//            //Libéllé de la table "recap" cible
//            string recapTableName="";
//            //bolléen pour construire la requête sql
//            bool premier=true;
//            bool firstfield=true;
//            //bool firstgrouby=true;
//            //bool firstorderby=true;
//            bool beginbyand =true;		
//            //Liste des annonceurs concurrents sélectionnés
//            string CompetitorAdvertiserAccessList="";
//            //Liste des familles possédant au moins un groupe ou une variété sélectionnée
//            string listVehicle="";
//            //Pour stocker les données générés par la requête
//            DataSet ds =null;
//            //Indique si la requête sql doit être construite et exécitée
//            bool buildSqlStatement=true;
//            #endregion

//            #region construction des listes de produits, media, annonceurs sélectionnés		
//            //listes de produits, media, annonceurs sélectionnés							
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

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
			
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName.ToString().Trim())){
//                /*Liste des familles ayant au moins une variété ou un groupe sélectionné.
//                 Cette donnée est nécesssaire pour calculer le total famille des produits sélectionnés.*/
//                if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal && !withAdvertisers && !withRights){ 
//                    listVehicle=WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(!WebFunctions.CheckedText.IsStringEmpty(listVehicle))buildSqlStatement=false;
//                }
//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName) && buildSqlStatement){
//                    #region Mise en forme des mois de la période sélectionnée					
//                    string StudyMonths=IntervalMonthsStudy(webSession,false);
//                    #endregion
		
//                    #region Construction de la requête

//                    #region Close Select
//                    string sql="select";
			
//                    #region  selection annonceurs de références et concurrents
//                    if(withAdvertisers){
//                        //selection des annonceurs de références en accès			
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

//                    #region Tables concernées par la requete
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
			
//                    #region sélections
//                    //Cas du vehicle dans la table pluri
//                    string list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                        sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, !beginbyand);
//                        beginbyand=false;
//                    }
//                    // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
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
					
//                    //Si la requête ne porte pas sur le total famille ou marché
//                    if(withRights){					
//                        #region Sélection de Produits
//                        // Sélection en accès
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
		
//                        // Sélection en Exception
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
		
//                    #region Execution de la requête
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour le graphique saisonalité: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
//            return(ds);
//        }

//        #endregion

//        #region Dataset Saisonnalité - POUR TOTAUX MARCHES (AVEC PROCEDURES STOCKEES)
//        /// <summary>
//        /// Pour le total marché chaque ligne de la table de données recupérée correspond
//        ///à l'investissement mensuel,evolution par rapport au mois 
//        ///de la pérode N-1,nombre de références et le budget moyen ,1er annonceur avec son
//        ///investissement et SOV,1 ere référence avec son investissement et SOV.</summary>
//        ///<remarks> Traitement distinct du total famille pour optimiser les temps de réponses
//        /// par utilisation de procédures stockées</remarks>
//        /// <param name="webSession">Session du client</param>				
//        /// <returns>Données de saisonnalité (analyse sectorielle)</returns>
//        /// <history>[D. V. Mussuma]  Modifié le 24/11/04</history>
//        public static DataSet GetSeasonalityData(WebSession webSession) {	
//            #region variables
//            //annonceur concorrents en accès
//            string CompetitorAdvertiserAccessList="";
//            //groupe de données
//            DataSet ds = null; 
//            //est ce 1er terme requete
//            bool premier=true;	
//            //liste des mois étudiés période N		
//            ArrayList StudyMonthListArr=null;
//            //liste des mois étudiés période N-1
//            ArrayList previousYearStudyMonthsListArr=null;
//            //liste des mois étudiés Année actuelle 
//            ArrayList currentYearStudyMonthsListArr=null;
//            //mois étudié(s)
//            string StudyMonths="";
//            //mois étudié(s) pour la requete 2
//            string StudyMonthsForRequest2="";
//            //mois étudiés Année précédente
//            string previousYearStudyMonths="";
//            //mois étudiés Année actuelle 
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

//            #region construction des listes de produits, media, annonceurs sélectionnés
//            //listes de produits, media, annonceurs sélectionnés							
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
//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
//            #endregion

//            #region Mise en forme des mois de la période sélectionnée
//            //liste mois période N-1 et/ou N
//            StudyMonths=IntervalMonthsStudy(webSession,true);
//            //liste mois période (alias)
//            StudyMonthsForRequest2=ListMonthsStudy(webSession,false,1,true);
			
//            if(webSession.ComparativeStudy){				
//                //liste mois période N
//                currentYearStudyMonths=ListMonthsStudy(webSession,false,1,true);				
//                //liste mois période N-1
//                previousYearStudyMonths=ListMonthsStudy(webSession,true,1,true);
//            }
//            #endregion			

//            #region Construction de la requête
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                #region Requete 2
//                /* Requete 2 : Obtient pour chaque mois : l'annonceur, budget moyen,evolution,nombre de références
//                 * à partir des données fournies par la "requete 1".
//                 * */
//                sql="select";

//                #region  selection annonceurs de références et concurrents
//                //sélection des annonceurs de références en accès			
//                sql+="  id_advertiser,advertiser,nbRef ";
//                firstfield=false;
//                #endregion
//                //On récupere la chaine de caractère représentant chaque mois
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
//                /* Requete 1 : Obtient pour chaque mois : l'annonceur, budget moyen,evolution, nombre de références.				
//                 * */
//                //Debut Requete 1
//                #region Close Select
//                firstfield=true;
//                sql+="select";

//                #region  Champs annonceurs de références et concurrents
//                //selection des annonceurs de références en accès			
//                sql+="  distinct "+DBConstantes.Tables.RECAP_PREFIXE+".id_advertiser,"+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser ";
//                //Nombre de références
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

//                #region Tables concernées par la requete
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
//                // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,!beginbyand);	
//                    beginbyand=false;
//                }
//                //Si la requête ne porte pas sur le total marché			
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
//                //Si la requête ne porte pas sur le total famille ou martché																					
//                #region Sélection de Produits
//                // Sélection en accès
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
		
//                // Sélection en Exception
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
//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour le tableau saisonalité: "+sql,err));
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
//            // Table recap appelé
//            string recapTableName="";
//            // Mois étudiés
//            string StudyMonths="";
//            bool premier;
//            string sql="";
//            string list="";
////			string list2="";
//            string VehicleAccessList ="";
//            #endregion

//            #region Construction de la requête
			
//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
			
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


//            // Sélection de Produits
//            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
//                sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.RECAP_PREFIXE, true);						


	
//            #region sélection des médias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//            }
//            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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

//            #region Execution de la requête
//            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//            try{
//                ds=dataSource.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour les evolution: "+sql,err));
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
//        /// <param name="typeYear">type d'année</param>
//        /// <param name="tableType">type de table</param>
//        /// <returns>données palmares</returns>
//        public static DataSet GetPalmaresData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear,FrameWorkConstantes.PalmaresRecap.ElementType tableType){
			
//            #region Variables
//            DataSet ds=null;
//            // Table recap appelé
//            string recapTableName="";
//            // Mois étudiés
//            string StudyMonths="";
			
//            bool premier;
//            string sql="";
//            string list="";
////			string list2="";
//            string VehicleAccessList ="";
//            #endregion

//            #region Construction de la requête
			
//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
			
//            StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);
//            //sélection des 10 plus grand palmares

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

//            // Sélection de Produits
//            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
//                sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.RECAP_PREFIXE, true);						

//            #region sélection des médias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//            }
//            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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
//            //fin sélection des 10 plus grand palmares
//            #endregion

//            #region Execution de la requête
//            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//            try{
//                ds=dataSource.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour les palmares: "+sql,err));
//            }
//            #endregion			

//            return(ds);
//        }
		
//        #endregion		

//        #region DataSet Nouveautés
//        /// <summary>
//        /// Crée le tableau de résultats qui permettra de détecter les réels nouveaux produits
//        /// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
//        /// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.		
//        /// </summary>
//        /// <remarks>Il fournit les données  pour déterminer les nouveaux annonceurs ou les nouvellles références.
//        /// </remarks>
//        /// <example>
//        /// -Pour les nouvelles références l'appel est le suivant :
//        /// <code>dsNovelty=IndicatorDataAccess.getNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.product,false,true,true);</code>
//        /// -Pour les nouveaux annonceurs l'appel est le suivant :
//        /// <code> dsNovelty=IndicatorDataAccess.getNoveltyData(webSession,ConstResults.PalmaresRecap.ElementType.advertiser,true,false,true);</code>
//        /// </example>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">produit ou annonceur</param>
//        /// <param name="onAdvertiser">la requête porte sur les annonceurs</param>
//        /// <param name="onProduct">la requête porte sur les produits</param>
//        /// <param name="TotalType"></param>
//        /// <returns>données pour nouveautés</returns>
//        ///<history>[D. V. Mussuma]  Modifié le 24/11/04</history>
//        public static DataSet GetNoveltyData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,bool onAdvertiser, bool onProduct,bool TotalType){
			
//            #region variables
//            //Libéllé table recap
//            string recapTableName="";
//            //est ce premier terme de la requête
//            bool premier=true;
//            //est ce premier champ de la requête
//            bool firstfield=true;
//            //condition commence par "AND"		
//            bool beginbyand =true;
//            //liste d'éléments
//            string list;
//            //Mois étudiés
//            string NoveltyIntervalMonth="";
//            //mois courant
//            string currentMonth="";
//            //Liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //Groupe de données
//            DataSet ds =null;
//            //index année N	
//            string YearSelected="";
//            //index année N
//            int year=0;

//            #endregion

//            #region periode etudiée
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);			
//            GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);	
//            #endregion

			
//            if(HasData(PeriodEndDate)){
//                #region construction des listes de produits, media, annonceurs sélectionnés
//                //listes de produits, media, annonceurs sélectionnés							
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
//                //Determine la table recap à appeler
//                if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                    recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//                else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
//                #endregion

//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                    #region Construction de la requête

//                    #region Clause Select
//                    #region requête 2 : recupère les annonceurs ou les nouveaux produits
//                    //Pour récupérer les nouveaux éléments on vérifie que l'investissement depuis
//                    //le début de l'année est nul excepté pour le mois en cours
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
//                    //Somme des investissements des mois précédents jusqu'au début de l'année
//                    //Si la somme est égale à zero alors le produit ou l'annonceur est nouveau									
//                    NoveltyIntervalMonth = NoveltyIntervalMonthsStudy(webSession,TotalType);
//                    if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth)){
//                        if(!firstfield)sql+=",";
//                        sql+=" isInactive ";
//                    }
					
//                    firstfield=true;
//                    sql+=" from ( ";
//                    #region requête 1 : sélection annonceurs et produits 
//                    sql+="select";

//                    #region  selection annonceurs de références et concurrents et/ou selection produits
//                    //selection des annonceurs de références en accès
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

//                    #region sélections pour la période d'étude
//                    #region année N
//                    //Investissement mois
//                    if(!firstfield)sql+=",";					
//                     // mois actuel
//                    if(PeriodEndDate.Month==DateTime.Now.Month)
//                    sql+="  "+" sum(exp_euro_N_"+PeriodEndDate.AddMonths(-1).Month.ToString()+") "+WebFunctions.Dates.GetMonthAlias(PeriodEndDate.Month-1,WebFunctions.Dates.yearID(PeriodEndDate.AddMonths(-1).Date,webSession),3,webSession)+" ";
//                    else sql+="  "+" sum(exp_euro_N"+YearSelected+"_"+PeriodEndDate.Month.ToString()+") "+WebFunctions.Dates.GetMonthAlias(PeriodEndDate.Month,WebFunctions.Dates.yearID(PeriodEndDate,webSession),3,webSession)+" ";
//                    //autre mois

//                    //Somme des investissements des mois précédents jusqu'au début de l'année
//                    //Si la somme est égale à zero alors le produit ou l'annonceur est nouveau														
//                    if(WebFunctions.CheckedText.IsStringEmpty(NoveltyIntervalMonth)){
//                        if(!firstfield)sql+=",";
//                        sql+=NoveltyIntervalMonth;
//                        firstfield=false;
//                    }
					

//                    #endregion
//                    #endregion
				
//                    #region Tables concernées par la requete
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
													
//                    #region Sélection de Produits
//                        // Sélection en accès
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
		
//                        // Sélection en Exception
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
					
//                    #region sélection des médias
//                    list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                    if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                        sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                    }
//                    // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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

//                    #region Execution de la requête
//                    IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                    try{
//                        ds=dataSource.Fill(sql.ToString());
//                    }
//                    catch(System.Exception err){
//                        throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour les nouveautés: "+sql,err));
//                    }
//                    #endregion
					
//                }
//            }
			
//            return(ds);
//        }
//        #endregion

//        #region DataSet(s) Stratégie Média

//        #region Stratégie Média Calcule Investissements par média
//        /// <summary>
//        /// Charge les données pour créer la présentation graphique de stratégie média (analsyse sectorielle).
//        /// Recupère la répartition média sur le total de la période 
//        ///contenant les éléments ci-après :
//        ///en ligne :
//        ///l'investissement total famille ou marché ou univers  				
//        ///de la période N et N-1		
//        ///décliné par media, catégorie ,supports, annonceur.
//        /// </summary>
//        ///<remarks>
//        /// Elle est utilisée aussi bien que pour remonter les données du total univers, marché, famille
//        ///que pour celles des annonceurs de références ou concurrents.
//        ///Exemples d'appel en fonction du type de données ciblées :
//        ///</remarks>
//        /// <example>-Pour recupérer les investissements des annonceurs de références et concurrents sur la période N et N-1
//        /// <code>dsAdvertiser = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,true);
//        /// </code>
//        /// -Pour recupérer les investissements pour les totaux univers par media,catégorie, support sur la période N et N-1
//        /// <code>dsTotalUniverse = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,false);</code>
//        /// -Pour recupérer les investissements pour les totaux marchés ou famille par media,catégorie, support sur la période N et N-1
//        /// <code>dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.marketTotal,false);</code>
//        /// </example>		
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">produit ou annonceur</param>
//        /// <param name="comparisonCriterion">critère de comparaison</param>
//        /// <param name="RefenceOrCompetitorAdvertiser">calcul pour annonceur de références ou concurrents</param>
//        /// <returns>groupe de données pour stratégie média</returns>
//        /// <history>[D. V. Mussuma]  Modifié le 28/01/05</history>
//        public static DataSet getMediaStrategyData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,bool RefenceOrCompetitorAdvertiser){
			
//            #region variables
//            //libéllé table recap
//            string recapTableName="";
//            // Mois étudiés
//            string StudyMonths="";
//            //Est ce premier membre de la requête
//            bool premier=true;						
//            //Est ce premier membre de la requête à trier
//            bool firstorderby=true;			
//            //annonceur concurrents
//            string CompetitorAdvertiserAccessList="";
//            //groupe de données
//            DataSet ds = null;
//            //Chaine sql
//            string sql="";			
//            //index pour alias sous requete
//            string indexTb0="1";
//             bool buildStatement=true;
//            string totalSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs sélectionnés
//            //listes de produits, media, annonceurs sélectionnés							
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

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException (GestionWeb.GetWebWord(1372,webSession.SiteLanguage)));						
			
//            //Vérifie si possède les données pour les familles sélectionnées
//            if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                totalSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                if(!WebFunctions.CheckedText.IsStringEmpty(totalSector))
//                    buildStatement=false;
//            }

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName) && buildStatement){
//                //Periode de l'étude
//                #region periode etudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
//                //Mois concernés
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);
//                #endregion

//                #region construction de la requête																		
//                #region REQUETE 2				
//                /* Table REQUETE 2 (requete principal) :	
//                 Recupère la répartition média sur le total de la période 
//                 contenant les éléments ci-après :
//                 en ligne :
//                 l'investissement total famille ou marché ou univers  				
//                 de la période N et N-1		
//                 décliné par media, catégorie ,supports, annonceur.
//                 */
//                //Niveau famille
//                sql+=" select ";
//                premier=false;				
//                //Champs média
//                sql+=GetMediaStrategyFieldsForMediaDetail(webSession,"maxt","");				
//                //Champs produits
//                sql+=GetProductFieldsForMediaStrategy(tableType,"","maxt","maxt",false,".");
//                //investissements sur période N et N-1
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
//                #region REQUETE 1 : répartition média sur le total de la période et par annonceur
//                    sql+=MediaStrategySubQuery(webSession,recapTableName,comparisonCriterion,tableType,StudyMonths,indexTb0,GroupAccessList,GroupExceptionList,SegmentAccessList,SegmentExceptionList,CategoryAccessList,MediaAccessList,AdvertiserAccessList,RefenceOrCompetitorAdvertiser);
//                #endregion
//                sql+=" ) maxt,"; //Table genérée par la requete 1 :répartition média sur le total de la période et par annonceur
//                //Tables médias
//                sql+=GetMediaStrategyTablesForMediaDetail(webSession);

//                sql+=" where ";

//                #region jointure et choix de la langue
//                //jointure et choix de la langue
//                sql+="  "+GetMediaStrategyJointForMediaDetail(webSession,"maxt");					
//                #endregion											

//                #region tri des enregistrements 
//                //tri
//                sql+=" order by ";
//                //tri des médias
//                sql+=GetMediaStrategyOrderForMediaDetail(webSession,"maxt","asc",true);
//                firstorderby=false;				
//                //tri des produits
//                if(!firstorderby)sql+=" ,";
//                sql+=GetProductByOrderForMediaStrategy(tableType,"maxt","asc");
//                #endregion

//                #endregion											

//                #endregion

//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    return(dataSource.Fill(sql.ToString()));
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour startégie media: "+sql,err));
//                }
//                #endregion	
//            }
			

//            return (ds);
//        }
//        #endregion
		
//        #region Stratégie média premier annonceur ou référence
//        /// <summary>
//        /// Goupe de données qui fournit les premiers annonceurs de références ou concurrents par média
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table générer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">critère de comparaison (univers ou marché  ou famille)</param>
//        /// <param name="mediaLevel">niveau média (média ou catégorie ou support)</param>
//        /// <history>[D. V. Mussuma]  Modifié le 28/01/05</history>
//        /// <returns>requete sql</returns>
//        public static DataSet GetMediaStrategy1stElmntData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel){
//            #region variables
//            //Groupe de données
//            DataSet ds = null; 
//            string sql="";
//            //sous requete sql
//            string sqlChild="";
//            //préfixe sous requete 
//            string sr="r2";			
//            #endregion

//            #region construction de la requête

//            #region table virtuelle 1 et 2 : les premiers anonceurs ou références par média
//            //Max investissments des premiers annonceurs ou références par média
//            sqlChild=MediaStrategy1stElmntSql(webSession,tableType,comparisonCriterion,mediaLevel);
//            if(WebFunctions.CheckedText.IsStringEmpty(sqlChild.ToString().Trim())){
//                sql+=" select ";
//                //Choix champ média
//                sql+=getFieldForMediaLevel(mediaLevel,sr,".");						
//                //Choix champ produit ou annonceur
//                sql+=GetProductFieldsForMediaStrategy(tableType,"",sr,sr,false,".");
//                //investissement premier élément	
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
//                //investissments des premiers annonceurs ou références par média				
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
				
//                #region Exécution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour startégie media: "+sql,err));
//                }
//                #endregion

//            }
//            #endregion

//            #endregion
		
//            return ds;
//        }
//        /// <summary>
//        /// Requete sql pour récupérer les premiers annonceurs de références ou concurrents par média
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table générer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">critère de comparaison (univers ou marché  ou famille)</param>
//        /// <param name="mediaLevel">niveau média (média ou catégorie ou support)</param>
//        /// <returns>requete sql</returns>
//        public static string MediaStrategy1stElmntSql(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel){
			
//            #region variables
//            // Mois étudiés
//            string StudyMonths="";
//            //liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //requête sql
//            string sql="";
//            //Libéllé table recap cible
//            string recapTableName="";
//            //est t'il le premier champ de la requête
//            bool premier=true;
//            //liste d'éléments
//            string list="";
//            //est t'il le premier terme à grouper
//            bool firstgrouby=true;			
//            //liste des familles ayant au moins un élément sélectionné
//            string listSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs sélectionnés
//            //listes de produits, media, annonceurs sélectionnés							
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
//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));						
//            #endregion

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                //Periode de l'étude
//                #region periode etudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();
				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//                #endregion

//                //Liste des mois étudiés
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);

//                #region construction de la sous requête
		
//                sql+="select ";

//                #region champs de la requête
//                // champs du niveau média ( média ou catégorie  ou support)
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

//                #region champs de la sous requête
//                //Champs du niveau média ( média ou catégorie  ou support)
//                sql+=getFieldForMediaLevel(mediaLevel,DBConstantes.Tables.RECAP_PREFIXE,".");								
//                premier=false;
//                //Champs produit
//                sql+=GetProductFieldsForMediaStrategy(tableType,"",DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,premier,".");
//                //liste des mois étudiés
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=StudyMonths;
//                    premier=false;
//                }
//                #endregion

//                sql+=" from  ";

//                #region table concernées 				
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

//                //Sélections des familles pour le total famille
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(WebFunctions.CheckedText.IsStringEmpty(listSector))
//                        sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector in ("+listSector+") ";
//                }

//                //Sélection produit pour le calcul du total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region Sélection de Produits (groupes et/ou variétés) 
//                    // Sélection en accès
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
			
//                    // Sélection en Exception
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
//                #region droits média
//                // cas du vehicle dans la table pluri
//                list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession, WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                }
//                // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,true);	
//                }

//                //Droits Parrainage TV
//                if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += " and   " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ";
//                }
//                #endregion				

//                #region sélection des médias 				
//                // Media
//                //Non utilisé si sélection plurimedia
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
//                //Cas Références
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
//                //Cas Références
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
		
//        #region récupérer les premiers annonceurs de références ou concurrents pour plurimédia
//        /// <summary>
//        /// Requete sql pour récupérer les premiers annonceurs de références ou concurrents pour plurimédia
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="tableType">type de la table générer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">critère de comparaison (univers ou marché  ou famille)</param>		
//        /// <returns>requete sql</returns>
//        public static DataSet GetPluriMediaStrategy1stElmntData(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion){
//            #region variables
//            //Groupe de données
//            DataSet ds = null; 
//            string sql="";
//            //sous requete sql
//            string sqlChild="";			
//            #endregion

//            #region table virtuelle 1 et 2 : les premiers anonceurs ou références par média
//            //Max investissments des premiers annonceurs ou références pour plurimedia
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
//                sql+=" r1.total_N "; //investissement premier élément									
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

//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour startégie media: "+sql,err));
//                } 				
//                #endregion
//            }
//            #endregion

//            return ds;
//        }
//        #endregion

//        #region 1 er élément pour plurimédia 
//        /// <summary>
//        /// Requete sql pour récupérer les premiers annonceurs de références ou concurrents pour plurimédia
//        /// </summary>
//        /// <param name="webSession">sesion du client</param>
//        /// <param name="tableType">type de la table générer (annonceur ou produit)</param>
//        /// <param name="comparisonCriterion">critère de comparaison (univers ou marché  ou famille)</param>
//        /// <returns>requete sql</returns>
//        public static string PluriMediaStrategy1stElmntSql(WebSession webSession,FrameWorkConstantes.PalmaresRecap.ElementType tableType,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion){
			
//            #region variables
//            // Mois étudiés
//            string StudyMonths="";
//            //liste des annonceurs concurrents
//            string CompetitorAdvertiserAccessList="";
//            //requête sql
//            string sql="";
//            //Libéllé table recap cible
//            string recapTableName="";
//            //est t'il le premier champ de la requête
//            bool premier=true;
//            //liste d'éléments
//            string list="";
//            //est t'il le premier terme à grouper
//            bool firstgrouby=true;			
//            //liste des familles ayant au moins un élément sélectionné
//            string listSector="";
//            #endregion

//            #region construction des listes de produits, media, annonceurs sélectionnés
//            //listes de produits, media, annonceurs sélectionnés							
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
//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));						
//            #endregion

//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
//                //Periode de l'étude
//                #region periode etudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();
				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
//                #endregion

//                //Liste des mois étudiés
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);

//                #region construction de la requête
//                sql+=" select ";

//                #region champs de la  requête				
//                //choix produit
//                if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){										
//                    sql+=""+DBConstantes.Tables.PRODUCT_PREFIXE+".product";
//                    premier=false;
//                }				
//                else{//choix annonceur										
//                    sql+=""+DBConstantes.Tables.ADVERTISER_PREFIXE+".advertiser";
//                    premier=false;
//                }	
//                //liste des mois étudiés
//                if(WebFunctions.CheckedText.IsStringEmpty(StudyMonths)){
//                    if(!premier)sql+=" ,";
//                    sql+=StudyMonths;
//                    premier=false;
//                }
//                #endregion

//                sql+=" from ";

//                #region table concernées 				
//                sql+=GetMediaStrategyProductTables(recapTableName,tableType,"");
//                #endregion

//                sql+=" where  ";

//                #region jointure et choix de la langue 
//                //Jointures et choix de la langue				
//                sql+="  "+GetMediaStrategyJointForProduct(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,tableType,"");
//                #endregion

//                //Sélections des familles pour le total famille
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,GroupAccessList,SegmentAccessList);
//                    if(WebFunctions.CheckedText.IsStringEmpty(listSector))
//                        sql+=" and "+DBConstantes.Tables.RECAP_PREFIXE+".id_sector in ("+listSector+") ";
//                }

//                //Sélection produit pour le calcul du total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    #region Sélection de Produits (groupes et/ou variétés) 
//                    // Sélection en accès
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
			
//                    // Sélection en Exception
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
//                #region sélection des médias 

//                #region droits média
//                // cas du vehicle dans la table pluri
//                list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//                if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,true);
//                }
//                // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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
				
//                //Cas Références
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

//        #region Récupère total Marché où total famille
		
//        /// <summary>
//        /// Fournit le total marché ou le total famille
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="typeYear">année courante ou année précédente</param>
//        /// <returns>total marché ou le total famille</returns>
//        public static Int64 getTotalForPeriod(WebSession webSession, FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){
			
//            #region Variables
//            string sql="";			
//            string VehicleAccessList ="";
//            string list;
//            string list2;
//            string StudyMonths;		
//            // Table recap appelé
//            string recapTableName="";
//            long total=0;

//            #endregion
		
//            #region Construction de la requête 

//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //listes de produits, media, annonceurs sélectionnés							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResultSegmentLevel((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));

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

//            #region sélection des médias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql += WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, true);
//            }
//            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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

//        #region Récupère total Marché où total famille
		
//        /// <summary>
//        /// Fournit le total marché ou le total famille pour le mois courant
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="typeYear">année courante ou année précédente</param>
//        /// <returns></returns>
//        public static Int64 getTotalForMonth(WebSession webSession, FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){
			
//            #region Variables
//            string sql="";			
//            string VehicleAccessList ="";
//            string list;
//            string list2;
//            string StudyMonths;		
//            // Table recap appelé
//            string recapTableName="";
//            long total=0;
//            #endregion
		
//            #region Construction de la requête 

//            VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();

//            //listes de produits, media, annonceurs sélectionnés							
//            DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);

//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList)) {
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResultSegmentLevel((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));
//            }
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));

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

		
//            #region sélection des médias
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql += "  " + WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, true);
//            }
//            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
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
		
//        #region Récupère total Marché où total famille ou univers pour Saisonnalite
		
//        /// <summary>
//        /// Fournit le total marché ou le total famille ou univers
//        /// </summary>
//        /// <param name="webSession">session</param>
//        /// <param name="comparisonCriterion">Critère de commparaison pour le calcul des totaux (famille ou marché ou univers)</param>
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
		
//            #region construction des listes de produits, media, annonceurs sélectionnés
//            //listes de produits, media, annonceurs sélectionnés							
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
//            //Determine la table recap à appeler
//            if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList))			
//                recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(VehicleAccessList));			
//            else throw (new SectorAnalysisIndicatorDataAccessException ("Impossible d'identifier la table recap à utiliser."));
//            #endregion

//            #region Mise en forme des mois de la période sélectionnée
//            //liste mois période N-1 et/ou N
//            StudyMonths=IntervalMonthsStudy(webSession,true);
//            //liste mois période (alias)
//            StudyMonthsForRequest2=ListMonthsStudy(webSession,false,1,true);
			
//            if(webSession.ComparativeStudy){				
//                //liste mois période N
//                currentYearStudyMonths=ListMonthsStudy(webSession,false,1,true);				
//                //liste mois période N-1
//                previousYearStudyMonths=ListMonthsStudy(webSession,true,1,true);
//            }
//            #endregion			

//            #region Periode étude
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();				
//            DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
////			DateTime PeriodBeginningDate = WebFunctions.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
////			DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
//            #endregion

//            #region Construction de la requête
//            if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)) {
//                if(PeriodEndDate.Month - PeriodBeginningDate.Month >=0){
					
					
//                    #region Close Select
//                    #region Mois traité
//                    //Pour chaque mois de la période traitée
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
						
//                        //année N						
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
//                        //Investissement année N												
//                        if(!firstfield)sql+=",";
//                        sql+=" sum(exp_euro_N"+strYear+"_"+i+") as Total_N";
//                        firstfield=false;
//                        //Investissement année N-1						 
//                        if(WebFunctions.Dates.yearID(PeriodEndDate.AddYears(-1),webSession)>0) strYear_1=WebFunctions.Dates.yearID(PeriodEndDate.AddYears(-1),webSession).ToString().Trim();
//                        if(webSession.ComparativeStudy){
//                            if(!firstfield)sql+=",";
//                            sql+=" sum(exp_euro_N"+strYear_1+"_"+i+") as Total_N_1";
//                            firstfield=false;
//                        }
//                        try{
//                            //Nombre de références par mois	
//                            if(WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal==comparisonCriterion){
								
//                                if( WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList.ToString().Trim())) cat = "'"+CategoryAccessList+"'";								
//                                if( WebFunctions.CheckedText.IsStringEmpty(MediaAccessList.ToString().Trim())) med = "'"+MediaAccessList+"'";
//                                if(!firstfield)sql+=",";
//                                sql+=" pkg_recap_test.NB_REF_BY_MONTH_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as nbref";
//                                firstfield=false;
//                                //Premiere référence
//                                if(!firstfield)sql+=",";							
//                                sql+="pkg_recap_test.FIRST_PRODUCT_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as id_product";
//                                firstfield=false;
//                                //Premier annonceur
//                                if(!firstfield)sql+=",";
//                                sql+="pkg_recap_test.FIRST_ADVERTISER_TOTAL_MARKET('exp_euro_N"+strYear+"_"+i+"','"+recapTableName+"',"+cat+","+med+") as id_advertiser, "+i+" as Mois";
//                                firstfield=false;
//                            }
							
//                        }catch (Exception firtsErr){
//                            throw (new SectorAnalysisIndicatorDataAccessException("Impossible de déterminer le premier annonceur, produit et nombre de références pour le total calculé."+firtsErr.Message));
//                        }
											

//                        sql+=" from";

//                        #region Tables concernées par la requete
//                        //TABLE RECAP	
//                        sql+="  "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+"";
						
						
//                        premier=true;
//                        #region where pour requete 1
////						sql+=" where ";												
////						// Langue recap				
////						sql+=" "+DBConstantes.Tables.RECAP_PREFIXE+".id_language_i="+webSession.SiteLanguage.ToString();				
////						beginbyand=true;

//                        #region selections
//                        //Si la requête ne porte pas sur le total marché			
//                        //selection des categories et/ou supports
//                        beginbywhere=true;
//                        if(WebFunctions.CheckedText.IsStringEmpty(CategoryAccessList) || WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
//                            if(beginbywhere)sql+=" where  ";		
////							sql+="  "+GetMediaSelection(CategoryAccessList,MediaAccessList,!beginbyand);
//                            sql+="  "+WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,false);

//                            beginbyand=false;
//                            beginbywhere=false;
//                        }
										
//                        //Si la requête ne porte  sur le total univers
//                        if(webSession.ComparaisonCriterion ==WebConstantes.CustomerSessions.ComparisonCriterion.universTotal ){						
//                            #region Sélection de Produits
//                            // Sélection en accès
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
		
//                            // Sélection en Exception
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
//                        #region sélection des familles
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
//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
//                try{
//                    ds=dataSource.Fill(sql.ToString());
//                }
//                catch(System.Exception err){
//                    throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données"+sql,err));
//                }
//                #endregion				
//            }

//            return ds;
//        }
//        #endregion

//        #region Méthodes internes
		
//        #region liste mois de la période de l'étude N et optionnellment N-1
//        /// <summary>
//        /// Obtient la chaine de caractère representant la période de l'étude
//        /// Elle est utilisé dans la requete pour la selection des mois
//        /// </summary>
//        /// <param name="webSession">session client</param>	
//        /// <param name="withPreviousYearMonths">booléen specifiant la chaine retournée</param>	
//        /// <returns>chaine de caractère de la période d'étude</returns>
//        public static string IntervalMonthsStudy(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, bool withPreviousYearMonths){
//            #region Mise en forme des dates
//            //Determine la période de l'étude
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;
			
//            int downLoadDate=webSession.DownLoadDate;
			
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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

//        #region liste mois de la période étudiée N et/ou N-1
//        /// <summary>
//        /// Obtient la chaine de caractère representant la période de l'étude
//        /// Elle est utilisé dans la requete pour la selection des mois
//        /// </summary>
//        /// <param name="webSession">session client</param>	
//        /// <param name="nbYear">Nombre d'années</param>
//        /// <param name="onlyAlias">Sans alias</param>
//        /// <param name="withPreviousYearMonths">booléen specifiant la chaine retournée</param>	
//        /// <returns>chaine de caractère de la période d'étude</returns>
//        public static string ListMonthsStudy(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, bool withPreviousYearMonths, int nbYear,bool onlyAlias){
//            #region Mise en forme des dates
//            //Determine la période de l'étude
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;
			
//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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
//                //On ramène les mois de la période N et/ou N-1
//                switch (nbYear){					
//                    case 1 :
//                        for(int i=StartMonth;i<=EndMonth;i++) {
//                            //On retourne uniquement les mois de la période N
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
//                            //Recupération de la periode N-1 pour etude comparative
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
//                        throw (new SectorAnalysisIndicatorDataAccessException("Impossible d'identifier la période à étudier."));
//                }
//            }	
//            #endregion

//            return StudyMonths;
//        }
//        #endregion     		
		
//        #region période étudiée pour indicateur évolution
//        /// <summary>
//        /// Obient la période pour l'indicateur Evolution
//        /// </summary>
//        /// <param name="webSession">sesssion client</param>
//        /// <returns>liste des mois</returns>
//        public static string EvolutionIntervalMonthsStudy(WebSession webSession){
		
//            #region Mise en forme des dates
//            //Determine l'evolution
//            string StudyMonthsEvolution="";
//            // Recupère l'investissement de la période N
//            string StudyMonths="";
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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
		
//        #region période étudiée pour total marché ou univers
//        /// <summary>
//        /// Utilisée dans le total univers et total marché
//        /// </summary>
//        /// <param name="webSession">Session</param>
//        /// <param name="typeYear">type d'année (courante, précédente)</param>
//        /// <returns></returns>
//        public static string IntervalMonthsStudyForTotal(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){

//            #region Mise en forme des dates
//            //Determine la période de l'étude
//            string StudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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

//        #region Mois courant étudié
//        /// <summary>
//        /// Utilisée dans le total univers et total marché
//        /// </summary>
//        /// <param name="webSession">Session</param>
//        /// <param name="typeYear">type d'année (courante, précédente)</param>
//        /// <returns></returns>
//        public static string CurrentMonthStudyForTotal(WebSession webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected typeYear){

//            #region Mise en forme des dates
//            //Determine la période de l'étude
//            string StudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";
//            int year=0;
//            int comparativeYear=0;

//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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

//        #region periode etudiée pour nouveaux produits ou annonceurs
//        /// <summary>
//        /// Genère la chaine de cararctère nécessaire au sélections d'investissements
//        /// des nouveaux produits
//        /// </summary>
//        /// <param name="webSession">session client</param>
//        /// <param name="TotalType">total univers ou marché ou famille</param>
//        /// <returns>chaine selection investissements</returns>
//        public static string NoveltyIntervalMonthsStudy(WebSession webSession,bool TotalType){
//            #region Mise en forme des dates
//            //Determine la période de l'étude
//            string StudyMonths="";			
//            //Periode etude comparative
//            string ComparativeStudyMonths="";
//            string YearSelected="";
//            string ComparativeYearSelected="";			
//            int year=0;
//            int comparativeYear=0;			
//            // bool hasData=true;

//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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
//                    //Determine inactivité depuis le début de l'année
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
//        /// Les nouveaux produits ou annonceurs ne sont disponibles que passé
//        /// le 20 février s'il s'agit de l'année actuelle
//        /// </summary>
//        /// <param name="PeriodEndDate">fin de période d'étude</param>
//        /// <returns>booléen </returns>
//        public static bool HasData(DateTime PeriodEndDate){
//            //Différence en jours, heurs, et minutes.
//            DateTime oldDate = new DateTime(DateTime.Now.Year,3,20);
//            TimeSpan ts = PeriodEndDate - oldDate;
//            // Difference en jours.
//            int differenceInDays = 1; 
//            //L'etude s'effectue sur un mois complet
//            if(!PeriodEndDate.Equals(null) ){
//                //L'etude s'effectue sur un mois complet
//                //Si année étudiée == année actuelle, il n'y pas de données disponibles avant le 20 mars												
//                if(PeriodEndDate.Year.Equals(DateTime.Now.Year) || PeriodEndDate.Month==1 || PeriodEndDate.Month==2 || PeriodEndDate.Month==3)
//                differenceInDays = ts.Days;	
//                if(differenceInDays <=0 )return false;
//                else return true;
//            }	
//            else return false;
//        }
//        #endregion

//        #region mois etude comparative année N-1
//        /// <summary>
//        /// Donne la liste des mois pour année N-1
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

//            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//            //du dernier mois dispo en BDD
//            //traitement de la notion de fréquence
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

//        #region identifiant de l'année : 0==N , 1==N-1,2==N-2
//        /// <summary>
//        /// Obtient l'identifiant de l'année sélectionnée  : 0==N , 1==N-1,2==N-2
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="YearSelected">année sélectionné</param>
//        /// <param name="year">identifiant année sélectionné</param>
//        /// <param name="PeriodBeginningDate">date de début</param>
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
	
//        #region Sous REQUETE 1 : répartition média sur le total de la période et par annonceur
//        /// <summary>
//        ///Sous REQUETE 1:	récupère la répartition média sur le total de la période  contenant les éléments ci-après :
//        ///en ligne :
//        ///l'investissement total famille ou marché ou univers  de la période N et N-1 décliné par media, catégorie ,supports, annonceur.													
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="recapTableName">nom de la table de données</param>
//        /// <param name="comparisonCriterion">critère de comparaison (total univers, famille ou marché)</param>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        /// <param name="StudyMonths">période étudiée</param>
//        /// <param name="indexSubQuery">index de la sous requête</param>
//        /// <param name="GroupAccessList">groupes en accès</param>
//        /// <param name="GroupExceptionList">groupes en exceptions</param>
//        /// <param name="SegmentAccessList">variétés en accès</param>
//        /// <param name="SegmentExceptionList">variétés en exceptions</param>
//        /// <param name="CategoryAccessList">catégories en accès</param>
//        /// <param name="MediaAccessList">supports en accès</param>
//        /// <param name="AdvertiserAccessList">annonceurs en accès</param>
//        /// <param name="RefenceOrCompetitorAdvertiser">choix annonceur de références ou concurrents</param>
//        /// <returns>répartition média sur le total de la période et par annonceur</returns>
//        private static string MediaStrategySubQuery(WebSession webSession,string recapTableName,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkConstantes.PalmaresRecap.ElementType tableType,string StudyMonths, string indexSubQuery,string GroupAccessList,string GroupExceptionList,string SegmentAccessList,string SegmentExceptionList,string CategoryAccessList,string MediaAccessList,string AdvertiserAccessList,bool RefenceOrCompetitorAdvertiser){
			
//            #region variables
//            string sql="";
//            bool premier =true;
//            bool firstgrouby=true;
//            string dot=".";
//            string list="";
//            string SortOrder="asc";	
			
//            #endregion
			
//            //champs média
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

//            #region tables concernés
//            sql+=GetMediaStrategyProductTables(recapTableName,tableType,indexSubQuery);				
//            #endregion

//            sql+=" where  "; 

//            //Jointures et choix de la langue
//            sql+=GetMediaStrategyJointForProduct(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.PRODUCT_PREFIXE,DBConstantes.Tables.ADVERTISER_PREFIXE,tableType,indexSubQuery);			
			
//            #region sélection produit
//            //Sélection produit
//            sql+=WebFunctions.SQLGenerator.GetRecapProductSelection(comparisonCriterion,recapTableName,indexSubQuery,GroupAccessList,GroupExceptionList,SegmentAccessList,SegmentExceptionList,AdvertiserAccessList,RefenceOrCompetitorAdvertiser,true);						
//            #endregion

//            #region droits média
//            // cas du vehicle dans la table pluri
//            list = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                sql+="  "+WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,true);
//                //liste des média autorisés
////				sql+="  and "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+".id_vehicle in ("+webSession.CustomerLogin.getListIdVehicle()+") ";
//            }
			
//            // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
//            if(list.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
//                sql+="  "+WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery,true);	
//            }

//            //Droits Parrainage TV
//            if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                sql += " and   " + DBConstantes.Tables.RECAP_PREFIXE + indexSubQuery + ".id_category not in (68)  ";
//            }
//            #endregion

//            #region sélection des médias										
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
//            //Regroupement des médias
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

//        #region champs pour le niveau de détail média
//        /// <summary>
//        ///Champs média de la requête en fonction du niveau média
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        /// <param name="indexSubQuery">index sous requête</param>
//        /// <returns>champs média à afficher</returns>
//        private static string GetFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+PREFIXE+indexSubQuery+".id_category  ";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+PREFIXE+indexSubQuery+".id_category, "+PREFIXE+indexSubQuery+".id_media ";
//                default:
//                     throw (new SectorAnalysisIndicatorDataAccessException ("getFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery)-->Impossible d'initialiser les champs média de la requête."));					
//            }			
//        }
//        /// <summary>
//        /// Champs média de la requête en fonction du niveau média
//        /// </summary>
//        /// <param name="mediaLevel">niveau média</param>
//        /// <param name="PREFIXE">préfixe de la table de données</param>
//        /// <param name="dot">point (caractère) si champs doit être afficher avec le préfixe de la table</param>
//        /// <returns>champ média à afficher</returns>
//        private static string getFieldForMediaLevel(FrameWorkConstantes.MediaStrategy.MediaLevel mediaLevel,string PREFIXE,string dot){
//            switch(mediaLevel){
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.vehicleLevel:
//                    return PREFIXE+dot+"id_vehicle ";
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.categoryLevel:
//                    return PREFIXE+dot+"id_category";
//                case FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel:
//                    return PREFIXE+dot+"id_media";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getFieldForMediaLevel(FrameWorkConstantes.MediaStrategy.MediaLevel.mediaLevel mediaLevel)-->Impossible d'initialiser les champs média de la requête."));					
//            }
//        }
//        #endregion

//        #region champs pour le niveau de détail média pour stratégie média
//        /// <summary>
//        ///Champs média de la requête en fonction du niveau média pour média stratégie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        /// <param name="indexSubQuery">index sous requête</param>
//        /// <returns>champs média à afficher</returns>
//        private static string GetMediaStrategyFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle ,"+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+PREFIXE+indexSubQuery+".id_category,"+DBConstantes.Tables.CATEGORY_PREFIXE+".category";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return "  "+PREFIXE+indexSubQuery+".id_vehicle, "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+PREFIXE+indexSubQuery+".id_category, "+DBConstantes.Tables.CATEGORY_PREFIXE+".category,"+PREFIXE+indexSubQuery+".id_media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".media";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyFieldsForMediaDetail(WebSession webSession,string PREFIXE,string indexSubQuery)-->Impossible d'initialiser les champs média de la requête."));					
//            }			
//        }
//        #endregion

//        #region Champs produit pour statégie média
//        /// <summary>
//        /// Champs produit pour statégie média
//        /// </summary>
//        /// <param name="tableType">type de table ("advertiser" ou "product")</param>		
//        /// <param name="indexSubQuery">index de la sous requête</param>
//        /// <param name="ADVERTISER_PREFIXE">préfixe annonceur</param>	
//        /// <param name="PRODUCT_PREFIXE">préfixe produit</param>	
//        /// <param name="premier">vrai si c'est le premier champ</param>
//        /// <param name="dot">point (caractère) si champs doit être afficher avec le préfixe de la table</param>
//        /// <returns>champs produits</returns>
//        private static string GetProductFieldsForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery,string PRODUCT_PREFIXE,string ADVERTISER_PREFIXE,bool premier,string dot){
//            string sql="";
//            //Cas Références
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

//        #region sélection des média
////		/// <summary>
////		/// sélection  média 
////		/// </summary>
////		/// <param name="CategoryAccessList">catégories en accès</param>
////		/// <param name="MediaAccessList">supports en accès</param>	
////		/// <param name="beginbyand">vrai si commence par "and"</param>	
////		/// <returns>sélection média</returns>
////		private static string GetMediaSelection(string CategoryAccessList,string MediaAccessList,bool beginbyand){
////			return WebFunctions.SQLGenerator.GetRecapMediaSelection(CategoryAccessList,MediaAccessList,"",beginbyand);
////		}

//        #region sélection  média pour statégie média
////		/// <summary>
////		/// sélection  média pour statégie média
////		/// </summary>
////		/// <param name="CategoryAccessList">catégories en accès</param>
////		/// <param name="MediaAccessList">supports en accès</param>
////		/// <param name="indexSubQuery">index de la sous requête</param>
////		/// <param name="beginbyand">vrai si commence par "and"</param>
////		/// <returns>sélection média</returns>
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

//        #region regroupement des produits pour stratégie média
//        /// <summary>
//        /// Regroupement des produits pour stratégie média.
//        /// </summary>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requete</param>
//        /// <returns>produit regroupés</returns>
//        private static string GetProductByGroupForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery){
//            switch(tableType){
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product:
//                    //Cas Références
//                    return " "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+".id_product, "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+".product  ";
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser:
//                    //Cas Annonceur
//                    return " "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+".id_advertiser, "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+".advertiser";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getProductByGroupForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string indexSubQuery)-->Impossible de déterminer le type de l'élément de la nomenclature produit."));					
//            }			
//        }
//        #endregion

//        #region Tables pour le niveau de détail média pour stratégie média
//        /// <summary>
//        ///Tables média de la requête en fonction du niveau média pour média stratégie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>		
//        /// <returns>champs média à afficher</returns>
//        private static string GetMediaStrategyTablesForMediaDetail(WebSession webSession){			
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+"";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+"";
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return " "+DBConstantes.Schema.RECAP_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+" , "+DBConstantes.Schema.RECAP_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+"";
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyTablesForMediaDetail(WebSession webSession)-->Impossible d'initialiser les champs média de la requête."));					
//            }						
//        }
//        #endregion

//        #region Tables produits pour stratégie média
//        /// <summary>
//        /// Choix table de la nomenclature produit
//        /// </summary>
//        /// <param name="recapTableName">table recap</param>
//        /// <param name="tableType">type de table ("advertisr" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requête (éventuellement)</param>
//        /// <returns>nom de la table produit</returns>
//        private static string GetMediaStrategyProductTables(string recapTableName,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType tableType,string indexSubQuery){
//            string sql="";
//            //TABLE RECAP ciblée
//            sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+"."+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE+indexSubQuery+" , " ;
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
//                //TABLE PRODUITS (PRODUCT)
//                sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".product "+DBConstantes.Tables.PRODUCT_PREFIXE+indexSubQuery+"";					
//                //TABLE ANNONCEUR (ADVERTISER)
//            else sql+=" "+DBConstantes.Schema.RECAP_SCHEMA+".advertiser "+DBConstantes.Tables.ADVERTISER_PREFIXE+indexSubQuery+" ";				
			
//            return sql;
//        }
//        #endregion

//        #region Jointures pour le niveau de détail média pour stratégie média
//        /// <summary>
//        ///Jointures média de la requête en fonction du niveau média pour média stratégie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="SubQueryAlias">alias de la sous requête</param>		
//        /// <returns>jointures des média en fonction du niveau sélectionné</returns>
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
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyTablesForMediaDetail(WebSession webSession)-->Impossible d'initialiser les champs média de la requête."));					
//            }							
//        }
//        #endregion

//        #region Jointures des éléments la sélection produit pour stratégie média
//        /// <summary>
//        /// Jointures pour la sélection produit
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="Tables_RECAP_PREFIXE">prefixe de la table recap</param>
//        /// <param name="PRODUCT_PREFIXE">prefixe produit</param>
//        /// <param name="ADVERTISER_PREFIXE">prefixe annonceur</param>
//        /// <param name="tableType">type de table ( "advertiser" ou "product")</param>
//        /// <param name="indexSubQuery">index de la sous requête</param>
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

//        #region tri des média pour stratégie média
//        /// <summary>
//        ///tri des  média de la requête en fonction du niveau média pour média stratégie
//        /// </summary>
//        /// <param name="webSession">sessioin du client</param>
//        /// <param name="PREFIXE">Prefixe de la table</param>
//        ///<param name="SortOrder">Ordre de tri</param>
//        ///<param name="withMediaLabel">vrai s'il faut ordonner le libellé du média</param>
//        /// <returns>champs média à afficher</returns>
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
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getMediaStrategyOrderForMediaDetail(WebSession webSession,string PREFIXE,string SortOrder,bool withMediaLabel)-->Impossible d'initialiser les champs média de la requête."));					
//            }			
//        }
//        #endregion

//        #region tri des produits pour stratégie média
//        /// <summary>
//        /// tri des produits pour stratégie média.
//        /// </summary>
//        /// <param name="tableType">Type tableau produit ( "advertiser" ou "product")</param>
//        ///<param name="SubQueryAlias">Alias de la sous requête</param>
//        ///<param name="SortOrder">ordre de tri</param>
//        /// <returns>produit triés</returns>
//        private static string GetProductByOrderForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string SubQueryAlias, string SortOrder){
//            switch(tableType){
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product:
//                    //Cas Références
//                    return " "+SubQueryAlias+".id_product  "+SortOrder;
//                case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser:
//                    //Cas Annonceur
//                    return " "+SubQueryAlias+".id_advertiser  "+SortOrder;					
//                default:
//                    throw (new SectorAnalysisIndicatorDataAccessException ("getProductByOrderForMediaStrategy(FrameWorkConstantes.PalmaresRecap.ElementType tableType,string SubQueryAlias, string SortOrder)-->Impossible de déterminer le type de l'élément de la nomenclature produit."));					
//            }			
//        }
//        #endregion		

//        #endregion								  
//    }
//}
