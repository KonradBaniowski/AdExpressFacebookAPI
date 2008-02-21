#region Informations
// Auteur: A. Obermeyer 
// Date de création: 07/02/2005 
// Date de modification
// 23/08/2005	G. Facon		Solution temporaire pour les IDataSource
// 10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
// 25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using Oracle.DataAccess.Client;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Chargement des données pour le module Tendance
	/// </summary>
	public class TendenciesDataAccess{

		/// <summary>
		/// Dataset avec les données sous total nécessaire au module tendance
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="vehicleName">Nom du vehicle</param>
		/// <returns>DataSet</returns>
		public static DataSet GetDataTendencies(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName){

			#region Variables
			string dataTableName="";
			string dataTotalTableName="";
			string getUnitsFields="";
			string sql="";
			#endregion

			#region Construction de la requête
			dataTableName = WebFunctions.SQLGenerator.GetTableNameForTendency(webSession.DetailPeriod);
			dataTotalTableName = WebFunctions.SQLGenerator.GetTotalTableNameForTendency(webSession.DetailPeriod);
			getUnitsFields = WebFunctions.SQLGenerator.GetUnitFieldsForTendency(vehicleName,DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE, DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE);

			sql+="select "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".DATE_PERIOD, ";
			
			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.outdoor:
					sql+= DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".MEDIA, ";
					break;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
					sql+= DBConstantes.Tables.TITLE_PREFIXE+".TITLE as media, ";
					break;
				default:
					throw(new TendenciesDataAccessException ("Impossible de déterminer le media"));					
			}
			
			sql+= DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_CATEGORY, ";
			sql+= DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".CATEGORY, ";
			sql+= getUnitsFields;
			sql+= " from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+", ";
			sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTotalTableName +" "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE;

			if(vehicleName==DBClassificationConstantes.Vehicles.names.press || vehicleName==DBClassificationConstantes.Vehicles.names.internationalPress){
				sql+= ","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".TITLE "+DBConstantes.Tables.TITLE_PREFIXE;
				sql+= ","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".MEDIA "+DBConstantes.Tables.MEDIA_PREFIXE;
			}

			sql+= " where ";
			if (webSession.PDM){
				sql+= DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_PDM = "+ DBConstantes.Hathor.PDM_TRUE;
			}
			else{
				sql+= DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_PDM = "+ DBConstantes.Hathor.PDM_FALSE;
			}
			
			if(webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate){
				sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_CUMULATIVE ="+DBConstantes.Hathor.CUMULATIVE_TRUE;
			}
			else{
				sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".DATE_PERIOD between "+webSession.PeriodBeginningDate+ " and "+webSession.PeriodEndDate;
				sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_CUMULATIVE ="+DBConstantes.Hathor.CUMULATIVE_FALSE;
			}


			sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_VEHICLE = "+vehicleName.GetHashCode().ToString();
			sql+=" and "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".ID_TYPE_TENDENCY = "+DBConstantes.Hathor.TYPE_TENDENCY_SUBTOTAL;	
			sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_PDM = "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".ID_PDM";
			sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".CATEGORY = "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".CATEGORY";
			sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".ID_CUMULATIVE = "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".ID_CUMULATIVE";

			sql+=" and "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".DATE_PERIOD = "+DBConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".DATE_PERIOD";
			
			if(vehicleName==DBClassificationConstantes.Vehicles.names.press || vehicleName==DBClassificationConstantes.Vehicles.names.internationalPress){
				sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage;
				sql+=" and "+DBConstantes.Tables.TITLE_PREFIXE+".id_language="+webSession.SiteLanguage;
				sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;
				sql+=" and "+DBConstantes.Tables.TITLE_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;
				sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_title="+DBConstantes.Tables.TITLE_PREFIXE+".id_title "; 
				sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media="+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_media "; 
				sql+=" group by "+DBConstantes.Tables.TITLE_PREFIXE+".title ";
				sql+=", "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_category";
				sql+=", "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".category ";
				sql+=", "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".DATE_PERIOD";
			}
			

			sql+=" order by "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".category";
				
			
			if(vehicleName==DBClassificationConstantes.Vehicles.names.press || vehicleName==DBClassificationConstantes.Vehicles.names.internationalPress){
				sql+=","+DBConstantes.Tables.TITLE_PREFIXE+".title ";
			}else{
				sql+=","+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".media ";
			}
			//sql+=" group by "+DBConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_category";

			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new TendenciesDataAccessException ("Impossible de charger les tendances: "+sql,err));
			}
			#endregion

		}

		/// <summary>
		/// Retourne la ligne totale d'un média
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="vehicleName">Nom du média</param>
		/// <returns></returns>
		public static DataSet GetTotalTendencies(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName){
		
			#region Variables
			string dataTotalTableName="";			
			string sql="";
			#endregion

			#region Construction de la requête
			dataTotalTableName = WebFunctions.SQLGenerator.GetTotalTableNameForTendency(webSession.DetailPeriod);

			sql+="select * from adexpr03."+dataTotalTableName;
			sql+=" where id_vehicle="+vehicleName.GetHashCode().ToString();
			
			if (webSession.PDM){
				sql+=" and ID_PDM = "+ DBConstantes.Hathor.PDM_TRUE;
			}
			else{
				sql+="and ID_PDM = "+ DBConstantes.Hathor.PDM_FALSE;
			}			
			sql+=" and id_type_tendency="+DBConstantes.Hathor.TYPE_TENDENCY_TOTAL;		
			if(webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate){
				sql+=" and ID_CUMULATIVE ="+DBConstantes.Hathor.CUMULATIVE_TRUE;
			}
			else{
				sql+=" and DATE_PERIOD between "+webSession.PeriodBeginningDate+ " and "+webSession.PeriodEndDate;
				sql+=" and ID_CUMULATIVE ="+DBConstantes.Hathor.CUMULATIVE_FALSE;
			}
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new TendenciesDataAccessException ("Impossible de charger les tendances: "+sql,err));
			}
			#endregion

		}
		
		/// <summary>
		/// Dataset avec les données nécessaire au module tendance
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="vehicleName">Nom du vehicle</param>
		/// <returns>DataSet</returns>
		public static DataSet GetData(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName){
			
			#region Variables

			string dataTableName="";
			string getUnitsFields="";
			string getMediaFields="";
			string getTableForTendencies="";
			string getPeriodForTendencies="";
			string getJointForTendencies="";
			string getGroupByForTendencies="";
			string getOrderByForTendencies="";
			//string productsRights="";
			//string mediaRights="";
			string listExcludeProduct="";
			string listMedia="";
			string sql="";
		
			#endregion

			#region Construction de la requête
			dataTableName=WebFunctions.SQLGenerator.getTableNameForDashBoardResult(webSession.DetailPeriod);
			getUnitsFields=WebFunctions.SQLGenerator.getTotalUnitFields(vehicleName,DBConstantes.Tables.WEB_PLAN_PREFIXE);
			getMediaFields=WebFunctions.SQLGenerator.getMediaFieldsForTendencies(vehicleName);
			getTableForTendencies=WebFunctions.SQLGenerator.getTableForTendencies(vehicleName);
			getPeriodForTendencies=WebFunctions.SQLGenerator.getPeriodForTendencies(webSession.DetailPeriod,webSession.PeriodBeginningDate,webSession.PeriodEndDate,GetPeriodN1(webSession.PeriodBeginningDate),GetPeriodN1(webSession.PeriodEndDate));
			getJointForTendencies=WebFunctions.SQLGenerator.getJointForTendencies(webSession,vehicleName);
			getGroupByForTendencies=WebFunctions.SQLGenerator.getGroupByForTendencies(vehicleName);
			getOrderByForTendencies=WebFunctions.SQLGenerator.getOrderByForTendencies(vehicleName);
		//	productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true);
		//	mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DBConstantes.Tables.WEB_PLAN_PREFIXE,true);
			listExcludeProduct=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DBConstantes.Tables.WEB_PLAN_PREFIXE,true,false);

			int idMediaListToLoad;
			switch(vehicleName){
				//TODO:INTER
				case DBClassificationConstantes.Vehicles.names.press:
					idMediaListToLoad=WebConstantes.AdExpressUniverse.TENDENCY_PRESS_MEDIA_LIST_ID;
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
					idMediaListToLoad=WebConstantes.AdExpressUniverse.TENDENCY_RADIO_MEDIA_LIST_ID;
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
					idMediaListToLoad=WebConstantes.AdExpressUniverse.TENDENCY_TV_MEDIA_LIST_ID;
					break;
				case DBClassificationConstantes.Vehicles.names.others:
					idMediaListToLoad=WebConstantes.AdExpressUniverse.TENDENCY_PANEURO_MEDIA_LIST_ID;
					break;
				default:
					throw(new TendenciesDataAccessException ("Impossible de charger la liste des média à afficher"));
			}
			listMedia=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(idMediaListToLoad,DBConstantes.Tables.WEB_PLAN_PREFIXE,true);

			sql+=" select "+getUnitsFields+", "+getMediaFields+" ";
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					sql+=" ,month_media_num as period";
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly :
				case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly :
					sql+=" ,week_media_num as period";
					break;
				default:
					throw(new TendenciesDataAccessException ("Impossible de charger la période"));
			}	
			sql+=" from  "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DBConstantes.Tables.WEB_PLAN_PREFIXE+" ";
			sql+=","+getTableForTendencies;
			sql+=" where  "+DBConstantes.Tables.WEB_PLAN_PREFIXE+".id_vehicle="+vehicleName.GetHashCode().ToString()+" ";
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle="+vehicleName.GetHashCode().ToString()+" ";
			sql+=getPeriodForTendencies;
			sql+=getJointForTendencies;
			sql+=listMedia;
			sql+=listExcludeProduct;
			sql+=getGroupByForTendencies;
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					sql+=" ,month_media_num";
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly :
				case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly :
					sql+=" ,week_media_num";
					break;
				default:
					throw(new TendenciesDataAccessException ("Impossible de charger la période"));
			}

			sql+=getOrderByForTendencies;
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new TendenciesDataAccessException ("Impossible de charger les tendances: "+sql,err));
			}
			#endregion
		
		}

		#region Méthodes Internes

		/// <summary>
		/// Fournit la période N-1
		/// </summary>
		/// <param name="period">periode N</param>
		/// <returns>période N-1</returns>
		internal static string GetPeriodN1(string period){
			
			int year=int.Parse(period.Substring(0,4));
			year=year-1;

			return (year.ToString()+period.Substring(4,2));
		}
		#endregion
		
	}
}
