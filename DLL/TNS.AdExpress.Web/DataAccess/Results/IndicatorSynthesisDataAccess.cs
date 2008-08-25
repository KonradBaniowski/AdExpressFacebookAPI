//#region Information
//// Auteur: Dédé Mussuma
//// Créé le: 11/04/2006
//// Modifiée le: 
//#endregion

//using System;
//using System.Data;
//using System.Collections;
//using System.Windows.Forms;

//using Oracle.DataAccess.Client;

//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using TNS.AdExpress.Web.Exceptions;
//using TNS.FrameWork.DB.Common;
//using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
//using DBConstantes = TNS.AdExpress.Constantes.DB;
//using TNS.AdExpress.Domain.DataBaseDescription;
//using TNS.AdExpress.Web.Core;
//using TNS.AdExpress.Domain.Web;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

//namespace TNS.AdExpress.Web.DataAccess.Results
//{
//    /// <summary>
//    /// Obtient les données de synthèses pour les indicateurs.
//    /// </summary>
//    public class IndicatorSynthesisDataAccess
//    {
//        #region Budget
//        /// <summary>
//        /// Obtient les investissements de l'année N et/ou N-1
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="comparisonCriterion">critère de comparaison</param>
//        /// <returns>Données synthèses indicateurs</returns>
//        public static DataTable GetInvestmentData(WebSession webSession,WebConstantes.CustomerSessions.ComparisonCriterion comparisonCriterion)
//        {
//            #region variables			
//            //Libéllé de la table "recap" cible
//            string recapTableName="";
//            string listSector="";			
//            System.Text.StringBuilder sql = new System.Text.StringBuilder(3000);
//            string StudyMonths="";
//            string recapProductSelection="";
//            string recapMediaSelection="";
//            bool beginByAnd=false;
//            string adExpressUniverseCondition="";
//            string rightAccessVehicleList="";
//            string classificationCustomerProductRight="";
//            bool hasConditions=false;
//            #endregion
			
//            try{
//                //listes de produits, media, annonceurs sélectionnés							
//                DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);			

//                //Determine la table recap à appeler
//                if(WebFunctions.CheckedText.IsStringEmpty(recapUniversSelection.VehicleAccessList))			
//                    recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(recapUniversSelection.VehicleAccessList));			
//                else throw (new SectorAnalysisIndicatorDataAccessException(GestionWeb.GetWebWord(1372,webSession.SiteLanguage)));
			
//                //Obtient la liste des familles correspondant à la sélection client
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,recapUniversSelection.GroupAccessList,recapUniversSelection.SegmentAccessList);				
//                }
				
//                #region periode etudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
//                //Mois concernés
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,webSession.ComparativeStudy);
//                #endregion

//                //sélection produit
//                recapProductSelection=WebFunctions.SQLGenerator.GetRecapProductSelection(comparisonCriterion,recapTableName,"",recapUniversSelection.GroupAccessList,recapUniversSelection.GroupExceptionList,recapUniversSelection.SegmentAccessList,recapUniversSelection.SegmentExceptionList,recapUniversSelection.AdvertiserAccessList,false,false);					
				
//                //sélection média
//                if(recapProductSelection!=null && recapProductSelection.Length>0){
//                    beginByAnd=true;
//                    hasConditions=true;
//                }
//                recapMediaSelection = WebFunctions.SQLGenerator.GetRecapMediaSelection(recapUniversSelection.CategoryAccessList,recapUniversSelection.MediaAccessList,beginByAnd);
				
//                if(recapMediaSelection!=null && recapMediaSelection.Length>0){
//                    beginByAnd=true;
//                    hasConditions=true;
//                }

//                #region droits média
//                // cas du vehicle dans la table pluri
					
//                if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    adExpressUniverseCondition = WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBConstantes.Tables.RECAP_PREFIXE,beginByAnd);					
//                    if(adExpressUniverseCondition!=null && adExpressUniverseCondition.Length>0){
//                        beginByAnd=true;
//                        hasConditions=true;
//                    }
//                }
			
//                // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
//                if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){						
//                    rightAccessVehicleList=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,beginByAnd);						
//                    if(rightAccessVehicleList!=null && rightAccessVehicleList.Length>0){
//                        beginByAnd=true;
//                        hasConditions=true;
//                    }
//                }

				

//                #endregion

//                //Gestion des droits uniquement pour le total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    //Droits Produit
//                   classificationCustomerProductRight = WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,beginByAnd); 										
//                    if(classificationCustomerProductRight!=null && classificationCustomerProductRight.Length>0)hasConditions=true;
//                }

//                #region Construction de la requête
									
//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
					
//                    sql.Append(" select total_N ");//Debut requete principal
//                    if(webSession.ComparativeStudy){
//                        sql.Append(" ,total_N1"); 

//                        //Calcul evolution K€ (année N) divisés par K€ (année N-1).
//                        sql.Append(",decode(total_N1,0,null,ROUND(((total_N/total_N1)*100)-100,0)) as evol");	

//                        //Calcul Ecart : 	K€ (année N) moins K€ (année N-1).
//                        sql.Append(" ,total_N-total_N1 as ecart ");
//                    }
//                    sql.Append(" from ( ");									
					

//                    sql.Append(" select "+StudyMonths+" ");//Debut sous principal
//                    sql.Append(" from "+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE);

//                    #region Clause Where
//                    if(hasConditions){
//                        sql.Append(" Where ");

//                        //sélection produit
//                        sql.Append(recapProductSelection);
					
//                        //sélection média
//                        sql.Append(recapMediaSelection);
					
//                        #region droits média
//                        // cas du vehicle dans la table pluri					
//                        if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                            sql.Append("  "+adExpressUniverseCondition);	
//                        }
			
//                        // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
//                        if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){						
//                            sql.Append("  "+rightAccessVehicleList);	
//                        }

//                        //Droits Parrainage TV
//                        if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                            if (beginByAnd) sql.Append(" and ");
//                            sql.Append("    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ");
//                        }
//                        #endregion

//                        //Gestion des droits uniquement pour le total univers
//                        if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                            //Droits Produit 	
//                            sql.Append("  "+classificationCustomerProductRight);						
//                        }
//                    }
//                    #endregion
				
//                    sql.Append(" ) ");//Fin requete principal
//                }

				
//                #endregion

//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
			
//                return dataSource.Fill(sql.ToString()).Tables[0];
			
//                #endregion
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour le tableau saisonalité: "+sql,err));
//            }
			
//        }
//        #endregion

//        #region Nombre de produits
//        /// <summary>
//        /// Obtient les investissements de l'année N et/ou N-1
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="comparisonCriterion">critère de comparaison</param>
//        /// <param name="idProductLabel">libellé identifiant produit</param>
//        /// <returns>Données synthèses indicateurs</returns>
//        public static DataTable GetProductNumberData(WebSession webSession,WebConstantes.CustomerSessions.ComparisonCriterion comparisonCriterion,string idProductLabel) {
//            #region variables		
//            //Libéllé de la table "recap" cible
//            string recapTableName="";
//            string listSector="";			
//            System.Text.StringBuilder sql = new System.Text.StringBuilder(3000);
//            string StudyMonths="";
//            #endregion
			
//            try{
//                //listes de produits, media, annonceurs sélectionnés							
//                DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);			

//                //Determine la table recap à appeler
//                if(WebFunctions.CheckedText.IsStringEmpty(recapUniversSelection.VehicleAccessList))			
//                    recapTableName=WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult((ClassificationConstantes.DB.Vehicles.names)int.Parse(recapUniversSelection.VehicleAccessList));			
//                else throw (new SectorAnalysisIndicatorDataAccessException(GestionWeb.GetWebWord(1372,webSession.SiteLanguage)));
			
//                //Obtient la liste des familles correspondant à la sélection client
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal){
//                    listSector = WebFunctions.SQLGenerator.GetSectorList(recapTableName,recapUniversSelection.GroupAccessList,recapUniversSelection.SegmentAccessList);				
//                }
				
//                #region periode etudiée
//                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
//                //du dernier mois dispo en BDD
//                //traitement de la notion de fréquence
//                string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//                if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                    throw new NoDataException();				
//                DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//                DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);				
//                //Mois concernés
//                StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,false,false);
//                #endregion

//                #region Construction de la requête					
			
//                if(WebFunctions.CheckedText.IsStringEmpty(recapTableName)){
					
//                    //Requete Année N
//                    GetProductNumberSql(webSession,comparisonCriterion,idProductLabel,sql,recapTableName,recapUniversSelection,StudyMonths,false);

//                    //Requete Année N-1
//                    if(webSession.ComparativeStudy){
//                        sql.Append(" UNION ALL");
//                        StudyMonths=DataAccess.Functions.SumMonthlyExpenditureEuroToString(webSession,true,true);//Mois concernés
//                        GetProductNumberSql(webSession,comparisonCriterion,idProductLabel,sql,recapTableName,recapUniversSelection,StudyMonths,true);
//                    }
//                }

//                #endregion

//                #region Execution de la requête
//                IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 			
//                return dataSource.Fill(sql.ToString()).Tables[0];
			
//                #endregion
//            }
//            catch(System.Exception err){
//                throw(new SectorAnalysisIndicatorDataAccessException ("Impossible de charger les données pour le tableau saisonalité: "+sql,err));
//            }
			
//        }
//        #endregion

//        #region Méthodes internes
		
//        /// <summary>
//        /// Obtient le nombre de produit (ou annonceur) actifs en fonction de la sélection client.
//        /// </summary>
//        /// <param name="webSession">Sesion  du client</param>
//        /// <param name="comparisonCriterion">critère de comparaison</param>
//        /// <param name="idProductLabel">Libellé identifiant produit</param>
//        /// <param name="sql">Chaine de requete</param>
//        /// <param name="recapTableName">Libellé de la table de données</param>
//        /// <param name="recapUniversSelection">Sélection cliente</param>
//        /// <param name="StudyMonths">mois à traiter</param>
//        /// <param name="previousPeriod">Période précédente</param>
//        private static void  GetProductNumberSql(WebSession webSession,WebConstantes.CustomerSessions.ComparisonCriterion comparisonCriterion,string idProductLabel,System.Text.StringBuilder sql,string recapTableName,DataAccess.Functions.RecapUniversSelection recapUniversSelection,string StudyMonths,bool previousPeriod) {
			
//            #region variables
//            string recapProductSelection="";
//            bool beginByAnd=false;			
//            string recapMediaSelection="";			
//            string adExpressUniverseCondition="";
//            string rightAccessVehicleList="";
//            string classificationCustomerProductRight="";
//            bool hasConditions=false;
//            #endregion
			

//            //sélection produit
//            recapProductSelection=WebFunctions.SQLGenerator.GetRecapProductSelection(comparisonCriterion,recapTableName,"",recapUniversSelection.GroupAccessList,recapUniversSelection.GroupExceptionList,recapUniversSelection.SegmentAccessList,recapUniversSelection.SegmentExceptionList,recapUniversSelection.AdvertiserAccessList,false,false);					
				
//            //sélection média
//            if(recapProductSelection!=null && recapProductSelection.Length>0){
//                beginByAnd=true;
//                hasConditions=true;
//            }
//            recapMediaSelection = WebFunctions.SQLGenerator.GetRecapMediaSelection(recapUniversSelection.CategoryAccessList,recapUniversSelection.MediaAccessList,beginByAnd);
				
//            if(recapMediaSelection!=null && recapMediaSelection.Length>0){
//                beginByAnd=true;
//                hasConditions=true;
//            }

//            #region droits média
//            // cas du vehicle dans la table pluri
					
//            if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                adExpressUniverseCondition = WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID, DBConstantes.Tables.RECAP_PREFIXE, beginByAnd);					
//                if(adExpressUniverseCondition!=null && adExpressUniverseCondition.Length>0){
//                    beginByAnd=true;
//                    hasConditions=true;
//                }
//            }
			
//            // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
//            if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){						
//                rightAccessVehicleList=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.RECAP_PREFIXE,beginByAnd);						
//                if(rightAccessVehicleList!=null && rightAccessVehicleList.Length>0){
//                    beginByAnd=true;
//                    hasConditions=true;
//                }
//            }
//            #endregion

//            //Gestion des droits uniquement pour le total univers
//            if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                //Droits Produit
//                classificationCustomerProductRight = WebFunctions.SQLGenerator.getClassificationCustomerProductRight(webSession,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,DBConstantes.Tables.RECAP_PREFIXE,beginByAnd); 										
//                if(classificationCustomerProductRight!=null && classificationCustomerProductRight.Length>0)hasConditions=true;
//            }

//            #region Construction de la requête
					
			
//            sql.Append(" select count("+idProductLabel+") as nbProduct ");//Debut requete principal					
//            sql.Append(" from ( ");									
					

//            sql.Append(" select distinct "+idProductLabel+","+StudyMonths+" ");//Debut sous requete
//            sql.Append(" from "+recapTableName+" "+DBConstantes.Tables.RECAP_PREFIXE);
			
//            #region Clause Where
//            if(hasConditions){
//                sql.Append(" Where ");

//                //sélection produit
//                sql.Append(recapProductSelection);
					
//                //sélection média
//                sql.Append(recapMediaSelection);
					
//                #region droits média
//                // cas du vehicle dans la table pluri					
//                if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())>=0){
//                    sql.Append("  "+adExpressUniverseCondition);	
//                }
			
//                // Vérifie s'il a toujours les droits pour accéder aux données de ce Vehicle
//                if(recapUniversSelection.VehicleAccessList.IndexOf(ClassificationConstantes.DB.Vehicles.names.plurimedia.GetHashCode().ToString())<0){						
//                    sql.Append("  "+rightAccessVehicleList);	
//                }
//                //Droits Parrainage TV
//                if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG)) {
//                    if (beginByAnd) sql.Append(" and ");
//                    sql.Append("    " + DBConstantes.Tables.RECAP_PREFIXE + ".id_category not in (68)  ");
//                }
//                #endregion

//                //Gestion des droits uniquement pour le total univers
//                if(comparisonCriterion == WebConstantes.CustomerSessions.ComparisonCriterion.universTotal){
//                    //Droits Produit 	
//                    sql.Append("  "+classificationCustomerProductRight);						
//                }
//            }
//            #endregion
//            //Fin sous requete


//            sql.Append(" group by "+idProductLabel);
//            if(previousPeriod)
//                sql.Append(" ) where total_N1>0 ");
//            else
//            sql.Append(" ) where total_N>0 ");//Fin requete principal
			

			

//            #endregion

//        }

//        #endregion

//    }
//}
