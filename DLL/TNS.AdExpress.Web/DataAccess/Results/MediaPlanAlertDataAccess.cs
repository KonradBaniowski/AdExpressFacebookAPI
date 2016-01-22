//#region Information
//// Auteur: Guillaume Ragneau
//// Créé le:
//// Modifiée le:
////	22/07/2004	G. Ragneau
////	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
////	25/10/2005	D. V. Mussuma	centralisation des unités
////	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
////	25/11/2005	B.Masson		webSession.Source
//#endregion

//#region Using
//using System;
//using System.Data;
//using Oracle.DataAccess.Client;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Web.Functions;
//using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
//using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
//using AccessCst = TNS.AdExpress.Constantes.Customer.Right.type;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using DBTableFieldsName = TNS.AdExpress.Constantes.DB;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using WebCommon=TNS.AdExpress.Web.Common;
//using TNS.FrameWork.DB.Common;
//using TNS.AdExpress.Constantes.Web;
//using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
//using DbTables=TNS.AdExpress.Constantes.DB.Tables;
//using TNS.AdExpress.Domain.Web.Navigation;
//using TNS.AdExpress.Web.Core.Exceptions;
//#endregion

//namespace TNS.AdExpress.Web.DataAccess.Results{
//    /// <summary>
//    /// Extraction des données d'alerte plan média de la BD
//    /// </summary>
//    public class MediaPlanAlertDataAccess{

//        /// <summary>
//        /// Charge les données pour créer un plan média d'UN vehicle
//        ///		Assignation du nom de la table à attaquer et de l'unité à sélectionner
//        ///		Construction et exécution de la requête
//        /// </summary>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lancée si aucune table ne correspond au vehicle,si l'unité n'est pas valide ou si une erreur Oracle est survenue
//        /// </exception>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du vehicle à traiter</param>
//        /// <param name="beginingDate">Date de début</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <returns>Dataset contenant les données rapatriées de la base de données pour le vehicule considéré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		private static string getTable(DBClassificationConstantes.Vehicles.names idVehicle)
//        ///		private static string getField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit unit)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerProductRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerMediaRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
//        /// </remarks>
//        public static DataSet GetDataWithMediaDetailLevel(WebSession webSession,string idVehicle, string beginingDate, string endDate){

//            #region Variables
//            string list="";
//            string tableName="";
//            string unitField="";
//            bool premier = true;
//            #endregion

//            #region Récupération des noms de tables et de champs suivant le vehicule
//            try{
//                //tableName = getTable((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)));
//                TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
//                tableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)),currentModuleDescription.ModuleType);
//                if(DBClassificationConstantes.Vehicles.names.outdoor ==(DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)) 
//                    && webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion){
//                    unitField = WebFunctions.SQLGenerator.getUnitField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.numberBoard);
//                }else
//                    unitField = WebFunctions.SQLGenerator.getUnitField(webSession.Unit);
//            }
//            catch(Exception exc){
//                throw new MediaPlanAlertDataAccessException("MediaPlanAlertDataAccess.getData(WebSession webSession,string idVehicle, string beginingDate, string endDate) : \n\t" + exc.Message);
//            }
//            #endregion

//            #region Construction de la requête
//            string sql="";
//            // Sélection de la nomenclature Support
//            sql+="select wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity";
//            // Sélection de la date
//            sql+=", date_media_num as date_num";
//            // Sélection de l'unité
//            //			if(webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages) sql+=", (sum("+unitField+"))/1000 as unit";
//            //			else sql+=", sum("+unitField+") as unit";
//            sql+=", sum("+unitField+") as unit";
//            // Tables
//            sql+=" from "+DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"vehicle vh, "
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"category ct,"
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"media md,"
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ";
//            // Conditions de jointure
//            sql+=" Where vh.id_vehicle=wp.id_vehicle ";
//            sql+=" and ct.id_category=wp.id_category ";
//            sql+=" and md.id_media=wp.id_media ";
//            sql+=" and vh.id_language="+webSession.SiteLanguage.ToString();
//            sql+=" and ct.id_language="+webSession.SiteLanguage.ToString();
//            sql+=" and md.id_language="+webSession.SiteLanguage.ToString();
//            // activation
//            sql+=" and vh.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//            sql+=" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//            sql+=" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;


//            // Période
//            sql+=" and date_media_num >="+beginingDate;
//            sql+=" and date_media_num <="+endDate;
//            // Gestion des sélections et des droits

//            #region Nomenclature Produit (droits)
//            premier=true;
//            //Droits en accès
//            sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
//            // Produit à exclure en radio
//            sql+=SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false);
//            #endregion

//            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

//            #region Sélection
//            // Sélection en accès
//            premier=true;
//            // HoldingCompany
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
//            if(list.Length>0){
//                sql+=" and ((wp.id_holding_company in ("+list+") ";
//                premier=false;
//            }
//            // Advertiser
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_advertiser in ("+list+") ";
//                premier=false;
//            }
//            // Marque
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
//            if(list.Length>0) {
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_brand in ("+list+") ";
//                premier=false;
//            }
//            // Product
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_product in ("+list+") ";
//                premier=false;
//            }
//            // Sector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
//            if(list.Length>0){
//                sql+=" and ((wp.id_sector in ("+list+") ";
//                premier=false;
//            }
//            // SubSector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_subsector in ("+list+") ";
//                premier=false;
//            }
//            // Group
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_group_ in ("+list+") ";
//                premier=false;
//            }

//            if(!premier) sql+=" )";
			
//            // Sélection en Exception
//            // HoldingCompany
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_holding_company not in ("+list+") ";
//                premier=false;
//            }
//            // Advertiser
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_advertiser not in ("+list+") ";
//                premier=false;
//            }
//            // Marque
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
//            if(list.Length>0) {
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_brand not in ("+list+") ";
//                premier=false;
//            }
//            // Product
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_product not in ("+list+") ";
//                premier=false;
//            }
//            // Sector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_sector not in ("+list+") ";
//                premier=false;
//            }
//            // SubSector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_subsector not in ("+list+") ";
//                premier=false;
//            }
//            // Group
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_group_ not in ("+list+") ";
//                premier=false;
//            }
//            if(!premier) sql+=" )";
//            #endregion

//            #endregion

//            #region Nomenclature Media (droits et sélection)

//            #region Droits
//            sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
//            #endregion

//            #region Sélection
//            //list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
//            //if(list.Length>0) sql+=" and ((wp.id_vehicle in ("+list+"))) ";
//            sql+=" and ((wp.id_vehicle= "+idVehicle+")) ";
//            #endregion

//            #endregion

//            #region Nomenclature Produit (Niveau de détail)  
//            // Niveau de produit
//            sql+=SQLGenerator.getLevelProduct(webSession,"wp",true);
//            #endregion

//            // Ordre
//            sql+="Group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity ";
//            // et la date
//            sql+=", date_media_num ";

//            // Ordre
//            sql+="Order by wp.id_vehicle, vehicle, wp.id_category, category, media, wp.id_media ";
//            // et la date
//            sql+=", date_media_num ";

//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new MediaPlanAlertDataAccessException ("Impossible de charger pour l'alerte plan media: "+sql,err));
//            }
//            #endregion

//        }

//        #region Méthode internes nouvelle formule
//        /*** Nouvelle gestion des détails support ***/

		

//        /// <summary>
//        /// Obtient les champs correspondants au détail media demandée par le client.
//        /// Les champs demandées corespondent à l'identifiant et le libellé du niveau support
//        /// </summary>
//        /// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
//        /// <returns>Chaîne contenant les champs</returns>
//        public static string GetMediaFields(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                    return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                    return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
//                default:
//                    throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
//            }
		
//        }

//        /// <summary>
//        /// Obtient les champs correspondants au détail media demandée par le client.
//        /// Les champs demandées corespondent à l'identifiant et le libellé du niveau support
//        /// </summary>
//        /// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
//        /// <returns>Chaîne contenant les champs</returns>
//        public static string GetOrderMediaFields(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                    return(" vehicle, category ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    return(" vehicle, category,media ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                    return(" vehicle,media ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                    return(" vehicle, interest_center ");
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    return(" vehicle,interest_center, media ");
//                default:
//                    throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
//            }
		
//        }

//        /// <summary>
//        /// Obtient les nom des tables à utiliser lors d'un détail media
//        /// </summary>
//        /// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
//        /// <returns>Chaîne contenant les tables</returns>
//        public static string GetMediaTable(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
//            string tmp="";
//            //Vehicles
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE+", ";
//                    break;
//            }
			
//            //Categories
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.CATEGORY+" "+DbTables.CATEGORY_PREFIXE+", ";
//                    break;
//            }

//            // Media
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.MEDIA+" "+DbTables.MEDIA_PREFIXE+", ";
//                    break;
//            }

//            // Interest center
//            switch(preformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.INTEREST_CENTER+" "+DbTables.INTEREST_CENTER_PREFIXE+", ";
//                    break;
//            }
//            if(tmp.Length==0)throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
//            tmp=tmp.Substring(0,tmp.Length-2);
//            return(tmp);
//        }

//        /// <summary>
//        /// Obtient les nom des tables à utiliser lors d'un détail media
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="dataTablePrefixe">Prefixe de la table de résultat</param>
//        /// <param name="beginByAnd">La condition doit commencée par And</param>
//        /// <returns>Chaîne contenant les tables</returns>
//        public static string GetMediaJoinConditions(WebSession webSession,string dataTablePrefixe,bool beginByAnd){
//            string tmp="";
//            //Vehicles
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage;
//                    tmp+=" and "+DbTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                    tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle ";
//                    break;
//            }
			
//            //Categories
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                    tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage;
//                    tmp+=" and "+DbTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                    tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_category="+dataTablePrefixe+".id_category ";
//                    break;
//            }

//            // Media
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage;
//                    tmp+=" and "+DbTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                    tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_media="+dataTablePrefixe+".id_media ";
//                    break;
//            }

//            // Interest center
//            switch(webSession.PreformatedMediaDetail){
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
//                case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
//                    tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage;
//                    tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                    tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center="+dataTablePrefixe+".id_interest_center ";
//                    break;
//            }
//            if(tmp.Length==0)throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
//            if(!beginByAnd)tmp=tmp.Substring(4,tmp.Length-4);
//            return(tmp);
//        }
//        #endregion




//        #region Acienne Méthode
		
//        /// <summary>
//        /// Charge les données pour créer un plan média d'UN vehicle
//        ///		Assignation du nom de la table à attaquer et de l'unité à sélectionner
//        ///		Construction et exécution de la requête
//        /// </summary>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lancée si aucune table ne correspond au vehicle,si l'unité n'est pas valide ou si une erreur Oracle est survenue
//        /// </exception>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du vehicle à traiter</param>
//        /// <param name="beginingDate">Date de début</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <returns>Dataset contenant les données rapatriées de la base de données pour le vehicule considéré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		private static string getTable(DBClassificationConstantes.Vehicles.names idVehicle)
//        ///		private static string getField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit unit)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerProductRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerMediaRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
//        /// </remarks>
//        public static DataSet GetData(WebSession webSession,string idVehicle, string beginingDate, string endDate){

//            #region Variables
//            string list="";
//            string tableName="";
//            string unitField="";
//            bool premier = true;
//            #endregion

//            #region Récupération des noms de tables et de champs suivant le vehicule
//            try{
//                //tableName = getTable((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)));
//                TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
//                tableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)),currentModuleDescription.ModuleType);
//                if(DBClassificationConstantes.Vehicles.names.outdoor ==(DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)) 
//                    && webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion){
//                    unitField = WebFunctions.SQLGenerator.getUnitField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.numberBoard);
//                }else
//                unitField = WebFunctions.SQLGenerator.getUnitField(webSession.Unit);
//            }
//            catch(Exception exc){
//                throw new MediaPlanAlertDataAccessException("MediaPlanAlertDataAccess.getData(WebSession webSession,string idVehicle, string beginingDate, string endDate) : \n\t" + exc.Message);
//            }
//            #endregion

//            #region Construction de la requête
//            string sql="";
//            // Sélection de la nomenclature Support
//            sql+="select wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity";
//            // Sélection de la date
//            sql+=", date_media_num as date_num";
//            // Sélection de l'unité
////			if(webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages) sql+=", (sum("+unitField+"))/1000 as unit";
////			else sql+=", sum("+unitField+") as unit";
//            sql+=", sum("+unitField+") as unit";
//            // Tables
//            sql+=" from "+DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"vehicle vh, "
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"category ct,"
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"media md,"
//                +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ";
//            // Conditions de jointure
//            sql+=" Where vh.id_vehicle=wp.id_vehicle ";
//            sql+=" and ct.id_category=wp.id_category ";
//            sql+=" and md.id_media=wp.id_media ";
//            sql+=" and vh.id_language="+webSession.SiteLanguage.ToString();
//            sql+=" and ct.id_language="+webSession.SiteLanguage.ToString();
//            sql+=" and md.id_language="+webSession.SiteLanguage.ToString();
//            // activation
//            sql+=" and vh.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//            sql+=" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//            sql+=" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;


//            // Période
//            sql+=" and date_media_num >="+beginingDate;
//            sql+=" and date_media_num <="+endDate;
//            // Gestion des sélections et des droits

//            #region Nomenclature Produit (droits)
//            premier=true;
//            //Droits en accès
//            sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
//            // Produit à exclure en radio
//            sql+=SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false);
//            #endregion

//            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

//            #region Sélection
//            // Sélection en accès
//            premier=true;
//            // HoldingCompany
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
//            if(list.Length>0){
//                sql+=" and ((wp.id_holding_company in ("+list+") ";
//                premier=false;
//            }
//            // Advertiser
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_advertiser in ("+list+") ";
//                premier=false;
//            }
//            // Marque
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
//            if(list.Length>0)
//            {
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_brand in ("+list+") ";
//                premier=false;
//            }
//            // Product
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_product in ("+list+") ";
//                premier=false;
//            }
//            // Sector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
//            if(list.Length>0){
//                sql+=" and ((wp.id_sector in ("+list+") ";
//                premier=false;
//            }
//            // SubSector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_subsector in ("+list+") ";
//                premier=false;
//            }
//            // Group
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
//            if(list.Length>0){
//                if(!premier) sql+=" or";
//                else sql+=" and ((";
//                sql+=" wp.id_group_ in ("+list+") ";
//                premier=false;
//            }

//            if(!premier) sql+=" )";
			
//            // Sélection en Exception
//            // HoldingCompany
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_holding_company not in ("+list+") ";
//                premier=false;
//            }
//            // Advertiser
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_advertiser not in ("+list+") ";
//                premier=false;
//            }
//            // Marque
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
//            if(list.Length>0)
//            {
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_brand not in ("+list+") ";
//                premier=false;
//            }
//            // Product
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_product not in ("+list+") ";
//                premier=false;
//            }
//            // Sector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_sector not in ("+list+") ";
//                premier=false;
//            }
//            // SubSector
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_subsector not in ("+list+") ";
//                premier=false;
//            }
//            // Group
//            list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
//            if(list.Length>0){
//                if(premier) sql+=" and (";
//                else sql+=" and";
//                sql+=" wp.id_group_ not in ("+list+") ";
//                premier=false;
//            }
//            if(!premier) sql+=" )";
//            #endregion

//            #endregion

//            #region Nomenclature Media (droits et sélection)

//            #region Droits
//            sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
//            #endregion

//            #region Sélection
//            //list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
//            //if(list.Length>0) sql+=" and ((wp.id_vehicle in ("+list+"))) ";
//            sql+=" and ((wp.id_vehicle= "+idVehicle+")) ";
//            #endregion

//            #endregion

//            #region Nomenclature Produit (Niveau de détail)  
//            // Niveau de produit
//            sql+=SQLGenerator.getLevelProduct(webSession,"wp",true);
//            #endregion

//            // Ordre
//            sql+="Group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity ";
//            // et la date
//            sql+=", date_media_num ";

//            // Ordre
//            sql+="Order by wp.id_vehicle, vehicle, wp.id_category, category, media, wp.id_media ";
//            // et la date
//            sql+=", date_media_num ";

//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new MediaPlanAlertDataAccessException ("Impossible de charger pour l'alerte plan media: "+sql,err));
//            }
//            #endregion

//        }

//        /// <summary>
//        /// Charge les données pour créer un plan média pour chaque vehicle sélectionné dans la session
//        /// sur la période spécifiée
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="beginningDate">date de début de période au formazt YYYYMMDD</param>
//        /// <param name="endDate">date de fin de période au formazt YYYYMMDD</param>
//        /// <returns>Dataset contenant les données rapatriées de la base de données pour les vehicule sélectionné dans la session</returns>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lancée en cas d'erreur sur un vehicle
//        /// </exception>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
//        /// </remarks>
//        public static DataSet GetPluriMediaDataset(WebSession webSession, string beginningDate, string endDate){
//            string[] listVehicles = webSession.GetSelection(webSession.SelectionUniversMedia, AccessCst.vehicleAccess).Split(new char[]{','});
//            DataSet ds = new DataSet();
//            for(int i=0; i< listVehicles.Length; i++){
//                try{
//                    ds.Merge(GetData(webSession,listVehicles[i],beginningDate,endDate));
//                }
//                catch(System.Exception err){
//                    throw new MediaPlanAlertDataAccessException("MedaiPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate",err);
//                }
//            }
//            return ds;
//        }


//        /// <summary>
//        /// Obtient les données pour créer un plan média plurimédia à partir  des données 
//        /// de la session d'un client
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Données chargées</returns>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lancée en cas d'erreur
//        /// </exception>
//        public static DataSet GetPluriMediaDataset(WebSession webSession){
//            try{
//                return GetPluriMediaDataset(webSession, webSession.PeriodBeginningDate, webSession.PeriodEndDate);
//            }
//            catch(System.Exception err){
//                throw new MediaPlanAlertDataAccessException("MedaiPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)",err);
//            }
//        }


//        /// <summary>
//        /// Obtient la table à utiliser en fonction du vehicle
//        /// </summary>
//        /// <param name="idVehicle">Identifiant du vehicle</param>
//        /// <returns>NOm de la table</returns>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lancée au cas ou le vehicle considéé n'est pas un cas traité
//        /// </exception>
//        private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle){
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.press:
//                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS;
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS_INTER;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_RADIO;
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_TV;
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                        return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_OUTDOOR;
//                default:
//                    throw new Exceptions.MediaPlanAlertDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }
//        #endregion
//    }
//}
