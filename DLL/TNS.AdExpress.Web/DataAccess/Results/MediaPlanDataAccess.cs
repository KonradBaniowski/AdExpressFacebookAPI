#region Informations
// Auteur: G. Facon
// Date de création: 07/12/2005
// Date de modification: 
#endregion

#region Using
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using TNS.FrameWork.DB.Common;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBTableFieldsName = TNS.AdExpress.Constantes.DB;
using WebCommon=TNS.AdExpress.Web.Common;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Accès données plan media
	/// </summary>
	public class MediaPlanDataAccess{

		/// <summary>
		/// Obtient les données d'un calendrier d'action pour une analyse plan média
		/// </summary>
		/// <remarks>A utiliser pour les analyses</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Données</returns>
		public static DataSet GetDataWithMediaDetailLevel(WebSession webSession){
			return(GetDataWithMediaDetailLevel(webSession,-1,webSession.PeriodBeginningDate,webSession.PeriodEndDate));
			
		}

		/// <summary>
		/// Obtient les données d'un calendrier d'action pour une analyse plan média
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du vehicle si cela est nécessaire</param>
		/// <param name="beginningDate">Date de début</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Données</returns>
		private static DataSet GetDataWithMediaDetailLevel(WebSession webSession,Int64 vehicleId, string beginningDate,string endDate){

			#region Variables
			string list="";
			//bool premier=true;
			string tableName=null;
			string mediaTableName=null;
			string dateFieldName=null;
			string mediaFieldName=null;
			string orderFieldName=null;
			string unitFieldName=null;
			string mediaJoinCondition=null;
			#endregion
			
			#region Construction de la requête
			try{

				
				tableName=GetMediaTableName(webSession,vehicleId);
				mediaTableName=GetMediaTable(webSession.PreformatedMediaDetail);
				dateFieldName=GetDateFieldName(webSession);
				unitFieldName=GetUnitFieldName(webSession,vehicleId);
				mediaFieldName=GetMediaFields(webSession.PreformatedMediaDetail);
				orderFieldName=GetOrderMediaFields(webSession.PreformatedMediaDetail);
				mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPlanDataAccessException("Impossible de construire la requête",err));	
			}

			string sql="";
			// Sélection de la nomenclature Support
			sql+="select "+mediaFieldName+" ";
			// Sélection de la date
			sql+=", "+dateFieldName+" as date_num,";
			//sql+=",max(wp.id_periodicity) as id_periodicity ";
			sql+=" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".RECUP_ID_PERIOD(max("+DbTables.PERIODICITY_PREFIXE+".value_second)) as id_periodicity,";
			// Sélection de l'unité
			sql+="sum("+unitFieldName+") as unit";
			// Tables
			sql+=" from "+mediaTableName+", "+tableName+" wp,"+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.PERIODICITY+" "+DbTables.PERIODICITY_PREFIXE+" ";
			//Conditions media
			sql+="where "+mediaJoinCondition+"";
			// Jointure Périodicité
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_language=33";
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_periodicity=wp.id_periodicity";
			// Période
			sql+=" and "+dateFieldName+">="+beginningDate;
			sql+=" and "+dateFieldName+"<="+endDate;
			
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)
			//Droits en accès
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			// Produit à exclure en radio
			sql+=SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DbTables.WEB_PLAN_PREFIXE,true,false);
			#endregion

			#region Nomenclature Produit (Niveau de détail)  
			// Niveau de produit
			sql+=SQLGenerator.getLevelProduct(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			#endregion

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 
		
			#region Sélection
			sql+=GetProductSelection(webSession,DbTables.WEB_PLAN_PREFIXE);			
			#endregion

			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			#endregion

			#region Sélection
			if(webSession.DetailPeriod!=WebConstantes.CustomerSessions.Period.DisplayLevel.dayly){
				list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
				if(list.Length>0) sql+=" and (("+DbTables.WEB_PLAN_PREFIXE+".id_vehicle in ("+list+"))) ";
			}
			else
				sql+=" and ((wp.id_vehicle= "+vehicleId+")) ";
			#endregion

			#endregion
			
			// Ordre
			sql+="Group by "+mediaFieldName.Replace("nvl(id_slogan,0) as slogan","id_slogan").Replace("nvl(id_slogan,0) as id_slogan","id_slogan");
			// et la date
			sql+=", "+dateFieldName+" ";

			// Ordre
			sql+="Order by "+orderFieldName+" ";
			// et la date
			sql+=", "+dateFieldName+" ";

			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPlanDataAccessException ("Impossible de charger le plan media: "+sql,err));
			}
			#endregion			

		}

		/// <summary>
		/// Charge les données pour créer un plan média pour chaque vehicle sélectionné dans la session
		/// sur la période spécifiée
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningDate">date de début de période au format YYYYMMDD</param>
		/// <param name="endDate">date de fin de période au formazt YYYYMMDD</param>
		/// <returns>Dataset contenant les données rapatriées de la base de données pour les vehicule sélectionné dans la session</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lancée en cas d'erreur sur un vehicle
		/// </exception>
		/// <remarks>
		/// Utilise les méthodes:
		///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
		/// </remarks>
		public static DataSet GetPluriMediaDataSetWithMediaDetailLevel(WebSession webSession, string beginningDate, string endDate){
			string[] listVehicles = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess).Split(new char[]{','});
			DataSet ds = new DataSet();
			for(int i=0; i< listVehicles.Length; i++){
				try{
					ds.Merge(GetDataWithMediaDetailLevel(webSession,Int64.Parse(listVehicles[i]),beginningDate,endDate));
				}
				catch(System.Exception err){
					throw new WebExceptions.MediaPlanDataAccessException("MediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate",err);
				}
			}
			DataSet ds2=new DataSet();
			//ds2.Tables.Add();
			ds2.Merge(ds.Tables[0].Select("",GetFieldOrderForDataView(webSession.PreformatedMediaDetail)));
			
			return ds2;
		}


		/// <summary>
		/// Obtient les données pour créer un plan média plurimédia à partir  des données 
		/// de la session d'un client
		/// </summary>
		/// <remarks>A utiliser pour les alertes</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Données chargées</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lancée en cas d'erreur
		/// </exception>
		public static DataSet GetPluriMediaDataSetWithMediaDetailLevel(WebSession webSession){
			try{
				return GetPluriMediaDataSetWithMediaDetailLevel(webSession, webSession.PeriodBeginningDate, webSession.PeriodEndDate);
			}
			catch(System.Exception err){
				throw new WebExceptions.MediaPlanDataAccessException("MedaiPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)",err);
			}
		}


		#region Méthode internes nouvelle formule
		/*** Nouvelle gestion des détails support ***/

		#region Donne la sélection produit
		/// <summary>
		/// Donne la sélection produit
		/// </summary>
		/// <remarks>
		/// Commence obligatoirement par AND
		/// </remarks>
		/// <param name="webSession">Session client</param>
		/// <param name="dataTablePrefixe">Préfixe de la table des données</param>
		/// <returns>Sélection produit pour être ajouter à une requête SQL</returns>
		private static string GetProductSelection(WebSession webSession,string dataTablePrefixe){
			// Sélection en accès
			bool premier=true;
			string list="",sql="";
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			if(list.Length>0){
				sql+=" and (("+dataTablePrefixe+".id_holding_company in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" "+dataTablePrefixe+".id_advertiser in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
			if(list.Length>0) {
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" "+dataTablePrefixe+".id_brand in ("+list+") ";
				premier=false;
			}

			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" "+dataTablePrefixe+".id_product in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
			if(list.Length>0){
				sql+=" and (("+dataTablePrefixe+".id_sector in ("+list+") ";
				premier=false;
			}
			// subsector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" "+dataTablePrefixe+".id_subsector in ("+list+") ";
				premier=false;
			}
			// Group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" "+dataTablePrefixe+".id_group_ in ("+list+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			
			// Sélection en Exception
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_holding_company not in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_advertiser not in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
			if(list.Length>0) {
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_brand not in ("+list+") ";
				premier=false;
			}
			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_product not in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_sector not in ("+list+") ";
				premier=false;
			}
			// SubSector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_subsector not in ("+list+") ";
				premier=false;
			}
			// group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" "+dataTablePrefixe+".id_group_ not in ("+list+") ";
				premier=false;
			}

			if(!premier) sql+=" )";
			return(sql);
		}
		#endregion

		#region Donne les champs à utiliser pour l'unité dans la requête
		/// <summary>
		/// Indique le champ à utiliser pour l'unité dans la requête
		/// </summary>
		/// <param name="webSession">Session du client</param>
        /// <param name="vehicleId">Vehicle id</param>
		/// <returns>Le champ correspondant au type d'unité</returns>
        private static string GetUnitFieldName(WebSession webSession, Int64 vehicleId) {
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:

                    try {
                        return UnitsInformation.List[webSession.Unit].DatabaseMultimediaField;
                    }
                    catch{
                        throw (new SQLGeneratorException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:

                    try {
                        WebConstantes.CustomerSessions.Unit unit = VehiclesInformation.Get(vehicleId).GetUnitFromBaseId(webSession.Unit);
                        return UnitsInformation.List[unit].DatabaseField;
                    }
                    catch{
                        throw (new SQLGeneratorException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

				default:
                    throw (new SQLGeneratorException("Selected period detail is uncorrect. Unable to determine unit field."));

			}
		}
		#endregion

		/// <summary>
		/// Indique le champ à utilisée pour la date dans la requête
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Le champ correspondant au type de période</returns>
		private static string GetDateFieldName(WebSession webSession){
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					return(Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					return("date_media_num");
				default:
					throw(new WebExceptions.MediaPlanDataAccessException("Le détails période sélectionné est incorrect pour le choix du champ"));
			}
		}

		/// <summary>
		/// Indique la table à utilisée pour la requête
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du media</param>
		/// <returns>La table correspondant au type de période</returns>
		private static string GetMediaTableName(WebSession webSession,Int64 vehicleId){
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					return(DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+Tables.WEB_PLAN_MEDIA_MONTH);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+Tables.WEB_PLAN_MEDIA_WEEK);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
					switch(currentModuleDescription.ModuleType){
						case WebConstantes.Module.Type.alert:
						switch((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
							case DBClassificationConstantes.Vehicles.names.press:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_PRESS;
							case DBClassificationConstantes.Vehicles.names.internationalPress:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_PRESS_INTER;
							case DBClassificationConstantes.Vehicles.names.radio:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_RADIO;
							case DBClassificationConstantes.Vehicles.names.tv:
							case DBClassificationConstantes.Vehicles.names.others:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_TV;
							case DBClassificationConstantes.Vehicles.names.outdoor:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_OUTDOOR;
							case DBClassificationConstantes.Vehicles.names.adnettrack:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_ADNETTRACK;
							default:
								throw(new SQLGeneratorException("Impossible de déterminer la table media à utiliser"));
						}
						case WebConstantes.Module.Type.analysis:
						switch((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
							case DBClassificationConstantes.Vehicles.names.press:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS;
							case DBClassificationConstantes.Vehicles.names.internationalPress:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS_INTER;
							case DBClassificationConstantes.Vehicles.names.radio:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_RADIO;
							case DBClassificationConstantes.Vehicles.names.tv:
							case DBClassificationConstantes.Vehicles.names.others:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_TV;
							case DBClassificationConstantes.Vehicles.names.outdoor:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_OUTDOOR;
							case DBClassificationConstantes.Vehicles.names.adnettrack:
								return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_ADNETTRACK;
							default:
								throw(new SQLGeneratorException("Impossible de déterminer la table media à utiliser"));
						}
						default:
							throw(new SQLGeneratorException("Impossible de déterminer le type du module pour déterminer la table à utiliser"));
					}
				default:
					throw(new SQLGeneratorException("Le détails période sélectionné est incorrect pour le choix de la table"));
			}
		}

		/// <summary>
		/// Obtient les champs correspondants au détail media demandée par le client.
		/// Les champs demandées corespondent à l'identifiant et le libellé du niveau support
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <returns>Chaîne contenant les champs</returns>
		public static string GetMediaFields(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller,"+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller, media_seller,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				// SLOGAN
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.CATEGORY_PREFIXE+".id_category, category,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,interest_center,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
					return(" "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center,"+DbTables.MEDIA_PREFIXE+".id_media, media, nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.MEDIA_PREFIXE+".id_media, media");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle,"+DbTables.CATEGORY_PREFIXE+".id_category,category,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,interest_center,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					return(" nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media_seller,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center,"+DbTables.MEDIA_PREFIXE+".id_media, media ");
				default:
					throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			}
		
		}

		/// <summary>
		/// Obtient les champs correspondants au détail media demandée par le client.
		/// Les champs demandées corespondent à l'identifiant et le libellé du niveau support
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <returns>Chaîne contenant les champs</returns>
		public static string GetOrderMediaFields(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,category,"+DbTables.CATEGORY_PREFIXE+".id_category ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,category,"+DbTables.CATEGORY_PREFIXE+".id_category,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media  ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
					// SLOGAN
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,category,"+DbTables.CATEGORY_PREFIXE+".id_category,media,"+DbTables.MEDIA_PREFIXE+".id_media,id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media, id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media,id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
					return(" vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media,id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media,id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media,id_slogan,id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
					return(" media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media, id_slogan ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
					return(" id_slogan,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
					return(" id_slogan,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
					return(" id_slogan,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,category,"+DbTables.CATEGORY_PREFIXE+".id_category,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
					return(" id_slogan,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
					return(" id_slogan,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
					return(" id_slogan,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
					return(" id_slogan,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,vehicle,"+DbTables.VEHICLE_PREFIXE+".id_vehicle,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					return(" id_slogan,media_seller,"+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller,interest_center,"+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center,media,"+DbTables.MEDIA_PREFIXE+".id_media ");
				default:
					throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			}
		
		}

		/// <summary>
		/// Obtient les nom des tables à utiliser lors d'un détail media
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <returns>Chaîne contenant les tables</returns>
		public static string GetMediaTable(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			string tmp="";
			//Vehicles
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE+", ";
					break;
			}
			
			//Categories
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.CATEGORY+" "+DbTables.CATEGORY_PREFIXE+", ";
					break;
			}

			// Media
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.MEDIA+" "+DbTables.MEDIA_PREFIXE+", ";
					break;
			}

			// Interest center
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.INTEREST_CENTER+" "+DbTables.INTEREST_CENTER_PREFIXE+", ";
					break;
			}

			// Régie
			switch(preformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.MEDIA_SELLER+" "+DbTables.MEDIA_SELLER_PREFIXE+", ";
					break;
			}



			if(tmp.Length==0)throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			tmp=tmp.Substring(0,tmp.Length-2);
			return(tmp);
		}

		/// <summary>
		/// Obtient les nom des tables à utiliser lors d'un détail media
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="dataTablePrefixe">Prefixe de la table de résultat</param>
		/// <param name="beginByAnd">La condition doit commencée par And</param>
		/// <returns>Chaîne contenant les tables</returns>
		public static string GetMediaJoinConditions(WebSession webSession,string dataTablePrefixe,bool beginByAnd){
			string tmp="";
			//Vehicles
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle ";
					break;
			}
			
			//Categories
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_category="+dataTablePrefixe+".id_category ";
					break;
			}

			// Media
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_media="+dataTablePrefixe+".id_media ";
					break;
			}

			// Interest center
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center="+dataTablePrefixe+".id_interest_center ";
					break;
			}

			// Régie
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
					tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller="+dataTablePrefixe+".id_media_seller ";
					break;
			}


			if(tmp.Length==0)throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			if(!beginByAnd)tmp=tmp.Substring(4,tmp.Length-4);
			return(tmp);
		}

		private static string GetFieldOrderForDataView(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			string sql=GetOrderMediaFields(preformatedMediaDetail).ToUpper();
			string tmp="";
			const string POINT_SEARCH=".ID_";
			string[] fields=sql.Split(',');
			foreach(string currentfield in fields){
				tmp+=currentfield.Substring(currentfield.IndexOf(POINT_SEARCH)+1,currentfield.Length-(currentfield.IndexOf(POINT_SEARCH)+1))+",";
			}
			if(tmp.Length>0)tmp=tmp.Substring(0,tmp.Length-1);
			return(tmp);
		}
		#endregion

	}
}
