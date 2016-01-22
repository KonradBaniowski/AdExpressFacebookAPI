#region Informations
// Auteur: G. Facon
// Date de création: 01/06/2004
// Date de modification: 
//	28/06/2004	G. Facon
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using WebConbstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Units;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe d'accès à la base de données, pour les plans media sur historique long
	/// </summary>
	public class MediaPlanAnalysisDataAccess{
    
		/// <summary>
		/// Obtient les données d'un calendrier d'action pour une analyse plan média
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Données</returns>
		public static DataSet GetDataWithMediaDetailLevel(WebSession webSession){

			#region Variables
			string list="";
			bool premier=true;
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
				tableName=GetTableName(webSession.DetailPeriod);
				mediaTableName=GetMediaTable(webSession.PreformatedMediaDetail);
				dateFieldName=GetDateFieldName(webSession.DetailPeriod);
                unitFieldName = SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, TableType.Type.webPlan);
				mediaFieldName=GetMediaFields(webSession.PreformatedMediaDetail);
				orderFieldName=GetOrderMediaFields(webSession.PreformatedMediaDetail);
				mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
			}
			catch(System.Exception err){
				throw(new MediaPlanAnalysisDataAccessException("Impossible de construire la requête",err));
				
			}

			string sql="";
			// Sélection de la nomenclature Support
			sql+="select "+mediaFieldName+" ";
			// Sélection de la date
			sql+=", "+dateFieldName+" as date_num,";
			//sql+=",max(wp.id_periodicity) as id_periodicity ";
			sql+=" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".RECUP_ID_PERIOD(max("+DbTables.PERIODICITY_PREFIXE+".value_second)) as id_periodicity,";
			// Sélection de l'unité
			sql+=unitFieldName;
			// Tables
			sql+=" from "+mediaTableName+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp,"+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.PERIODICITY+" "+DbTables.PERIODICITY_PREFIXE+" ";
			//Conditions media
			sql+="where "+mediaJoinCondition+"";
			// Jointure Périodicité
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_language=33";
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_periodicity=wp.id_periodicity";
			// Période
			sql+=" and "+dateFieldName+">="+webSession.PeriodBeginningDate;
			sql+=" and "+dateFieldName+"<="+webSession.PeriodEndDate;
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en accès
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
			// Produit à exclure en radio
			sql+=SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false);
			#endregion

			#region Nomenclature Produit (Niveau de détail)  
			// Niveau de produit
			sql+=SQLGenerator.getLevelProduct(webSession,"wp",true);
			#endregion

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

			#region Sélection
			// Sélection en accès
			premier=true;
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			if(list.Length>0){
				sql+=" and ((wp.id_holding_company in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_advertiser in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
			if(list.Length>0) {
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_brand in ("+list+") ";
				premier=false;
			}

			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_product in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
			if(list.Length>0){
				sql+=" and ((wp.id_sector in ("+list+") ";
				premier=false;
			}
			// subsector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_subsector in ("+list+") ";
				premier=false;
			}
			// Group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_group_ in ("+list+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			
			// Sélection en Exception
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_holding_company not in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_advertiser not in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
			if(list.Length>0) {
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_brand not in ("+list+") ";
				premier=false;
			}
			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_product not in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_sector not in ("+list+") ";
				premier=false;
			}
			// SubSector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_subsector not in ("+list+") ";
				premier=false;
			}
			// group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_group_ not in ("+list+") ";
				premier=false;
			}

			if(!premier) sql+=" )";
			#endregion

			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
			#endregion

			#region Sélection
			list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
			if(list.Length>0) sql+=" and ((wp.id_vehicle in ("+list+"))) ";
			#endregion

			#endregion

			
	
			// Ordre
			sql+="Group by "+mediaFieldName;
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
				throw(new MediaPlanAnalysisDataAccessException ("Impossible de charger le plan media: "+sql,err));
			}
			#endregion			

		}

		/// <summary>
		/// Obtient les données d'un calendrier d'action pour une analyse plan média
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Données</returns>
		public static DataSet GetData(WebSession webSession){

			#region Variables
			string list="";
			bool premier=true;
			string tableName=null;
			string dateFieldName=null;
			string unitFieldName=null;
			#endregion
			
			#region Construction de la requête
			try{
				tableName=GetTableName(webSession.DetailPeriod);
				dateFieldName=GetDateFieldName(webSession.DetailPeriod);
                unitFieldName = SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, TableType.Type.webPlan);
			}
			catch(System.Exception err){
				throw(new MediaPlanAnalysisDataAccessException("Impossible de construire la requête",err));
				
			}

			string sql="";
			// Sélection de la nomenclature Support
			sql+="select wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media,wp.id_periodicity";
			// Sélection de la date
			sql+=", "+dateFieldName+" as date_num";
			// Sélection de l'unité
			sql+=", " + unitFieldName;
			// Tables
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle vh, "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category ct, "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media md, "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ";
			// Conditions de jointure
			sql+=" Where vh.id_vehicle=wp.id_vehicle ";
			sql+=" and ct.id_category=wp.id_category ";
			sql+=" and md.id_media=wp.id_media ";
			sql+=" and vh.id_language="+webSession.SiteLanguage.ToString();
			sql+=" and ct.id_language="+webSession.SiteLanguage.ToString();
			sql+=" and md.id_language="+webSession.SiteLanguage.ToString();
			// Activation
			sql+=" and vh.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

			// Période
			sql+=" and "+dateFieldName+">="+webSession.PeriodBeginningDate;
			sql+=" and "+dateFieldName+"<="+webSession.PeriodEndDate;
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en accès
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
			// Produit à exclure en radio
			sql+=SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false);
			#endregion

			#region Nomenclature Produit (Niveau de détail)  
			// Niveau de produit
			sql+=SQLGenerator.getLevelProduct(webSession,"wp",true);
			#endregion

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

			#region Sélection
			// Sélection en accès
			premier=true;
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			if(list.Length>0){
				sql+=" and ((wp.id_holding_company in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_advertiser in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
			if(list.Length>0)
			{
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_brand in ("+list+") ";
				premier=false;
			}

			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_product in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
			if(list.Length>0){
				sql+=" and ((wp.id_sector in ("+list+") ";
				premier=false;
			}
			// subsector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_subsector in ("+list+") ";
				premier=false;
			}
			// Group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_group_ in ("+list+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			
			// Sélection en Exception
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_holding_company not in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_advertiser not in ("+list+") ";
				premier=false;
			}
			// Marque
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
			if(list.Length>0)
			{
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_brand not in ("+list+") ";
				premier=false;
			}
			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_product not in ("+list+") ";
				premier=false;
			}
			// Sector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_sector not in ("+list+") ";
				premier=false;
			}
			// SubSector
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_subsector not in ("+list+") ";
				premier=false;
			}
			// group
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_group_ not in ("+list+") ";
				premier=false;
			}

			if(!premier) sql+=" )";
			#endregion

			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
			#endregion

			#region Sélection
			list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
			if(list.Length>0) sql+=" and ((wp.id_vehicle in ("+list+"))) ";
			#endregion

			#endregion

			
	
			// Ordre
			sql+="Group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media,wp.id_periodicity ";
			// et la date
			sql+=", "+dateFieldName+" ";

			// Ordre
			sql+="Order by wp.id_vehicle, vehicle, category, media ";
			// et la date
			sql+=", "+dateFieldName+" ";

			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaPlanAnalysisDataAccessException ("Impossible de charger le plan media: "+sql,err));
			}
			#endregion
			
		}

		#region Méthode internes
		
		/// <summary>
		/// Indique la table à utilisée pour la requête
		/// </summary>
		/// <param name="periodType">Type de période</param>
		/// <returns>La table correspondant au type de période</returns>
		private static string GetTableName(CustomerSessions.Period.DisplayLevel periodType){
			switch(periodType){
				case CustomerSessions.Period.DisplayLevel.monthly:
					return(Tables.WEB_PLAN_MEDIA_MONTH);
				case CustomerSessions.Period.DisplayLevel.weekly:
					return(Tables.WEB_PLAN_MEDIA_WEEK);
				default:
					throw(new MediaPlanAnalysisDataAccessException("Le détails période sélectionné est incorrect pour le choix de la table"));
			}
		}

		/// <summary>
		/// Indique le champ à utilisée pour la date dans la requête
		/// </summary>
		/// <param name="periodType">Type de période</param>
		/// <returns>Le champ correspondant au type de période</returns>
		private static string GetDateFieldName(CustomerSessions.Period.DisplayLevel periodType){
			switch(periodType){
				case CustomerSessions.Period.DisplayLevel.monthly:
					return(Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
				case CustomerSessions.Period.DisplayLevel.weekly:
					return(Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
				default:
					throw(new MediaPlanAnalysisDataAccessException("Le détails période sélectionné est incorrect pour le choix du champ"));
			}
		}
		#endregion


		/*** Nouvelle gestion des détails support ***/

		/// <summary>
		/// Obtient les champs correspondants au détail media demandée par le client.
		/// Les champs demandées corespondent à l'identifiant et le libellé du niveau support
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <returns>Chaîne contenant les champs</returns>
		public static string GetMediaFields(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.CATEGORY_PREFIXE+".id_category, category, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					return(" "+DbTables.VEHICLE_PREFIXE+".id_vehicle, vehicle, "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center, interest_center, "+DbTables.MEDIA_PREFIXE+".id_media, media ");
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
		public static string GetOrderMediaFields(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					return(" vehicle, category ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					return(" vehicle, category,media ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
					return(" vehicle,media ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
					return(" vehicle, interest_center ");
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					return(" vehicle,interest_center, media ");
				default:
					throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			}
		
		}

		/// <summary>
		/// Obtient les nom des tables à utiliser lors d'un détail media
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <returns>Chaîne contenant les tables</returns>
		public static string GetMediaTable(CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			string tmp="";
			//Vehicles
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE+", ";
					break;
			}
			
			//Categories
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.CATEGORY+" "+DbTables.CATEGORY_PREFIXE+", ";
					break;
			}

			// Media
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.MEDIA+" "+DbTables.MEDIA_PREFIXE+", ";
					break;
			}

			// Interest center
			switch(preformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" "+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.INTEREST_CENTER+" "+DbTables.INTEREST_CENTER_PREFIXE+", ";
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
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle ";
					break;
			}
			
			//Categories
			switch(webSession.PreformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_category="+dataTablePrefixe+".id_category ";
					break;
			}

			// Media
			switch(webSession.PreformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_media="+dataTablePrefixe+".id_media ";
					break;
			}

			// Interest center
			switch(webSession.PreformatedMediaDetail){
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
				case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage;
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
					tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center="+dataTablePrefixe+".id_interest_center ";
					break;
			}
			if(tmp.Length==0)throw(new SQLGeneratorException("Le détail support demandé n'est pas valide"));
			if(!beginByAnd)tmp=tmp.Substring(4,tmp.Length-4);
			return(tmp);
		}
		
	}
}
